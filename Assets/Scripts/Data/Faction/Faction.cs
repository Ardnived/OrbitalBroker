using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class Faction {

	public string Name;
	public int ID;

	public int Funds;
	public int TechPoints;

	[SerializeField]
	private SatelliteData[] serializedBlueprintData;
	[NonSerialized]
	public SatelliteDataCollection Blueprints;

	[SerializeField]
	private SatelliteData[] serializedSatelliteData;
	[NonSerialized]
	public SatelliteDataCollection Satellites;

	[SerializeField]
	private Facility[] serializedFacilities;
	[NonSerialized]
	public OwnedDataCollection<Facility> Facilities;

	[SerializeField]
	public int[] ResearchLevels;
	[SerializeField]
	public int[] TotalMarketSupply;
	[SerializeField]
	private int[] marketSupply;

	public int TechFlags;
	public List<int> AvailableModuleIDs;

	[NonSerialized]
	public FactionVictoryStatus VictoryStatus;

	[SerializeField]
	public FactionTechStatus TechStatus;

	[SerializeField]
	public FactionMarketStatus MarketStatus;

	public Faction(int id, string name) {
		this.ID = id;
		this.Name = name;

		this.Funds = Constant.Init.Funds;
		this.TechPoints = Constant.Init.TechPoints;

		this.marketSupply = new int[Constant.Market.TypeCount * GameController.Map.Width * GameController.Map.Height];
		this.TotalMarketSupply = new int[Constant.Market.TypeCount];
		this.AvailableModuleIDs = new List<int>() { 0 };

		ReInitializeVariables();

		this.Blueprints.Publish();
		this.Satellites.Publish();
		this.Facilities.Publish();

		Facility hq = new Facility();
		hq.ID = 0;
		hq.Name = this.Name + " HQ";
		hq.Status = FacilityStatus.NoProject;
		Facilities.Set(hq);

		this.ResearchLevels = new int[GameController.TechWeb.Techs.Length];

		for (int i = 0; i < ResearchLevels.Length; i++) {
			this.ResearchLevels[i] = 0;
		}
	}

	public void HandleDataFromLoad() {
		Debug.Log("Deserialize "+this.Name + " #"+ID);
		ReInitializeVariables();

		Blueprints.Deserialize(serializedBlueprintData, asBlueprint: true);
		Satellites.Deserialize(serializedSatelliteData);
		Facilities.Deserialize(serializedFacilities);
	}

	private void ReInitializeVariables() {
		this.VictoryStatus = new FactionVictoryStatus();

		if (ID == Constant.PlayerFactionID) {
			this.Blueprints = new SatelliteDataCollection(ID, On.UpdateBlueprints);
			this.Satellites = new SatelliteDataCollection(ID, On.UpdateSatellites);
			this.Facilities = new OwnedDataCollection<Facility>(ID, On.UpdateFacilities);
		} else {
			this.Blueprints = new SatelliteDataCollection(ID, null);
			this.Satellites = new SatelliteDataCollection(ID, null);
			this.Facilities = new OwnedDataCollection<Facility>(ID, null);
		}
	}

	public void PrepareDataForSave() {
		serializedBlueprintData = Blueprints.Serialize();
		serializedSatelliteData = Satellites.Serialize();
		serializedFacilities = Facilities.Serialize();
	}

	public void AdjustMarketSupply(MarketType type, OrbitData orbit, int value) {
		if (value == 0) {
			return;
		}

		int t = (int)type;
		int totalSupply = 0;
		int index;

		Debug.Log("Orbit has "+orbit.Coverage.Length+" coverage points.");
		for (int i = 0; i < orbit.Coverage.Length; i++) {
			OrbitData.CoveragePoint point = orbit.Coverage[i];

			for (int x = -point.Radius; x <= point.Radius; x++) {
				for (int y = -point.Radius; y <= point.Radius; y++) {
					LatLong latlong = new LatLong(point.Latitude + x, point.Longitude + y);

					int latDiff = latlong.Latitude - point.Latitude;
					int lonDiff = latlong.Longitude - point.Longitude;
					float distance = Mathf.Sqrt(latDiff * latDiff + lonDiff * lonDiff);

					if (distance <= point.Radius) {
						try {
							index = GetMarketSupplyIndex(t, latlong.Latitude - Constant.MinLatitude, latlong.Longitude - Constant.MinLongitude);
							marketSupply[index] += value;
							totalSupply += value;
						} catch (Exception exc) {
							Debug.Log("Attempting to set "+t+" "+(latlong.Latitude - Constant.MinLatitude)+", "+(latlong.Longitude - Constant.MinLongitude)+" += "+value);
							throw exc;
						}
					}
				}
			}
		}

		Debug.Log("Added "+value+" -> "+totalSupply+" supply to faction #"+ID+"'s "+type+" market");

		TotalMarketSupply[t] += totalSupply;
	}

	public int GetMarketSupply(MarketType type, int x, int y) {
		int index = GetMarketSupplyIndex((int)type, x, y);
		return marketSupply[index];
	}

	private int GetMarketSupplyIndex(int t, int x, int y) {
		return x + GameController.Map.Width * (y + GameController.Map.Height * t);
	}

	public bool HasFlag(TechFlag flag) {
		return (this.TechFlags & (int)flag) == (int)flag;
	}

	public void BuyTech(int techID, bool cancel = false) {
		if (TechStatus.MarkTech(techID, !cancel)) {
			GlobalTechState techState = GameController.Data.TechStatus[techID];

			if (cancel) {
				TechPoints++;

				if (techState.Status == GlobalTechState.TechStatus.Copyright) {
					Funds += Constant.CostPerCopyrightPoint;
					GameController.Data.Factions[techState.OwnerID].Funds -= Constant.CostPerCopyrightPoint;

					if (ID == Constant.PlayerFactionID) {
						On.UpdateFunds.Trigger();
					}
				}
			} else {
				TechPoints--;

				if (techState.Status == GlobalTechState.TechStatus.Copyright) {
					Funds -= Constant.CostPerCopyrightPoint;
					GameController.Data.Factions[techState.OwnerID].Funds += Constant.CostPerCopyrightPoint;

					if (ID == Constant.PlayerFactionID) {
						On.UpdateFunds.Trigger();
					}
				}
			}

			if (ID == Constant.PlayerFactionID) {
				On.UpdateTechPoints.Trigger();
			}
		}
	}

	/*
	public struct FundsLog {

		public int Value;
		private Dictionary<string, int> income = new Dictionary<string, int>();
		private Dictionary<string, int> expenditure = new Dictionary<string, int>();

		public void Add(int amount, string reason) {
			if (amount >= 0) {
				Add(income, amount, reason);
			} else {
				Add(expenditure, amount, reason);
			}
		}

		private void Add(Dictionary<string, int> dict, int amount, string reason) {
			if (dict.ContainsKey(reason)) {
				dict[reason] += amount;
			} else {
				dict.Add(reason, amount);
			}
		}

		public void Apply() {

		}

	}
	*/
}