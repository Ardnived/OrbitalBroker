using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class UI_ModuleList : UI_List<ModuleDesign> {

	public EventHandler OnSelect;

	void Start() {
		On.UpdateModuleDesigns.Do(SetDataHandler);

		// TODO: Instead of calling SetDataHandler here, we should be triggering UpdateModuleDesigns during deserialization.
		SetDataHandler();
	}

	private void SetDataHandler() {
		ModuleDesign[] designs = GameController.ModuleDesigns;
		List<int> availableDesignIDs = GameController.Data.PlayerFaction.AvailableModuleIDs;

		Dictionary<string, List<ModuleDesign>> data = new Dictionary<string, List<ModuleDesign>>() {
			{ "Solar Arrays", new List<ModuleDesign>() },
			{ "Research Modules", new List<ModuleDesign>() },
			{ "Broadcast Modules", new List<ModuleDesign>() },
			{ "Sensor Modules", new List<ModuleDesign>() },
			{ "Energy Modules", new List<ModuleDesign>() },
			{ "Special Modules", new List<ModuleDesign>() }
		};

		for (int i = 1; i < availableDesignIDs.Count; i++) {
			ModuleDesign design = designs[availableDesignIDs[i]];
			string sectionName = "Special Modules";

			switch (design.Type) {
			case ModuleType.Solar:
				sectionName = "Solar Arrays";
				break;
			case ModuleType.Research:
				sectionName = "Research Modules";
				break;
			case ModuleType.Broadcast:
				sectionName = "Broadcast Modules";
				break;
			case ModuleType.Sensor:
				sectionName = "Sensor Modules";
				break;
			case ModuleType.Energy:
				sectionName = "Energy Modules";
				break;
			}

			data[sectionName].Add(design);
		}

		Dictionary<string, List<ModuleDesign>> result = new Dictionary<string, List<ModuleDesign>>();

		foreach (KeyValuePair<string, List<ModuleDesign>> pair in data) {
			if (pair.Value.Count > 0) {
				result.Add(pair.Key, pair.Value);
			}
		}

		SetData(data);
	}

	protected override UnityEvent<ModuleDesign> GetSelectionHandler() {
		return OnSelect;
	}

	[System.Serializable]
	public class EventHandler : UnityEvent<ModuleDesign> {}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_ModuleList))]
	public class Editor : ListEditor {}
	#endif
}
