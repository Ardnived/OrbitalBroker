using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SatelliteData {
	public string Name;
	public int OwnerID;
	public int ID;

	[Header("Status")]
	//public float Energy;
	//[Range(0, 100)]
	//public int SunExposure;
	[Range(0, 100)]
	public int AverageRepair; // out of 100

	[Header("Properties")]
	public int TotalWeight;
	public int EnergyConsumption;

	[Header("Capabilities")]
	public int ResearchCapacity;
	public int BroadcastCapacity;
	public int SensorCapacity;
	public int EnergyCapacity;


	public int EnergyProduction;

	[Header("Modules")]
	public List<ModuleData> Modules;

	[Header("Orbit")]
	public OrbitData Orbit;

	public void CalculateProperties() {
		AverageRepair = 0;
		TotalWeight = 0;
		EnergyConsumption = 0;

		EnergyProduction = 0;
		ResearchCapacity = 0;
		SensorCapacity = 0;
		BroadcastCapacity = 0;

		foreach (ModuleData module in Modules) {
			ModuleDesign moduleDesign = module.GetDesign();

			TotalWeight += moduleDesign.Weight;
			EnergyConsumption += moduleDesign.EnergyConsumption;
			AverageRepair += module.Repair;

			switch (moduleDesign.Type) {
			case ModuleType.Energy:
				EnergyProduction += moduleDesign.Capacity;
				break;
			case ModuleType.Research:
				ResearchCapacity += moduleDesign.Capacity;
				break;
			case ModuleType.Broadcast:
				BroadcastCapacity += moduleDesign.Capacity;
				break;
			case ModuleType.Sensor:
				SensorCapacity += moduleDesign.Capacity;
				break;
			}
		}

		AverageRepair /= Modules.Count;
	}

	public int AddModule(ModuleData data) {
		Modules.Add(data);
		return Modules.Count - 1;
	}

	public void RemoveModule(int moduleIndex) {
		ModuleData data = Modules[moduleIndex];

		if (data != null) {
			Modules[moduleIndex] = null;

			foreach (ModuleData.Child child in data.Children) {
				RemoveModule(child.ModuleIndex);
			}
		}
	}

}
