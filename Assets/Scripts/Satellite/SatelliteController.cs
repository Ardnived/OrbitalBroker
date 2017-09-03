using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SatelliteController : MonoBehaviour, OwnedDataElement {

	public SatelliteData Data;
	private SatelliteMouseHandler handler;

	public int OwnerID {
		get { return Data.OwnerID; }
		set { Data.OwnerID = value; }
	}

	public int ID {
		get { return Data.ID; }
		set { Data.ID = value; }
	}

	void Update() {
		// Update Orbital Position
		float currentAngle = Data.Orbit.CurrentAngle;
		float speed = Data.Orbit.GetAngularVelocity(currentAngle);

		currentAngle += speed * Time.deltaTime;// * GameController.Data.TimeScale;

		while (currentAngle > 360) {
			currentAngle -= 360;
		}

		this.transform.position = Data.Orbit.GetPositionAtAngle(currentAngle);
		Data.Orbit.CurrentAngle = currentAngle;

		// Update Stats
		//Data.Energy -= Data.EnergyConsumption * Time.deltaTime;
		//Data.Energy += Data.EnergyProduction * Time.deltaTime * Data.SunExposure / 100f;
		//Data.Energy = Mathf.Clamp(Data.Energy, 0, Data.EnergyStorage);
	}

	public void SetHandler(SatelliteMouseHandler handler) {
		this.handler = handler;
	}

	public void OnAnchorHover(Anchor anchor) {
		if (handler != null) {
			handler.OnAnchorHover(anchor);
		}
	}

	public void OnAnchorSelected(Anchor anchor) {
		if (handler != null) {
			handler.OnAnchorSelected(anchor);
		}
	}

	public void OnModuleSelected(int moduleIndex, ModuleLink link) {
		if (handler != null) {
			handler.OnModuleSelected(Data.Modules[moduleIndex], link);
		}
	}

	public void OnModuleHover(int moduleIndex, ModuleLink link, bool hover) {
		if (handler != null) {
			handler.OnModuleHover(Data.Modules[moduleIndex], link, hover);
		}
	}

	public void AddModuleData(ModuleLink link, ModuleData data, Anchor anchor = null) {
		int index = Data.AddModule(data);
		link.SetModuleIndex(index);

		if (anchor != null) {
			ModuleData parent = anchor.Parent.GetData();
			parent.Children.Add(new ModuleData.Child(anchor.ID, index));
		}
	}

	public void RemoveModule(ModuleLink link, int moduleIndex) {
		if (moduleIndex == 0) {
			ClearModules();
		} else {
			Data.RemoveModule(moduleIndex);
			Destroy(link);
		}
	}

	public void ClearModules() {
		Data.Modules.Clear();

		for (int i = this.transform.childCount - 1; i >= 0; i--) {
			Transform child = this.transform.GetChild(i);

			if (child.GetComponent<ModuleLink>() != null) {
				Destroy(child);
			}
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (this.enabled) {
			Gizmos.color = Color.yellow;
			const int resolution = 400;

			for (int i = 0; i < resolution; i++) {
				float angle = (float)i * 360f / (float)resolution;
				Vector3 start = Data.Orbit.GetPositionAtAngle(angle);
				angle = (float)(i+1) * 360f / (float)resolution;
				Vector3 end = Data.Orbit.GetPositionAtAngle(angle);

				Gizmos.DrawLine(start, end);
			}

			Gizmos.color = Color.white;
			Gizmos.DrawSphere(Data.Orbit.GetPositionAtAngle(Data.Orbit.CurrentAngle), 500f);
		}
	}

	[CustomEditor(typeof(SatelliteController))]
	public class SatelliteControllerEditor : Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			if (GUILayout.Button("Recalculate Properties")) {
				SatelliteController satellite = (SatelliteController)this.target;
				satellite.Data.CalculateProperties();
			}
		}

	}
	#endif

}

public interface SatelliteMouseHandler {
	void OnAnchorHover(Anchor anchor);
	void OnAnchorSelected(Anchor anchor);
	void OnModuleHover(ModuleData data, ModuleLink link, bool hover);
	void OnModuleSelected(ModuleData data, ModuleLink link);
}

