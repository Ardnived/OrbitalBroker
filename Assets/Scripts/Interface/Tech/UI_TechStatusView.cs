using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_TechStatusView : MonoBehaviour {

	public UI_TechWeb techWeb;

	[Header("UI Elements")]
	public Text title;
	public Text description;
	public GameObject acceptButton;
	public GameObject actionButtons;

	private int techID;
	private int factionID;

	public void Set(TechData techData, int factionID) {
		string techName = techData.Name.ToUpper();
		title.text = techName;

		GlobalTechState techState = GameController.Data.TechStatus[techData.ID];

		if (techState.Status == GlobalTechState.TechStatus.Secret) {
			description.text = "You will soon complete development of " + techName + ". What will you do with this technology?";
			acceptButton.SetActive(false);
			actionButtons.SetActive(true);
		} else {
			acceptButton.SetActive(true);
			actionButtons.SetActive(false);
		}

		if (techState.Status == GlobalTechState.TechStatus.PublicDomain) {
			description.text = "You will soon complete development of " + techName + ".\r\nThis technology has already been placed in the public domain.";
		} else if (techState.Status == GlobalTechState.TechStatus.Copyright) {
			description.text = "You will soon complete development of " + techName + ".\r\nThis technology has already been copyrighted by " + GameController.Data.Factions[techState.OwnerID].Name + ".";
		}

		this.techID = techData.ID;
		this.factionID = factionID;
	}

	public void ChoosePublish() {
		GlobalTechState techState = GameController.Data.TechStatus[techID];
		techState.Status = GlobalTechState.TechStatus.PublicDomain;
		techState.OwnerID = factionID;

		GameController.Data.TechStatus[techID] = techState;
		//techWeb.SetTargetTechStatus(techID, Constant.TechStatus.PublicDomain);
	}

	public void ChooseCopyright() {
		GlobalTechState techState = GameController.Data.TechStatus[techID];
		techState.Status = GlobalTechState.TechStatus.Copyright;
		techState.OwnerID = factionID;

		GameController.Data.TechStatus[techID] = techState;
		//techWeb.SetTargetTechStatus(techID, factionID);
	}

	public void ChooseSecret() {
		//techWeb.SetTargetTechStatus(techID, Constant.TechStatus.Secret);
	}

}

