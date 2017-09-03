using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_TopBar : MonoBehaviour {

	public GameObject mainMenuContainer;

	[Header("UI Elements")]
	public Text yearDisplay;
	public Text turnsDisplay;
	public Text fundsDisplay;
	public Text techPointsDisplay;

	private readonly string[] seasons = new string[] { "Spring", "Summer", "Fall", "Winter" };

	void Awake() {
		On.GameLoad.Do(UpdateDisplay);
		On.AfterTurnAdvance.Do(UpdateTurnsDisplay);
		On.UpdateTechPoints.Do(UpdateTechPointsDisplay);
		On.UpdateFunds.Do(UpdateFundsDisplay);
	}

	void Update() {
		if (Input.GetKeyDown("escape")) {
			mainMenuContainer.SetActive(!mainMenuContainer.activeSelf);
		}
	}

	void UpdateDisplay() {
		UpdateTurnsDisplay();
		UpdateTechPointsDisplay();
		UpdateFundsDisplay();
	}

	void UpdateTechPointsDisplay() {
		techPointsDisplay.text = GameController.Data.PlayerFaction.TechPoints.ToString();
	}

	void UpdateFundsDisplay() {
		fundsDisplay.text = String.Format("{0:0.00}", GameController.Data.PlayerFaction.Funds / 100f);
	}

	void UpdateTurnsDisplay() {
		int turn = GameController.Data.CurrentTurn;
		int year = (int) (turn / 4) + 1;
		int quarter = turn % 4;

		yearDisplay.text = seasons[quarter] + ", Year " + year;
		turnsDisplay.text = turn.ToString();
	}

}

