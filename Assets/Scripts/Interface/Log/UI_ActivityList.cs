using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UI_ActivityList : UI_List<Activity> {

	public Gradient progressGradient;
	public Color expiredIconColor;
	[HideInInspector]
	public Sprite[] icons;

	void Start() {
		//On.Activities.Listen(this.SetData);
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(UI_ActivityList))]
	public class Editor : ListEditor {

		SerializedProperty icons;

		protected void OnEnable() {
			icons = serializedObject.FindProperty("icons");
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			EditorGUILayout.LabelField("Icons", EditorStyles.boldLabel);

			string[] enumNames = System.Enum.GetNames(typeof(ActivityType));
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