using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class TechWeb : ScriptableObject {

	public TechDifficulty[] DifficultyLevels;
	public int PointsPerExtraDifficultyLevel;
	public TechData[] Techs;

	public void AwardTech(Faction faction, int techID) {
		Techs[techID].AwardTo(faction);
	}

	public TechDifficulty GetDifficulty(TechData tech, Faction faction) {
		int difficulty = tech.GetAdjustedDifficulty(faction);
		int maxDifficulty = DifficultyLevels.Length - 1;

		if (difficulty > maxDifficulty) {
			TechDifficulty data = DifficultyLevels[maxDifficulty];
			data.PointsRequired += (difficulty - maxDifficulty) * PointsPerExtraDifficultyLevel;
			return data;
		} else {
			return DifficultyLevels[difficulty];
		}
	}


	#if UNITY_EDITOR
	[CustomEditor(typeof(TechWeb))]
	public class TechWebEditor : Editor {

		SerializedProperty techs;
		SerializedProperty difficultyLevels;
		SerializedProperty pointsPerExtraDifficultyLevel;

		protected void OnEnable() {
			techs = serializedObject.FindProperty("Techs");
			difficultyLevels = serializedObject.FindProperty("DifficultyLevels");
			pointsPerExtraDifficultyLevel = serializedObject.FindProperty("PointsPerExtraDifficultyLevel");
		}

		public override void OnInspectorGUI() {
			TechWeb techWeb = (TechWeb)this.target;
			bool reindex = false;

			EditorGUILayout.PropertyField(difficultyLevels, true);
			EditorGUILayout.PropertyField(pointsPerExtraDifficultyLevel);

			EditorGUILayout.LabelField("Techs", EditorStyles.boldLabel);

			int n = 0;
			foreach (SerializedProperty child in techs) {
				if (n < techWeb.Techs.Length) {
					TechData tech = techWeb.Techs[n];
					string text = "[" + tech.ID + "] " + tech.Name;

					if (tech.Synergies.Length > 0) {
						text += " : ";
						for (int j = 0; j < tech.Synergies.Length; j++) {
							if (j < tech.Synergies.Length - 1) {
								text += tech.Synergies[j] + ",";
							} else {
								text += tech.Synergies[j];
							}
						}
					}

					text += " -> ";

					if (tech.Reward.DesignID > 0 || tech.Reward.Flag > TechFlag.None) {
						if (tech.Reward.DesignID > 0) {
							text += tech.Reward.DesignID;
						}

						if (tech.Reward.Flag > TechFlag.None) {
							if (tech.Reward.DesignID > 0) {
								text += ", ";
							}

							text += tech.Reward.Flag;
						}
					} else {
						text += "[NO REWARD]";
					}

					GUIContent guiContent = new GUIContent(text);
					EditorGUILayout.PropertyField(child, guiContent, true);
				} else {
					EditorGUILayout.PropertyField(child, true);
				}

				if (child.isExpanded) {
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
				}
				n++;
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add")) {
				techs.InsertArrayElementAtIndex(techs.arraySize);
				reindex = true;
				serializedObject.ApplyModifiedProperties();
			}

			if (GUILayout.Button("Clear")) {
				techs.ClearArray();
				reindex = true;
				serializedObject.ApplyModifiedProperties();
			}

			if (GUILayout.Button("Reindex") || reindex) {
				for (int i = 0; i < techWeb.Techs.Length; i++) {
					techWeb.Techs[i].ID = i;
				}

				serializedObject.Update();
			}
			GUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}

	}
	#endif
}


[Serializable]
public struct TechDifficulty {
	public int PointsRequired;
	public Sprite ProgressIcon;

	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(TechDifficulty))]
	public class DifficultyLevelDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			Rect pointsRect = new Rect(position.x, position.y, 30, position.height);
			Rect iconRect = new Rect(position.x+30, position.y, position.width-30, position.height);

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(pointsRect, property.FindPropertyRelative("PointsRequired"), GUIContent.none);
			EditorGUI.PropertyField(iconRect, property.FindPropertyRelative("ProgressIcon"), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
	#endif
}