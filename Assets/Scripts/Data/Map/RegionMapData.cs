using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class RegionMapData : ScriptableObject {

	private static readonly int regionWealthCount = System.Enum.GetValues(typeof(RegionWealth)).Length;

	[SerializeField]
	private int dataScale = 2; // How much the data should be simplified from the lat/long level.

	[SerializeField]
	private float[] marketTendencies;

	[SerializeField]
	private RegionData[] regionData = new RegionData[1];

	public RegionData[] Regions {
		get { return regionData; }
	}

	[SerializeField]
	private MegaRegionData[] megaRegionData = new MegaRegionData[1];

	[HideInInspector]
	[SerializeField]
	private int[] regionIdGrid = new int[0];

	[HideInInspector]
	public int Width, Height;

	public float GetMarketTendency(RegionWealth wealth, MarketType type) {
		return marketTendencies[regionWealthCount * (int)type + (int)wealth];
	}

	public int GetRegionID(int x, int y) {
		int index = (y * Width) + x;
		return regionIdGrid[index];
	}

	public int GetRegionID(LatLong latlong) {
		int index = GetIndex(latlong);
		return regionIdGrid[index];
	}

	public RegionData GetRegionData(int x, int y) {
		int id = GetRegionID(x, y);
		return regionData[id];
	}

	public RegionData GetRegionData(LatLong latlong) {
		int id = GetRegionID(latlong);
		return regionData[id];
	}

	public RegionData GetRegionData(int countryID) {
		return regionData[countryID];
	}

	private int GetIndex(LatLong latlong) {		
		// Get these into the positive range, and scale by fidelity
		int latitude = (latlong.Latitude - Constant.MinLatitude) / dataScale;
		int longitude = (latlong.Longitude - Constant.MinLongitude) / dataScale;

		// Convert this 2d index into a 1d index.
		return (latitude * Width) + longitude;
	}

	#if UNITY_EDITOR
	private Dictionary<string, int> countryToRegionIdMap;
	private HashSet<string> unassignedRegions = new HashSet<string>();

	// TODO: Delete this
	public RegionData[] GetRegionData() {
		return regionData;
	}

	private void ImportData() {
		this.Width = (Constant.LongitudeCount / dataScale);
		this.Height = (Constant.LatitudeCount / dataScale) + 1;

		string[] lines = Regex.Split(sourceFile.text, "\n|\r|\r\n");
		regionIdGrid = new int[Width * Height];
		countryToRegionIdMap = new Dictionary<string, int>();

		for (int i = 0; i < regionData.Length; i++) {
			regionData[i].ID = i;
			string[] countryCodes = regionData[i].Countries.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

			for (int k = 0; k < countryCodes.Length; k++) {
				countryToRegionIdMap.Add(countryCodes[k], i);
			}
		}

		int latitude, longitude;
		string countryCode, name;

		for (int i = 0; i < lines.Length; i++) {
			EditorUtility.DisplayProgressBar("Importing Data File ("+lines.Length+" lines)", "Importing "+lines[i], (float)i / (float)lines.Length);

			string[] values = Regex.Split(lines[i], ",");

			if (values.Length < 4) {
				// If this line doesn't have enough fields, just skip it.
				continue;
			}

			latitude = int.Parse(values[0]);
			longitude = int.Parse(values[1]);
			countryCode = values[2];
			name = values[3];

			InsertData(latitude, longitude, countryCode, name);

			// ------------------------
			// Do this to make up for some deficiencies in our data file.
			if (latitude == -89) {
				InsertData(-90, longitude, countryCode, name);
			} else if (latitude == 89) {
				InsertData(90, longitude, countryCode, name);
			}

			if (longitude == -179) {
				InsertData(latitude, -180, countryCode, name);

				if (latitude == -89) {
					InsertData(-90, -180, countryCode, name);
				} else if (latitude == 89) {
					InsertData(90, -180, countryCode, name);
				}
			}
			// ------------------------
		}

		if (unassignedRegions.Count > 0) {
			string text = "Unassigned Regions: ";
			foreach (string txt in unassignedRegions) {
				text += txt + ", ";
			}

			Debug.LogWarning(text);
		}

		EditorUtility.ClearProgressBar();
	}

	private void InsertData(int latitude, int longitude, string countryCode, string name) {
		if ((latitude - Constant.MinLatitude) % dataScale != 0 || (longitude - Constant.MinLongitude) % dataScale != 0) {
			// Skip this coordinate if it is not relevant to our fidelity level.
			return;
		}

		int regionId = 0;

		if (countryToRegionIdMap.ContainsKey(countryCode)) {
			regionId = countryToRegionIdMap[countryCode];
		} else {
			unassignedRegions.Add(countryCode + " (" + name + ")");
			return;
		}

		int index = GetIndex(new LatLong(latitude, longitude));

		regionIdGrid[index] = regionId;

		RegionData data = regionData[regionId];
		data.CellCount++;
		regionData[regionId] = data;
	}

	public void GenerateRegionMapTexture() {
		Texture2D texture = new Texture2D(Width, Height, TextureFormat.Alpha8, false, true);
		Color[] pixels = new Color[Width * Height];

		for (int i = 0; i < regionIdGrid.Length; i++) {
			EditorUtility.DisplayProgressBar("Rendering Region Map Texture ("+Width+"x"+Height+")", "Writing Pixels", (float)i / regionIdGrid.Length);

			float v = 0;

			if (regionIdGrid[i] > 0) {
				v = regionIdGrid[i] / 256f;
			}

			pixels[i] = new Color(0, 0, 0, v);
		}

		texture.SetPixels(pixels);
		texture.Apply();

		string path = "/Graphics/Map/"+this.name+".png";
		EditorUtility.DisplayProgressBar("Rendering Region Map Texture ("+Width+"x"+Height+")", "Generating File: "+path, 1f);
		System.IO.File.WriteAllBytes(Application.dataPath + path, texture.EncodeToPNG());

		Debug.Log("Generated Map texture at " + path);
		EditorUtility.ClearProgressBar();
	}

	[SerializeField]
	private TextAsset sourceFile;

	[CustomEditor(typeof(RegionMapData))]
	public class RegionMapDataEditor : Editor {

		SerializedProperty regionData;
		SerializedProperty megaRegionData;

		SerializedProperty marketTendencies;

		SerializedProperty sourceFile;
		SerializedProperty dataScale;

		/*
		[SerializeField]
		private bool[] foldout = new bool[Constant.Market.TypeCount];
		*/

		protected void OnEnable() {
			regionData = serializedObject.FindProperty("regionData");
			megaRegionData = serializedObject.FindProperty("megaRegionData");
			sourceFile = serializedObject.FindProperty("sourceFile");
			dataScale = serializedObject.FindProperty("dataScale");
			marketTendencies = serializedObject.FindProperty("marketTendencies");
		}

		public override void OnInspectorGUI() {
			RegionMapData mapData = (RegionMapData)this.target;

			EditorGUILayout.LabelField("Import", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Data Size", mapData.Width+"x"+mapData.Height);
			EditorGUILayout.PropertyField(dataScale);
			EditorGUILayout.PropertyField(sourceFile);

			if (GUILayout.Button("Import Data")) {
				mapData.ImportData();
			}

			if (GUILayout.Button("Generate Texture")) {
				mapData.GenerateRegionMapTexture();
			}


			EditorGUILayout.LabelField("Market Tendencies", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(marketTendencies, true);

			int j = 0;
			foreach (SerializedProperty child in marketTendencies) {
				EditorGUILayout.PropertyField(child, new GUIContent((RegionWealth)(j / Constant.Market.TypeCount) + " - " + (MarketType)(j % Constant.Market.TypeCount)));
				j++;
			}


			/*
			bool changed = false;
			RegionWealth[] wealthLevels = (RegionWealth[]) System.Enum.GetValues(typeof(RegionWealth));
			MarketType[] demandTypes = (MarketType[]) System.Enum.GetValues(typeof(MarketType));

			if (mapData.marketTendencies == null || mapData.marketTendencies.Length != (wealthLevels.Length * demandTypes.Length)) {
				mapData.marketTendencies = new float[wealthLevels.Length * demandTypes.Length];

				changed = true;
				foldout = new bool[wealthLevels.Length];
			}

			foreach (RegionWealth wealth in wealthLevels) {
				foldout[(int)wealth] = EditorGUILayout.Foldout(foldout[(int)wealth], wealth.ToString() + " Wealth", true);

				if (foldout[(int)wealth]) {
					EditorGUI.indentLevel++;

					foreach (MarketType type in demandTypes) {
						float oldValue = mapData.GetMarketTendency(wealth, type);
						float newValue = EditorGUILayout.FloatField(type.ToString(), oldValue);

						if (newValue != oldValue) {
							changed = true;
							mapData.marketTendencies[RegionMapData.regionWealthCount * (int)type + (int)wealth] = newValue;
						}
					}

					EditorGUI.indentLevel--;
				}
			}

			if (changed) {
				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}
			*/

			EditorGUILayout.Space();


			EditorGUILayout.LabelField("Region Data", EditorStyles.boldLabel);

			int n = 0;
			foreach (SerializedProperty child in regionData) {
				RegionData region = mapData.regionData[n];
				EditorGUILayout.PropertyField(child, new GUIContent("[" + region.ID + "] " + region.Name + " (" + region.CellCount + ") - " + region.Wealth), false);

				if (child.isExpanded) {
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Name"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Wealth"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Countries"));

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();

					if (GUILayout.Button("Duplicate")) {
						child.DuplicateCommand();
					}

					if (GUILayout.Button("Remove")) {
						child.DeleteCommand();
					}

					GUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
				}

				n++;
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add")) {
				regionData.InsertArrayElementAtIndex(regionData.arraySize);
				serializedObject.ApplyModifiedProperties();
			}

			if (GUILayout.Button("Clear")) {
				regionData.ClearArray();
				serializedObject.ApplyModifiedProperties();
			}
			GUILayout.EndHorizontal();



			EditorGUILayout.LabelField("Mega Region Data", EditorStyles.boldLabel);

			n = 0;
			foreach (SerializedProperty child in megaRegionData) {
				MegaRegionData region = mapData.megaRegionData[n];
				EditorGUILayout.PropertyField(child, new GUIContent("[" + region.ID + "] " + region.Name));

				if (child.isExpanded) {
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(child.FindPropertyRelative("Name"));
					EditorGUILayout.PropertyField(child.FindPropertyRelative("RegionIds"), true);

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();

					if (GUILayout.Button("Duplicate")) {
						child.DuplicateCommand();
					}

					if (GUILayout.Button("Remove")) {
						child.DeleteCommand();
					}

					GUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
				}

				n++;
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add")) {
				megaRegionData.InsertArrayElementAtIndex(megaRegionData.arraySize);
				serializedObject.ApplyModifiedProperties();
			}

			if (GUILayout.Button("Clear")) {
				megaRegionData.ClearArray();
				serializedObject.ApplyModifiedProperties();
			}

			if (GUILayout.Button("Reindex")) {
				for (int i = 0; i < mapData.megaRegionData.Length; i++) {
					mapData.megaRegionData[i].ID = i;
				}

				serializedObject.Update();
			}
			GUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}

	}

	#endif

}

public enum RegionWealth {
	Low,
	Average,
	High,
}

[System.Serializable]
public struct RegionData {
	public string Name;
	public int ID;
	public float CellCount;
	public RegionWealth Wealth;

	#if UNITY_EDITOR
	public string Countries;
	#endif
}

[System.Serializable]
public struct MegaRegionData {
	public int ID;
	public string Name;
	public int[] RegionIds;
}
