using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_TechWeb : MonoBehaviour {

	public TechWeb techWeb;
	public UI_TechStatusView techStatusView;
	public GameObject linePrefab;

	[Header("Display Parameters")]
	public float linePadding;
	public Sprite completeIcon;
	public Sprite[] facetIcons;

	public Sprite secretIcon;
	public Sprite patentIcon;
	public Sprite publicDomainIcon;

	[Header("UI Elements")]
	public Transform techItemsSourceContainer;
	public Transform techItemsContainer;
	public Transform linesContainer;

	[HideInInspector]
	[SerializeField]
	private UI_TechItem[] techItems;

	private bool processing = false;
	//private List<int> selectedTechIDs = new List<int>();
	//private Dictionary<int, int> targetTechStatus = new Dictionary<int, int>();
	private Faction faction;

	void Awake() {
		On.TurnAdvanceMain.Do(OnTurnAdvanceMain);
		On.AfterTurnAdvance.Do(AfterTurnAdvance);
		On.GameLoad.Do(ShowPlayerFaction);

		// Maybe there is a better way?
		Destroy(techItemsSourceContainer.gameObject);
	}

	void Start() {
		ShowPlayerFaction();
	}

	void ShowPlayerFaction() {
		Show(GameController.Data.PlayerFaction);
	}

	// TODO: Maybe make this public when we want to show other factions
	private void Show(Faction faction) {
		this.faction = faction;

		for (int i = 0; i < techItems.Length; i++) {
			techItems[i].Set(faction);
		}
	}

	public void SelectTech(int techID, bool selected) {
		if (!processing && faction.ID == Constant.PlayerFactionID) {
			if (selected) {
				faction.BuyTech(techID);

				/*
				selectedTechIDs.Add(techID);

				faction.TechPoints--;
				On.UpdateTechPoints.Trigger();

				if (GameController.Data.TechStatus[techID] >= Constant.TechStatus.MinCopyrightID) {
					faction.Funds -= Constant.CostPerCopyrightPoint;
					On.UpdateFunds.Trigger();
				}
				
				TechData tech = techWeb.Techs[techID];
				TechDifficulty difficulty = techWeb.GetDifficulty(tech, faction);

				if (faction.ResearchLevels[techID] + 1 >= difficulty.PointsRequired) {
					techStatusView.Set(tech, faction.ID);
					techStatusView.gameObject.SetActive(true);
				}
				*/

				if (faction.TechPoints == 0) {
					for (int i = 0; i < techItems.Length; i++) {
						techItems[i].UpdateInteractable(faction);
					}
				}
			} else {
				faction.BuyTech(techID, cancel: true);

				/*
				selectedTechIDs.Remove(techID);
				faction.TechPoints++;
				On.UpdateTechPoints.Trigger();

				if (GameController.Data.TechStatus[techID] >= Constant.TechStatus.MinCopyrightID) {
					faction.Funds += Constant.CostPerCopyrightPoint;
					On.UpdateFunds.Trigger();
				}
				*/

				if (faction.TechPoints == 1) {
					for (int i = 0; i < techItems.Length; i++) {
						techItems[i].UpdateInteractable(faction);
					}
				}
			}
		}
	}

	public void SetTargetTechStatus(int techID, int techStatus) {
		/*
		if (targetTechStatus.ContainsKey(techID)) {
			targetTechStatus[techID] = techStatus;
		} else {
			targetTechStatus.Add(techID, techStatus);
		}
		*/
	}

	void OnTurnAdvanceMain() {
		processing = true;

		/*
		for (int i = 0; i < selectedTechIDs.Count; i++) {
			int id = selectedTechIDs[i];
			GameController.Data.PlayerFaction.ResearchLevels[id] += 1;
			techItems[id].toggle.isOn = false;

			// TODO: This condition is a bit verbose
			if (GameController.Data.PlayerFaction.ResearchLevels[id] >= techWeb.GetDifficulty(techWeb.Techs[id], GameController.Data.PlayerFaction).PointsRequired) {
				GameController.Data.PlayerFaction.ResearchLevels[id] = Constant.TechStatus.Researched;
				techWeb.AwardTech(GameController.Data.PlayerFaction, id);

				if (targetTechStatus.ContainsKey(id)) {
					GameController.Data.TechStatus[id] = targetTechStatus[id];
				}

				techItems[id].Set(faction);
			}
		}
		*/

		//targetTechStatus.Clear();
		//selectedTechIDs.Clear();
		Show(faction);

		processing = false;
	}

	void AfterTurnAdvance() {
		if (faction.ID == Constant.PlayerFactionID && faction.TechPoints > 0) {
			for (int i = 0; i < techItems.Length; i++) {
				techItems[i].UpdateInteractable(faction);
			}
		}
	}

	#if UNITY_EDITOR
	private void Process() {
		Debug.Log("Processing Tech Web...");
		UI_TechItem[] unorderedTechItems = techItemsSourceContainer.GetComponentsInChildren<UI_TechItem>();
		techItems = new UI_TechItem[techWeb.Techs.Length];

		for (int i = techItemsContainer.childCount - 1; i >= 0; i--) {
			DestroyImmediate(techItemsContainer.GetChild(i).gameObject);
		}

		for (int i = 0; i < unorderedTechItems.Length; i++) {
			UI_TechItem techItem = Instantiate(unorderedTechItems[i], techItemsContainer, true);
			techItems[techItem.TechID] = techItem;
			techItem.master = this;
		}

		for (int i = linesContainer.childCount - 1; i >= 0; i--) {
			DestroyImmediate(linesContainer.GetChild(i).gameObject);
		}

		for (int id = 0; id < techItems.Length; id++) {
			UI_TechItem techItem = techItems[id];
			TechData tech = techWeb.Techs[id];

			if (techItem == null) {
				Debug.LogError("No UI_TechItem found for Tech #"+id);
				return;
			}

			techItem.name = "["+id+"] " + tech.Name;
			techItem.icon.sprite = tech.Icon;
			techItem.iconFill.sprite = tech.Icon;

			if (techItem.lines.Length != tech.Synergies.Length) {
				techItem.lines = new Image[tech.Synergies.Length];
			}

			for (int i = 0; i < tech.Synergies.Length; i++) {
				int synergizedTechID = tech.Synergies[i];

				if (synergizedTechID > id) { //&& synergizedTechID < techItems.Length) {
					UI_TechItem synergizedTechItem = techItems[synergizedTechID];
					GameObject line = Instantiate(linePrefab);
					RectTransform lineTransform = line.transform as RectTransform;

					Vector3 vector = synergizedTechItem.transform.position - techItem.transform.position;
					Vector3 position = techItem.transform.position + (vector.normalized * linePadding);
					Quaternion rotation = Quaternion.AngleAxis(Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg - 90, Vector3.forward);
					float distance = vector.magnitude - (2 * linePadding);

					line.name = "Line "+id+"-"+synergizedTechID;
					lineTransform.position = position;
					lineTransform.rotation = rotation;
					lineTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, distance);
					lineTransform.SetParent(linesContainer, true);

					TechData synergizedTech = techWeb.Techs[synergizedTechID];
					if (synergizedTechItem.lines.Length != synergizedTech.Synergies.Length) {
						synergizedTechItem.lines = new Image[synergizedTech.Synergies.Length];
					}

					for (int n = 0; n < techItem.lines.Length; n++) {
						if (techItem.lines[n] == null) {
							techItem.lines[n] = line.GetComponent<Image>();
							break;
						}
					}

					for (int n = 0; n < synergizedTechItem.lines.Length; n++) {
						if (synergizedTechItem.lines[n] == null) {
							synergizedTechItem.lines[n] = line.GetComponent<Image>();
							break;
						}
					}
				}
			}
		}
	}

	[CustomEditor(typeof(UI_TechWeb))]
	public class UI_TechWebEditor : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Process Web")) {
				UI_TechWeb techWeb = (UI_TechWeb)this.target;
				techWeb.Process();
			}
		}

	}
	#endif

}

