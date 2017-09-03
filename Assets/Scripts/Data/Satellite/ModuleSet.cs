using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

[CreateAssetMenu]
public class ModuleSet : ScriptableObject {

	public ModuleDesign[] ModuleDesigns;

	#if UNITY_EDITOR
	[CustomEditor(typeof(ModuleSet))]
	public class ModuleSetEditor : Editor {

		SerializedProperty modules;

		protected void OnEnable() {
			modules = serializedObject.FindProperty("ModuleDesigns");
		}

		public override void OnInspectorGUI() {
			ModuleSet moduleSet = (ModuleSet)this.target;
			bool reindex = false;

			EditorGUILayout.LabelField("Module Designs", EditorStyles.boldLabel);

			int n = 0;
			foreach (SerializedProperty child in modules) {
				ModuleDesign moduleDesign = moduleSet.ModuleDesigns[n];
				EditorGUILayout.PropertyField(child, new GUIContent("[" + moduleDesign.ID + "] " + moduleDesign.Name + " - " + moduleDesign.Type + " " + moduleDesign.Capacity + "/" + moduleDesign.Weight + "/" + moduleDesign.EnergyConsumption + (moduleDesign.RequiresAstronaut ? "*" : "") + " (" + moduleDesign.Model + ")"), false);

				if (n >= moduleSet.ModuleDesigns.Length) {
					continue;
				}

				if (child.isExpanded) {
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(child.FindPropertyRelative("ID"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Name"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Type"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Capacity"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Weight"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("EnergyConsumption"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("RequiresAstronaut"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Model"));

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();

					if (GUILayout.Button("Duplicate")) {
						child.DuplicateCommand();
						serializedObject.ApplyModifiedProperties();
						reindex = true;
					}

					if (GUILayout.Button("Remove")) {
						child.DeleteCommand();
						serializedObject.ApplyModifiedProperties();
						reindex = true;
					}

					GUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
				}

				n++;
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add")) {
				modules.InsertArrayElementAtIndex(modules.arraySize);
				reindex = true;
				serializedObject.ApplyModifiedProperties();
			}

			if (GUILayout.Button("Clear")) {
				modules.ClearArray();
				reindex = true;
				serializedObject.ApplyModifiedProperties();
			}

			if (GUILayout.Button("Reindex") || reindex) {
				for (int i = 0; i < moduleSet.ModuleDesigns.Length; i++) {
					ModuleDesign moduleDesign = moduleSet.ModuleDesigns[i];
					moduleDesign.ID = i;
					moduleSet.ModuleDesigns[i] = moduleDesign;
				}

				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}
			GUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}

	}
	#endif

}
