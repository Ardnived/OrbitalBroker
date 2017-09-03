using System;
using UnityEngine;

[Serializable]
public struct Facility : OwnedDataElement {

	[SerializeField]
	private int _ID;
	public int ID { get { return _ID; } set { _ID = value; } }

	[SerializeField]
	private int _OwnerID;
	public int OwnerID { get { return _OwnerID; } set { _OwnerID = value; } }

	public bool IsHQ { get { return _ID == 1; } }

	public string Name;
	public LatLong Location;
	public FacilityMode Mode;

	public FacilityStatus Status;
	public int FundsPerTurn;
	public int FundsRequired;
	public int FundsReceived;

	public ProjectLaunchData LaunchData;

	public int TurnsLeft {
		get { return FundsPerTurn == 0 ? -1 : Mathf.CeilToInt((FundsRequired - FundsReceived) / (float) FundsPerTurn); }
	}

	public void SwitchMode(FacilityMode mode) {
		this.Mode = mode;
	}

	public void AdvanceProject(int amount) {
		if (Status == FacilityStatus.PreConstruction) {
			Status = FacilityStatus.ActiveConstruction;
		} else if (Status == FacilityStatus.NoProject && Mode == FacilityMode.Research) {
			Status = FacilityStatus.ActiveProject;
		} else if (Status == FacilityStatus.NoProject && Mode == FacilityMode.Launch) {
			if (LaunchData.BlueprintID >= 0) {
				Status = FacilityStatus.ActiveProject;
			} else {
				return;
			}
		}

		FundsReceived += amount;

		if (FundsReceived >= FundsRequired) {
			OnComplete();
		}
	}

	private void OnComplete() {
		if (Status == FacilityStatus.ActiveProject) {
			switch (Mode) {
			case FacilityMode.Launch:
				Faction faction = GameController.Data.Factions[OwnerID];
				SatelliteController blueprint = faction.Blueprints.Get(LaunchData.BlueprintID);
				SatelliteController satellite = SatelliteManager.Instance.CreateSatellite(blueprint);
				satellite.Data.Orbit = LaunchData.Orbit;

				Debug.Log("Adjusting "+MarketType.Research+" supply by "+satellite.Data.ResearchCapacity);
				Debug.Log("Adjusting "+MarketType.Broadcast+" supply by "+satellite.Data.BroadcastCapacity);
				Debug.Log("Adjusting "+MarketType.Sensor+" supply by "+satellite.Data.SensorCapacity);
				Debug.Log("Adjusting "+MarketType.Energy+" supply by "+satellite.Data.EnergyCapacity);

				GameController.Data.Markets.AdjustSupply(MarketType.Research, satellite.Data.Orbit, satellite.Data.ResearchCapacity);
				GameController.Data.Markets.AdjustSupply(MarketType.Broadcast, satellite.Data.Orbit, satellite.Data.BroadcastCapacity);
				GameController.Data.Markets.AdjustSupply(MarketType.Sensor, satellite.Data.Orbit, satellite.Data.SensorCapacity);
				GameController.Data.Markets.AdjustSupply(MarketType.Energy, satellite.Data.Orbit, satellite.Data.EnergyCapacity);

				faction.AdjustMarketSupply(MarketType.Research, satellite.Data.Orbit, satellite.Data.ResearchCapacity);
				faction.AdjustMarketSupply(MarketType.Broadcast, satellite.Data.Orbit, satellite.Data.BroadcastCapacity);
				faction.AdjustMarketSupply(MarketType.Sensor, satellite.Data.Orbit, satellite.Data.SensorCapacity);
				faction.AdjustMarketSupply(MarketType.Energy, satellite.Data.Orbit, satellite.Data.EnergyCapacity);

				faction.Satellites.Set(satellite);

				this.LaunchData.BlueprintID = -1;
				this.FundsRequired = 0;

				GameController.Data.Logs.Add(new Log("Launch Complete: "+satellite.Data.Name, OnLogClick));
				break;
			case FacilityMode.Research:
				GameController.Data.Factions[OwnerID].TechPoints++;

				if (OwnerID == Constant.PlayerFactionID) {
					On.UpdateTechPoints.Trigger();
				}

				//this.FundsRequired = Constant.CostPerTechPoint;
				break;
			}
		} else if (Status == FacilityStatus.ActiveConstruction) {
			switch (Mode) {
			case FacilityMode.Launch:
				LaunchData.BlueprintID = -1;
				break;
			case FacilityMode.Research:
				this.FundsRequired = Constant.CostPerTechPoint;
				this.FundsPerTurn = 0;
				break;
			}

			GameController.Data.Logs.Add(new Log("Construction Complete: "+Name, OnLogClick));
		}

		this.FundsReceived -= this.FundsRequired;
		Status = FacilityStatus.NoProject;
	}

	private void OnLogClick() {
		ViewManager.Instance.Show(this);
	}

	public void AbandonProject() {
		// TODO: Maybe this shouldn't actually happen until the turn advancement?
		GameController.Data.Factions[OwnerID].Funds += (int) (FundsReceived * 0.1f);

		Status = FacilityStatus.NoProject;
		FundsPerTurn = 0;
		FundsReceived = 0;
		FundsRequired = 0;
		LaunchData.BlueprintID = -1;
	}

	[Serializable]
	public struct ProjectLaunchData {
		public int BlueprintID;
		public OrbitData Orbit;
	}

}

public enum FacilityMode {
	Launch,
	Research,
}

public enum FacilityStatus {
	PreConstruction,
	ActiveConstruction,
	NoProject,
	ActiveProject,
}

