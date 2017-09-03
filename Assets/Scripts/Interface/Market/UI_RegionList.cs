using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class UI_RegionList : UI_List<RegionData> {

	public EventHandler OnSelect;

	void Start() {
		On.GameLoad.Do(ResetData);
		ResetData();
	}

	private void ResetData() {
		SetData(GameController.Map.Regions);
	}

	protected override UnityEvent<RegionData> GetSelectionHandler() {
		return OnSelect;
	}

	[System.Serializable]
	public class EventHandler : UnityEvent<RegionData> {}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_RegionList))]
	public class Editor : ListEditor {}
	#endif
}
