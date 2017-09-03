using System;
using UnityEngine;

[Serializable]
public struct ModuleDesign {
	public string Name;
	public int ID;

	public int EnergyConsumption;
	public int Weight;
	public bool RequiresAstronaut;

	public ModuleType Type;
	public int Capacity;

	public ModuleModel Model;
}

public enum ModuleType {
	Core,
	Solar,
	Research,
	Sensor,
	Broadcast,
	Energy,

	GPS,
	AstroLab,

	LunarPayload,
	Hydroponics,
	WaterRecycler,
	OxygenRecycler,
	RadiationShield,
}

public enum ModuleModel {
	Core,
	Solar1,
	Solar2,
	Solar3,
	Research1,
	Research2,
	Research3,
	Sensor1,
	Sensor2,
	Sensor3,
	Broadcast1,
	Broadcast2,
	Broadcast3,
	Energy1,
	Energy2,
	Energy3,

	Hydroponics,
	LifeSupport,
	WaterCycler,
	LunarPayload,
}