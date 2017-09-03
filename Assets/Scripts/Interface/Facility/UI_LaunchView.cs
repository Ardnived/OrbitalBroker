using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_LaunchView : MonoBehaviour/*, DataObserver*/ {

	public UI_FacilityView facilityView;
	public Material mapMaterial;

	[Header("UI Elements")]
	public Dropdown blueprintDropdown;
	public Text altitudeLabel;
	public Slider altitudeSlider;
	public Text inclinationLabel;
	public Slider inclinationSlider;
	public Text eccentricityLabel;
	public Slider eccentricitySlider;
	public Text rotationLabel;
	public Slider rotationSlider;
	public Text detailText;

	private DataCollection<SatelliteController> blueprints;

	private Facility.ProjectLaunchData launchData;
	private int[] blueprintIDs;

	public void Start() {
		On.UpdateBlueprints.Do(SetDataHandler);
	}

	void SetDataHandler() {
		blueprints = GameController.Data.PlayerFaction.Blueprints;
		blueprintIDs = new int[blueprints.Count];
		List<string> options = new List<string>(blueprintIDs.Length);
		blueprintDropdown.ClearOptions();

		int i = 0;
		foreach (SatelliteController blueprint in blueprints) {
			options.Add(blueprint.Data.Name);
			blueprintIDs[i] = blueprint.ID;
		}

		blueprintDropdown.AddOptions(options);
		blueprintDropdown.value = System.Array.IndexOf(blueprintIDs, launchData.BlueprintID);
		blueprintDropdown.RefreshShownValue();
	}

	public void Set(Facility.ProjectLaunchData launchData) {
		this.launchData = launchData;

		if (blueprintIDs != null) {
			blueprintDropdown.value = System.Array.IndexOf(blueprintIDs, launchData.BlueprintID);
			blueprintDropdown.RefreshShownValue();
		}

		if (launchData.Orbit.Altitude >= 50000) {
			altitudeSlider.value = 4;
		} else if (launchData.Orbit.Altitude >= 35786) {
			altitudeSlider.value = 3;
		} else if (launchData.Orbit.Altitude >= 20000) {
			altitudeSlider.value = 2;
		} else if (launchData.Orbit.Altitude >= 1000) {
			altitudeSlider.value = 1;
		} else {
			altitudeSlider.value = 2;
		}

		inclinationSlider.value = launchData.Orbit.Inclination;
		eccentricitySlider.value = launchData.Orbit.Eccentricity;
		rotationSlider.value = launchData.Orbit.Rotation;

		UpdateOrbitDisplay();
	}

	public void SetBlueprintIndex(int index) {
		int id = blueprintIDs[index];
		launchData.BlueprintID = id;
		ShowDetails(blueprints.Get(id).Data);
	}

	public void SetAltitude(float value) {
		int level = (int)value;
		int altitude = 0;
		//int orbitalPeriod = 0;

		switch (level) {
		case 1:
			altitude = 1000;
			//orbitalPeriod = 5400;
			break;
		case 2:
			altitude = 20000;
			//orbitalPeriod = 14400;
			break;
		case 3:
			altitude = 35786;
			//orbitalPeriod = 43200;
			break;
		case 4:
			altitude = 50000;
			//orbitalPeriod = 86400;
			break;
		}

		altitudeLabel.text = "Altitude: " + altitude + "km";
		//orbit.Set(orbitalPeriod: orbitalPeriod);
		launchData.Orbit.Altitude = altitude;
		UpdateOrbitDisplay();
	}

	public void SetInclination(float inclination) {
		inclinationLabel.text = "Inclination: " + inclination + "˚";
		launchData.Orbit.Inclination = (int)inclination;
		UpdateOrbitDisplay();
	}

	public void SetEccentricity(float eccentricity) {
		eccentricityLabel.text = "Eccentricity: " + eccentricity;
		launchData.Orbit.Eccentricity = eccentricity;
		UpdateOrbitDisplay();
	}

	public void SetRotation(float rotation) {
		rotationLabel.text = "Rotation: " + rotation + "˚";
		launchData.Orbit.Rotation = (int) rotation;
		UpdateOrbitDisplay();
	}

	public void UpdateOrbitDisplay() {
		mapMaterial.SetInt("_HighlightCountry_Count", 0);
		mapMaterial.SetInt("_Coverage_Highlight", 0);
		mapMaterial.SetInt("_Coverage_Count", 1);

		launchData.Orbit.RecalculateCoverage();
		mapMaterial.SetVectorArray("_Coverage", launchData.Orbit.GetCoverage());
	}

	public void ShowDetails(SatelliteData data) {
		string text = "Sun Exposure: \r\n"
			+ "Energy Consumption: " + data.EnergyConsumption + "/hr\r\n"
			+ "Energy Production: " + data.EnergyProduction + "/hr\r\n"
			+ "Weight: " + data.TotalWeight + " tons\r\n"
			+ "\r\n";

		StringBuilder str = new StringBuilder(text);

		if (data.ResearchCapacity > 0) {
			str.AppendLine("Research Capacity: " + data.ResearchCapacity);
		}

		if (data.SensorCapacity > 0) {
			str.AppendLine("Sensor Capacity: " + data.SensorCapacity);
		}

		if (data.BroadcastCapacity > 0) {
			str.AppendLine("Broadcast Capacity: " + data.BroadcastCapacity);
		}

		detailText.text = str.ToString();
	}

	public void Confirm() {
		//facilityView.SetLaunchData(launchData);
	}

}
