using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SatelliteManager : MonoBehaviour {

	public static SatelliteManager Instance { get; private set; }

	public SatelliteController satellitePrefab;
	public Transform satelliteContainer;
	public Transform blueprintContainer;
	public ModuleLink[] moduleModels;

	void Awake() {
		Instance = this;
	}

	public SatelliteController CreateSatellite(SatelliteController blueprint = null, SatelliteData data = null) {
		SatelliteController satellite;

		if (blueprint == null) {
			satellite = Instantiate(satellitePrefab);
		} else {
			satellite = Instantiate(blueprint);
			satellite.Data.CalculateProperties(); // TODO: Find a better place for this call
		}

		if (data != null) {
			LoadSatelliteData(satellite, data);
			data.CalculateProperties(); // TODO: Find a better place for this call
		}

		// TODO: Figure out if this is the best place and most efficient method to do this.
		satellite.GetComponentInChildren<TrailRenderer>().enabled = true;
		satellite.enabled = true;
		// ----

		satellite.transform.SetParent(satelliteContainer, false);
		satellite.gameObject.SetActive(true);
		return satellite;
	}

	public SatelliteController CreateBlueprint(SatelliteData data = null) {
		SatelliteController blueprint = Instantiate(satellitePrefab);
		blueprint.transform.SetParent(blueprintContainer, false);
		blueprint.gameObject.SetActive(false);

		if (data != null) {
			LoadSatelliteData(blueprint, data);
			data.CalculateProperties(); // TODO: Find a better place for this call
		}

		return blueprint;
	}

	public ModuleLink CreateModule(ModuleModel moduleModel) {
		ModuleLink moduleTemplate = moduleModels[(int)moduleModel];
		ModuleLink module = Instantiate(moduleTemplate);
		return module;
	}

	public void Clear() {
		Debug.Log("Deleting all satellite and blueprint objects");

		for (int i = satelliteContainer.childCount - 1; i >= 0; i--) {
			Destroy(satelliteContainer.GetChild(i).gameObject);
		}

		for (int i = blueprintContainer.childCount - 1; i >= 0; i--) {
			Destroy(blueprintContainer.GetChild(i).gameObject);
		}
	}

	private void LoadSatelliteData(SatelliteController controller, SatelliteData data) {
		controller.Data = data;

		#if UNITY_EDITOR
		controller.name = data.Name;
		#endif

		if (data.Modules.Count > 0) {
			ModuleLink coreModule = LoadModuleData(data.Modules[0], controller);
			coreModule.SetModuleIndex(0);
			coreModule.transform.SetParent(controller.transform, false);
		}
	}

	private ModuleLink LoadModuleData(ModuleData data, SatelliteController controller) {
		ModuleLink link = CreateModule(data.GetDesign().Model);
		link.SetController(controller);

		// TODO: In the future we should just keep anchors always enabled, and selectively control the click events instead. Or only disable the Anchor script, and not the anchor's gameobject
		link.EnableAnchors();
		// -------

		foreach (ModuleData.Child child in data.Children) {
			Anchor anchor = link.GetAnchor(child.AnchorID);
			ModuleData childData = controller.Data.Modules[child.ModuleIndex];
			ModuleLink childLink = LoadModuleData(childData, controller);
			childLink.SetModuleIndex(child.ModuleIndex);
			childLink.SetParentAnchor(anchor);
		}

		return link;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SatelliteManager))]
public class SatelliteManagerEditor : Editor {

	SerializedProperty satellitePrefab;
	SerializedProperty satelliteContainer;
	SerializedProperty blueprintContainer;
	SerializedProperty moduleModels;

	void OnEnable() {
		satellitePrefab = serializedObject.FindProperty("satellitePrefab");
		satelliteContainer = serializedObject.FindProperty("satelliteContainer");
		blueprintContainer = serializedObject.FindProperty("blueprintContainer");
		moduleModels = serializedObject.FindProperty("moduleModels");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
		EditorGUILayout.PropertyField(satellitePrefab);
		EditorGUILayout.PropertyField(satelliteContainer);
		EditorGUILayout.PropertyField(blueprintContainer);

		EditorGUILayout.LabelField("Models", EditorStyles.boldLabel);

		string[] enumNames = System.Enum.GetNames(typeof(ModuleModel));
		moduleModels.arraySize = enumNames.Length;

		int i = 0;
		foreach (SerializedProperty child in moduleModels) {
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
