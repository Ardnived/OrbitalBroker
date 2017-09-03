using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class UI_FactionList : UI_List<Faction> {
	
	public EventHandler OnSelect;

	void Start() {
		On.GameLoad.Do(SetDataHandler);
	}

	void SetDataHandler() {
		SetData(GameController.Data.Factions);
	}

	protected override UnityEvent<Faction> GetSelectionHandler() {
		return OnSelect;
	}

	[System.Serializable]
	public class EventHandler : UnityEvent<Faction> {}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_FactionList))]
	public class Editor : ListEditor {}
	#endif
}
