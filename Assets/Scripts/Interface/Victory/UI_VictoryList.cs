using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_VictoryList : UI_List<Faction> {

	void OnEnable() {
		SetData(GameController.Data.Factions);
	}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_FacilityList))]
	public class Editor : ListEditor {}
	#endif
}
