using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

[CreateAssetMenu]
public class FactionSet : ScriptableObject {

	public FactionDefinition[] Factions = new FactionDefinition[0];

	[Serializable]
	public struct FactionDefinition {
		public string Name;
	}

	#if UNITY_EDITOR
	[SerializeField]
	private RegionMapData mapData;

	[CustomEditor(typeof(FactionSet))]
	public class FactionSetEditor : Editor {

		SerializedProperty factions;
		SerializedProperty mapData;

		private string[] regionOptions;
		private RegionData[] regionData;

		private string[] marketOptions;

		protected void OnEnable() {
			factions = serializedObject.FindProperty("Factions");
			mapData = serializedObject.FindProperty("mapData");
		}

		public override void OnInspectorGUI() {
			FactionSet factionSet = (FactionSet)this.target;

			if (factionSet.mapData != null && regionData == null) {
				regionData = factionSet.mapData.GetRegionData();
				regionOptions = new string[regionData.Length];

				for (int i = 1; i < regionData.Length; i++) {
					regionOptions[i] = regionData[i].Name;
				}
			}

			EditorGUILayout.PropertyField(mapData);

			EditorGUILayout.LabelField("Factions", EditorStyles.boldLabel);

			int n = 0;
			foreach (SerializedProperty child in factions) {
				FactionDefinition faction = factionSet.Factions[n];
				EditorGUILayout.PropertyField(child, new GUIContent(faction.Name), false);

				if (n >= factionSet.Factions.Length) {
					continue;
				}

				if (child.isExpanded) {
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Name"));

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();

					if (GUILayout.Button("Duplicate")) {
						child.DuplicateCommand();
						serializedObject.ApplyModifiedProperties();
					}

					if (GUILayout.Button("Remove")) {
						child.DeleteCommand();
						serializedObject.ApplyModifiedProperties();
					}

					GUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
				}

				n++;
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add")) {
				factions.InsertArrayElementAtIndex(factions.arraySize);
				serializedObject.ApplyModifiedProperties();
			}

			if (GUILayout.Button("Clear")) {
				factions.ClearArray();
				serializedObject.ApplyModifiedProperties();
			}
			GUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}

	}
	#endif

}
