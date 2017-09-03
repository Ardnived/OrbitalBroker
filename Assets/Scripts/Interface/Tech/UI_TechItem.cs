using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_TechItem : MonoBehaviour {

	private readonly Color disabledLinesColor = new Color(1, 1, 1, 0.3f);
	private readonly Color enabledLinesColor = new Color(1, 1, 1, 1);
	private readonly Color completeFacetColor = Color.green;
	private readonly Color incompleteFacetColor = Color.white;

	public int TechID;
	public Image[] lines;

	[Header("UI Elements")]
	public Toggle toggle;
	public UI_TooltipController tooltip;
	public UI_TooltipController popup;
	public Image progressBackground;
	public Image progressFill;
	public GameObject statusContainer;
	public Image statusIcon;

	#if UNITY_EDITOR
	public Image icon;
	public Image iconFill;
	#endif

	public UI_TechWeb master;
	private bool showingComplete = false;

	public void Start() {
		TechData tech = master.techWeb.Techs[TechID];
		tooltip.Text = tech.Name;
		popup.Text = tech.Description;
	}

	public void Set(Faction faction) {
		TechData tech = master.techWeb.Techs[TechID];
		TechDifficulty difficulty = master.techWeb.GetDifficulty(tech, faction);
		int researchLevel = faction.ResearchLevels[TechID];
		GlobalTechState.TechStatus techStatus = GameController.Data.TechStatus[TechID].Status;

		SetStatusIcon(techStatus, researchLevel);
		SetProgress(researchLevel, difficulty);
		UpdateInteractable(faction);
	}

	public void UpdateInteractable(Faction faction) {
		// TODO: We probably shouldn't be recomputing this again..
		int researchLevel = faction.ResearchLevels[TechID];

		if (faction.ID == Constant.PlayerFactionID && (faction.TechPoints > 0 || toggle.isOn) && researchLevel != Constant.TechStatus.Researched) {
			GlobalTechState.TechStatus techStatus = GameController.Data.TechStatus[TechID].Status;

			if (techStatus != GlobalTechState.TechStatus.Copyright || faction.Funds >= Constant.CostPerCopyrightPoint) {
				toggle.interactable = true;
				return;
			}
		}

		toggle.interactable = false;
	}

	private void SetStatusIcon(GlobalTechState.TechStatus techStatus, int researchLevel) {
		switch (techStatus) {
		case GlobalTechState.TechStatus.Secret:
			statusIcon.sprite = master.secretIcon;
			break;
		case GlobalTechState.TechStatus.PublicDomain:
			statusIcon.sprite = master.publicDomainIcon;
			break;
		case GlobalTechState.TechStatus.Copyright:
			statusIcon.sprite = master.patentIcon;
			break;
		}

		if (techStatus == GlobalTechState.TechStatus.Secret && researchLevel != Constant.TechStatus.Researched) {
			statusIcon.gameObject.SetActive(false);
		} else {
			statusIcon.gameObject.SetActive(true);
		}
	}

	private void SetProgress(int researchLevel, TechDifficulty difficulty) {
		if (researchLevel == Constant.TechStatus.Researched) {
			progressBackground.sprite = master.completeIcon;

			if (!showingComplete) {
				progressBackground.color = completeFacetColor;
				progressFill.gameObject.SetActive(false);

				for (int i = 0; i < lines.Length; i++) {
					lines[i].color = enabledLinesColor;
				}

				showingComplete = true;
			}
		} else {
			progressBackground.sprite = difficulty.ProgressIcon;
			progressFill.sprite = difficulty.ProgressIcon;
			progressFill.fillAmount = (float)researchLevel / difficulty.PointsRequired;

			if (showingComplete) {
				progressBackground.color = incompleteFacetColor;
				progressFill.gameObject.SetActive(true);

				for (int i = 0; i < lines.Length; i++) {
					lines[i].color = disabledLinesColor;
				}

				showingComplete = false;
			}
		}
	}

	public void OnSelectChanged(bool selected) {
		master.SelectTech(this.TechID, selected);
	}

}

