using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class UI_BlueprintList : UI_SatelliteList {

	void Start() {
		On.UpdateBlueprints.Do(SetDataHandler);
	}

	void SetDataHandler() {
		SetData(GameController.Data.PlayerFaction.Blueprints);
	}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_BlueprintList))]
	public new class Editor : UI_SatelliteList.Editor {}
	#endif
}