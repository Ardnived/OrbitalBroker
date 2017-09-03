using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour {

	public static ViewManager Instance { get; private set; }

	[Header("Cameras")]
	[SerializeField]
	private Camera techCamera;
	[SerializeField]
	private ConstrainedOrbitCamera satelliteCamera;

	[Header("Views")]
	[SerializeField]
	private UI_SatelliteView satelliteView;
	[SerializeField]
	private UI_FactionView factionView;
	[SerializeField]
	private UI_FacilityView facilityView;
	[SerializeField]
	private UI_EndGameView endGameView;

	void Awake() {
		Instance = this;
	}

	public void Show(Camera camera) {
		Camera.main.gameObject.SetActive(false);
		camera.gameObject.SetActive(true);
	}

	public void Show(SatelliteController satellite) {
		satelliteCamera.target = satellite.transform;
		satelliteView.Set(satellite.Data);
		Show(satelliteCamera.GetComponent<Camera>());
	}

	public void Show(Faction faction) {
		factionView.Set(faction);
		factionView.gameObject.SetActive(true);
	}

	public void Show(Facility facility) {
		facilityView.Set(facility);
		facilityView.gameObject.SetActive(true);
	}

	public void ShowDiplomaticVictory(Faction faction) {
		endGameView.ShowDiplomaticVictory(faction);
		endGameView.gameObject.SetActive(true);
	}

	public void ShowEconomicVictory(Faction faction) {
		endGameView.ShowEconomicVictory(faction);
		endGameView.gameObject.SetActive(true);
	}

	public void ShowScientificVictory(Faction faction) {
		endGameView.ShowScientificVictory(faction);
		endGameView.gameObject.SetActive(true);
	}


	public void ShowTechWeb() {
		Show(techCamera);
	}

}

