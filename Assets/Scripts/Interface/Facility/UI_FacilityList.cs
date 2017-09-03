using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_FacilityList : UI_List<Facility> {

	public EventHandler OnSelect;
	public Sprite constructionIcon;
	[HideInInspector]
	public Sprite[] icons;

	void Start() {
		On.UpdateFacilities.Do(SetDataHandler);
	}

	void SetDataHandler() {
		SetData(GameController.Data.PlayerFaction.Facilities);
	}

	/*
	private void SetDataHander(DataCollection<Facility> data) {
		Dictionary<string, List<Facility>> result = new Dictionary<string, List<Facility>>();

		foreach (Facility facility in data) {
			if (facility.IsHQ) {
				result.Add("", new List<Facility>() { facility });
			} else {
				string sectionName = "";

				switch (facility.Mode) {
				case FacilityMode.LaunchFacility:
					sectionName = "Launch Facilities";
					break;
				case FacilityMode.ResearchLab:
					sectionName = "Research Labs";
					break;
				}

				if (!result.ContainsKey(sectionName)) {
					result.Add(sectionName, new List<Facility>());
				}

				result[sectionName].Add(facility);
			}
		}

		SetData(result);
	}
	*/

	protected override UnityEvent<Facility> GetSelectionHandler() {
		return OnSelect;
	}

	[System.Serializable]
	public class EventHandler : UnityEvent<Facility> {}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(UI_FacilityList))]
	public class Editor : ListEditor {

		SerializedProperty icons;

		protected void OnEnable() {
			icons = serializedObject.FindProperty("icons");
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			EditorGUILayout.LabelField("Icons", EditorStyles.boldLabel);

			string[] enumNames = System.Enum.GetNames(typeof(FacilityMode));
			icons.arraySize = enumNames.Length;

			int i = 0;
			foreach (SerializedProperty child in icons) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(enumNames[i]);
				EditorGUILayout.PropertyField(child, GUIContent.none);
				EditorGUILayout.EndHorizontal();
				i++;
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
	#endif
}
