using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_LogList : UI_List<Log> {

	void Start() {
		On.GameLoad.Do(SetDataHandler);
		On.AfterTurnAdvance.Do(SetDataHandler);
	}

	private void SetDataHandler() {
		SetData(GameController.Data.Logs);
	}

	protected override Vector2 GetOffsetY(float height) {
		float yMin = (itemCount * itemHeight) + (headerCount * headerHeight);
		float yMax = yMin + height;
		return new Vector2(yMin, yMax);
	}

}