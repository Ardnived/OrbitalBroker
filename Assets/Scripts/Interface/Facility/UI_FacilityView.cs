using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FacilityView : MonoBehaviour {

	public RawImage mapRenderer;

	public GameObject launchControls;
	public GameObject researchControls;
	public GameObject constructionControls;

	[Header("General UI Elements")]
	public Text title;
	public Dropdown modeDropdown;

	public Text fundingLabel;
	public Slider fundingSlider;
	public Text turnsText;
	public Text priceText;

	public Toggle abandonToggle;

	[Header("Launch UI Elements")]
	public Dropdown blueprintsDropdown;
	public Text altitudeText;
	public Slider altitudeSlider;
	public Text inclinationText;
	public Slider inclinationSlider;
	public Text rotationText;
	public Slider rotationSlider;

	public Text weightText;

	[Header("Construction UI Elements")]
	public Dropdown typeDropdown;
	public InputField nameInput;
	public InputField latitudeInput;
	public InputField longitudeInput;

	private Facility facility;
	private DataCollection<SatelliteController> blueprints;
	private int[] blueprintIDs;

	public void Start() {
		On.UpdateBlueprints.Do(SetDataHandler);
	}

	void SetDataHandler() {
		this.blueprints = GameController.Data.PlayerFaction.Blueprints;
		this.blueprintIDs = new int[blueprints.Count];
		List<string> options = new List<string>(blueprintIDs.Length);

		int i = 0;
		foreach (SatelliteController blueprint in blueprints) {
			options.Add(blueprint.Data.Name);
			blueprintIDs[i] = blueprint.ID;
		}

		blueprintsDropdown.ClearOptions();
		blueprintsDropdown.AddOptions(options);
		blueprintsDropdown.value = System.Array.IndexOf(blueprintIDs, facility.LaunchData.BlueprintID);
		blueprintsDropdown.RefreshShownValue();


		/*
		options.Clear();
		foreach (FacilityMode mode in Enum.GetValues(typeof(FacilityMode))) {
			options.Add(mode.ToString());
		}

		modeDropdown.ClearOptions();
		modeDropdown.AddOptions(options);

		typeDropdown.ClearOptions();
		typeDropdown.AddOptions(options);
		*/
	}

	public void SetNewFacility() {
		Facility facility = new Facility();
		facility.ID = -1;
		facility.Status = FacilityStatus.PreConstruction;
		Debug.Log("Set Status "+facility.Status);

		if (facility.Mode == FacilityMode.Launch) {
			facility.FundsRequired = Constant.CostPerLaunchFacility;
		} else {
			facility.FundsRequired = Constant.CostPerResearchLab;
		}

		Set(facility);
		abandonToggle.gameObject.SetActive(false);
	}

	public void Set(Facility facility) {
		this.facility = facility;

		if (facility.Status == FacilityStatus.PreConstruction) {
			title.text = "New Facility";
		} else {
			title.text = facility.Name;
		}

		ShowFunding(facility.FundsPerTurn);
		ShowPrice(facility.FundsRequired);

		if (facility.IsHQ && facility.Status != FacilityStatus.PreConstruction) {
			modeDropdown.gameObject.SetActive(true);
			modeDropdown.interactable = (facility.Status == FacilityStatus.NoProject);
		} else {
			modeDropdown.gameObject.SetActive(false);
		}

		if (facility.Status == FacilityStatus.NoProject) {
			abandonToggle.gameObject.SetActive(false);
		} else {
			abandonToggle.interactable = true;
			abandonToggle.gameObject.SetActive(true);
		}

		if (facility.Status == FacilityStatus.ActiveConstruction || facility.Status == FacilityStatus.PreConstruction) {
			typeDropdown.value = (int)facility.Mode;
			typeDropdown.RefreshShownValue();

			nameInput.text = facility.Name;
			latitudeInput.text = facility.Location.Latitude.ToString();
			longitudeInput.text = facility.Location.Longitude.ToString();

			if (facility.Status == FacilityStatus.ActiveConstruction) {
				typeDropdown.interactable = false;
				nameInput.interactable = false;
				latitudeInput.interactable = false;
				longitudeInput.interactable = false;
			} else if (facility.Status == FacilityStatus.PreConstruction) {
				typeDropdown.interactable = true;
				nameInput.interactable = true;
				latitudeInput.interactable = true;
				longitudeInput.interactable = true;
			}

			constructionControls.SetActive(true);
			launchControls.SetActive(false);
			researchControls.SetActive(false);
		} else {
			constructionControls.SetActive(false);
			ShowMode(facility.Mode);

			if (facility.Mode == FacilityMode.Launch) {
				if (facility.Status == FacilityStatus.ActiveProject) {
					blueprintsDropdown.interactable = false;
					altitudeSlider.interactable = false;
					inclinationSlider.interactable = false;
					rotationSlider.interactable = false;
				}

				if (facility.LaunchData.BlueprintID >= 0) {
					blueprintsDropdown.value = facility.LaunchData.BlueprintID;
				} else {
					blueprintsDropdown.value = 0;
				}

				if (GameController.Data.PlayerFaction.Blueprints.Has(facility.LaunchData.BlueprintID)) {
					ShowBlueprint(GameController.Data.PlayerFaction.Blueprints.Get(facility.LaunchData.BlueprintID).Data);
				}

				ShowAltitude(facility.LaunchData.Orbit.Altitude);
				ShowInclination(facility.LaunchData.Orbit.Inclination);
				ShowRotation(facility.LaunchData.Orbit.Rotation);

				blueprintsDropdown.RefreshShownValue();
				UpdateOrbitDisplay();
			}
		}
	}

	public void Confirm() {
		if (abandonToggle.isOn) {
			if (facility.Status == FacilityStatus.ActiveConstruction || facility.Status == FacilityStatus.PreConstruction) {
				GameController.Data.PlayerFaction.Facilities.Remove(facility.ID);
				abandonToggle.isOn = false;
				return;
			}

			abandonToggle.isOn = false;
		}

		GameController.Data.PlayerFaction.Facilities.Set(facility);
	}

	// -------

	public void SetMode(int modeValue) {
		FacilityMode mode = (FacilityMode)modeValue;
		facility.Mode = mode;

		if (mode == FacilityMode.Research) {
			facility.FundsRequired = Constant.CostPerTechPoint;
			ShowPrice(facility.FundsRequired);
		} else if (mode == FacilityMode.Launch) {
			if (facility.LaunchData.BlueprintID >= 0) {
				blueprintsDropdown.value = facility.LaunchData.BlueprintID;
			} else {
				blueprintsDropdown.value = 0;
			}

			UpdateOrbitDisplay();
		}

		ShowMode(mode);
	}

	public void SetFunding(float fundsValue) {
		int funds = (int)fundsValue;
		facility.FundsPerTurn = funds;
		ShowFunding(funds);
	}

	public void SetBlueprint(int blueprintIndex) {
		int id = blueprintIDs[blueprintIndex];
		facility.LaunchData.BlueprintID = id;
		SatelliteData blueprintData = GameController.Data.PlayerFaction.Blueprints.Get(id).Data;

		Debug.Log("Set Blueprint: "+blueprintIndex+", with weight: "+blueprintData.TotalWeight);
		facility.FundsRequired = blueprintData.TotalWeight * Constant.CostPerWeight;

		ShowPrice(facility.FundsRequired);
		ShowBlueprint(blueprintData);
	}

	public void SetAltitude(float altitudeValue) {
		int altitude = (int)altitudeValue;
		facility.LaunchData.Orbit.Altitude = altitude;
		ShowAltitude(altitude);
		UpdateOrbitDisplay();
	}

	public void SetInclination(float inclinationValue) {
		int inclination = (int)inclinationValue;
		facility.LaunchData.Orbit.Inclination = inclination;
		ShowInclination(inclination);
		UpdateOrbitDisplay();
	}

	public void SetRotation(float rotationValue) {
		int rotation = (int)rotationValue;
		facility.LaunchData.Orbit.Rotation = rotation;
		ShowRotation(rotation);
		UpdateOrbitDisplay();
	}

	public void SetName(string name) {
		facility.Name = name;
	}

	public void SetType(int typeValue) {
		facility.Mode = (FacilityMode) typeValue;

		if (facility.Mode == FacilityMode.Launch) {
			facility.FundsRequired = Constant.CostPerLaunchFacility;
		} else if (facility.Mode == FacilityMode.Research) {
			facility.FundsRequired = Constant.CostPerResearchLab;
		}

		ShowPrice(facility.FundsRequired);
	}

	public void SetLatitude(string latitudeValue) {
		int latitude = int.Parse(latitudeValue);
		facility.Location = new LatLong(latitude, facility.Location.Longitude);
	}

	public void SetLongitude(string longitudeValue) {
		int longitude = int.Parse(longitudeValue);
		facility.Location = new LatLong(facility.Location.Latitude, longitude);
	}

	public void SetLocation(LatLong location) {
		facility.Location = location;
	}

	public void SetAbandoned() {
		abandonToggle.interactable = false;

		modeDropdown.interactable = true;
		blueprintsDropdown.interactable = true;
		altitudeSlider.interactable = true;
		inclinationSlider.interactable = true;
		rotationSlider.interactable = true;

		if (facility.Status == FacilityStatus.ActiveProject) {
			facility.AbandonProject();
		}

		ShowFunding(facility.FundsPerTurn);
		ShowPrice(facility.FundsRequired);
		blueprintsDropdown.value = facility.LaunchData.BlueprintID;
		blueprintsDropdown.RefreshShownValue();
	}

	// -------

	private void ShowMode(FacilityMode mode) {
		if (facility.IsHQ) {
			modeDropdown.value = (int) mode;
			modeDropdown.RefreshShownValue();
		}

		launchControls.SetActive(mode == FacilityMode.Launch);
		researchControls.SetActive(mode == FacilityMode.Research);
	}

	private void ShowFunding(int funds) {
		fundingSlider.value = funds;
		fundingLabel.text = "Funding: ¥" + String.Format("{0:0.00}", funds / 100f);
		UpdateTurnsLeft();
	}

	private void ShowPrice(int price) {
		if (price > 0) {
			priceText.text = String.Format("{0:0.00}", price / 100f);
		} else {
			priceText.text = "-";
		}

		fundingSlider.maxValue = Mathf.Min(GameController.Data.PlayerFaction.Funds, price);
		UpdateTurnsLeft();
	}

	private void ShowAltitude(int altitude) {
		altitudeSlider.value = altitude;
		altitudeText.text = "Altitude: " + altitude + "km";
	}

	private void ShowInclination(int inclination) {
		inclinationSlider.value = inclination;
		inclinationText.text = "Inclination: " + inclination + "˚";
	}

	private void ShowRotation(int rotation) {
		rotationSlider.value = rotation;
		rotationText.text = "Rotation: " + rotation + "˚";
	}

	private void ShowBlueprint(SatelliteData blueprintData) {
		weightText.text = blueprintData.TotalWeight.ToString();
	}

	private void UpdateTurnsLeft() {
		if (facility.FundsRequired > 0) {
			int turns = facility.TurnsLeft;

			if (turns > 999) {
				turnsText.text = "999 Turns";
			} else if (turns < 0) {
				turnsText.text = "Never";
			} else {
				turnsText.text = turns + " Turns";
			}
		} else {
			turnsText.text = "-";
		}
	}

	public void UpdateOrbitDisplay() {
		facility.LaunchData.Orbit.RecalculateCoverage();

		Material material = mapRenderer.material;
		material.SetInt("_Orbit_Defined", 1);
		material.SetVectorArray("_Orbit", facility.LaunchData.Orbit.GetCoverage());
	}
}

