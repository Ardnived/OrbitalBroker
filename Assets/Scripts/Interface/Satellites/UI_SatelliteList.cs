using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class UI_SatelliteList : UI_List<SatelliteController> {

	public EventHandler OnSelect;

	void Start() {
		On.UpdateSatellites.Do(SetDataHandler);
	}

	void SetDataHandler() {
		SetData(GameController.Data.PlayerFaction.Satellites);
	}

	protected override UnityEvent<SatelliteController> GetSelectionHandler() {
		return OnSelect;
	}

	[System.Serializable]
	public class EventHandler : UnityEvent<SatelliteController> {}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_SatelliteList))]
	public class Editor : ListEditor {}
	#endif
}
