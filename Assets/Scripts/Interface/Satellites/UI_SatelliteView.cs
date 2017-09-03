using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SatelliteView : MonoBehaviour {

	public Text text;

	public void Set(SatelliteData satellite) {
		string txt = "<b>"+satellite.Name+"</b>\r\n"
			+ "Repair: " + satellite.AverageRepair + "%\r\n"
			+ "Energy Consumption: " + satellite.EnergyConsumption + "/hr\r\n"
			+ "Energy Production: " + satellite.EnergyProduction + "/hr\r\n"
			+ "Weight: " + satellite.TotalWeight + " tons";

		StringBuilder str = new StringBuilder(txt);

		if (satellite.ResearchCapacity > 0) {
			str.AppendLine("Research Capacity: " + satellite.ResearchCapacity);
		}

		if (satellite.SensorCapacity > 0) {
			str.AppendLine("Sensor Capacity: " + satellite.SensorCapacity);
		}

		if (satellite.BroadcastCapacity > 0) {
			str.AppendLine("Broadcast Capacity: " + satellite.BroadcastCapacity);
		}

		text.text = str.ToString();
	}

}
