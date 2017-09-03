using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class UI_FileList : UI_List<string> {

	public EventHandler OnSelect;

	void Start() {
		string[] fileNames = new string[] { "Test", "Test1", "Test2" };
		SetData(fileNames);
	}

	protected override UnityEvent<string> GetSelectionHandler() {
		return OnSelect;
	}

	[System.Serializable]
	public class EventHandler : UnityEvent<string> {}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_FileList))]
	public class Editor : ListEditor {}
	#endif
}

