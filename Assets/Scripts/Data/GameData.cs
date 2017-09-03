using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class GameData {

	private static BinaryFormatter binaryFormatter = new BinaryFormatter();

	public Faction PlayerFaction {
		get {
			return Factions[Constant.PlayerFactionID];
		}
	}

	public int CurrentTurn = 0;
	public Faction[] Factions;
	public MarketMap Markets;
	public GlobalTechState[] TechStatus;
	public List<Log> Logs = new List<Log>();

	public void Save(string fileName) {
		FileStream file = File.Open(Application.persistentDataPath + "/" + fileName + ".game", FileMode.OpenOrCreate);
		binaryFormatter.Serialize(file, this);
		file.Close();
		Debug.Log("Saved game: " + fileName + ".game");
	}

	public static GameData Load(string saveName) {
		string path = Application.persistentDataPath + "/" + saveName + ".game";

		if (File.Exists(path)) {
			FileStream file = File.Open(path, FileMode.Open);
			GameData data = (GameData) binaryFormatter.Deserialize(file);
			file.Close();

			Debug.Log("Loaded game: " + saveName + ".game");
			return data;
		} else {
			Debug.LogError("Tried to load a file which doesn't exist: "+path);
		}

		return null;
	}

	public static GameData New(FactionSet factionSet) {
		GameData gameData = new GameData();
		gameData.Init(factionSet);
		return gameData;
	}

	private void Init(FactionSet factionSet) {
		Debug.Log("Initialize Game");
		this.Markets = new MarketMap(GameController.Map);

		this.TechStatus = new GlobalTechState[GameController.TechWeb.Techs.Length];

		this.Factions = new Faction[factionSet.Factions.Length];
		for (int i = 0; i < factionSet.Factions.Length; i++) {
			this.Factions[i] = new Faction(i, factionSet.Factions[i].Name);

			for (int n = 0; n < Constant.Init.Techs.Length; n++) {
				GameController.TechWeb.AwardTech(Factions[i], Constant.Init.Techs[n]);
			}
		}

		RegisterListeners();
	}

	public void HandleDataForSave() {
		for (int i = 0; i < Factions.Length; i++) {
			Factions[i].PrepareDataForSave();
		}
	}

	public void HandleDataFromLoad() {
		for (int i = 0; i < Factions.Length; i++) {
			Factions[i].HandleDataFromLoad();
		}

		Markets.ReInitializeVariables(GameController.Map);
		RegisterListeners();
	}

	public void RegisterListeners() {
		On.TurnAdvanceMain.Do(OnTurnAdvanceMain);
		On.TurnAdvanceLate.Do(OnTurnAdvanceLate);
	}

	private void OnTurnAdvanceEarly() {
		Markets.Simulate();
	}

	private void OnTurnAdvanceMain() {
		// Calculate income
		//Debug.Log(" ============= Calculating income ============= ");
		for (int t = 0; t < Constant.Market.TypeCount; t++) {
			MarketType type = (MarketType) t;

			//Debug.Log("Calculating income for "+type);

			for (int x = 0; x < GameController.Map.Width; x++) {
				for (int y = 0; y < GameController.Map.Height; y++) {
					int totalDemand = Markets.GetDemand(type, x, y);
					int totalSupply = Markets.GetSupply(type, x, y);

					if (totalDemand > 0 && totalSupply > 0) {
						for (int f = 0; f < Factions.Length; f++) {
							Faction faction = Factions[f];
							int factionSupply = faction.GetMarketSupply(type, x, y);
							//Debug.Log("Got supply "+t+" = "+factionSupply);

							if (factionSupply > 0) {
								int value = (int) (totalDemand * ((float)factionSupply / totalSupply) * Constant.ValuePerDemandPoint);

								faction.Funds += value;

								// TODO: Factions are getting income and supply for areas that they have no business in.
								//Debug.Log("Market "+x+", "+y+" = "+totalSupply+"/"+factionSupply+" vs "+totalDemand);
								//Debug.Log(faction.Name + " gains ¥" + value + " from " + type.ToString() + " markets.");
							}
						}
					}
				}
			}
		}
		//Debug.Log(" ============= ============= ============= ");

		// Advance research
		for (int f = 0; f < Factions.Length; f++) {
			Factions[f].TechStatus.Update();
		}

		// Calculate Expenses and advance projects
		List<Facility> modifiedFacilities = new List<Facility>();

		for (int f = 0; f < Factions.Length; f++) {
			Faction faction = Factions[f];
			modifiedFacilities.Clear();

			foreach (Facility facility in faction.Facilities) {
				int funds = Mathf.Min(faction.Funds, facility.FundsPerTurn, facility.FundsRequired - facility.FundsReceived);

				if (funds > 0 && facility.FundsRequired > 0) {
					faction.Funds -= funds;
					facility.AdvanceProject(funds);
					modifiedFacilities.Add(facility);
				}
			}

			for (int i = 0; i < modifiedFacilities.Count; i++) {
				faction.Facilities.Set(modifiedFacilities[i]);
			}
		}

		On.UpdateFunds.Trigger();
	}

	private void OnTurnAdvanceLate() {
		for (int f = 0; f < Factions.Length; f++) {
			Factions[f].VictoryStatus.UpdateAll(Factions[f]);
		}

		if (PlayerFaction.TechPoints > 0) {
			Logs.Add(new Log(PlayerFaction.TechPoints + " Unspent Tech Points"/*, ViewManager.Instance.ShowTechWeb*/));
		}
	}

}

public struct GlobalTechState {
	public int OwnerID;
	public TechStatus Status;

	public enum TechStatus {
		Secret,
		Copyright,
		PublicDomain,
	}
}

public enum MarketType {
	Research,
	Broadcast,
	Sensor,
	Energy
}
