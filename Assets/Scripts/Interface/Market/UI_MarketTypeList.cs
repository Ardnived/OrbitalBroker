using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class UI_MarketTypeList : UI_List<MarketType> {

	public EventHandler OnSelect;

	void Start() {
		SetData((MarketType[]) System.Enum.GetValues(typeof(MarketType)));
	}

	protected override UnityEvent<MarketType> GetSelectionHandler() {
		return OnSelect;
	}

	[System.Serializable]
	public class EventHandler : UnityEvent<MarketType> {}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_MarketTypeList))]
	public class Editor : ListEditor {}
	#endif
}
