using UnityEngine;
using System.Collections.Generic;

public class ModuleLink : MonoBehaviour {

	private SatelliteController controller;
	private int moduleIndex = -1;

	[SerializeField]
	private Anchor rootAnchor;
	private Anchor parentAnchor;
	private Dictionary<string, Anchor> anchors = new Dictionary<string, Anchor>();

	void Awake() {
		foreach (Transform child in this.transform) {
			Anchor anchor = child.GetComponent<Anchor>();

			if (anchor != null) {
				anchors.Add(anchor.ID, anchor);
			}
		}
	}

	public void SetController(SatelliteController controller) {
		this.controller = controller;
	}

	public void SetModuleIndex(int index) {
		this.moduleIndex = index;
	}

	public Anchor GetAnchor(string ID) {
		return anchors[ID];
	}

	public ModuleData GetData() {
		Debug.Log("Getting module data with index: "+moduleIndex);
		return controller.Data.Modules[moduleIndex];
	}

	void OnMouseEnter() {
		if (moduleIndex >= 0) {
			controller.OnModuleHover(moduleIndex, this, true);
		}
	}

	void OnMouseExit() {
		if (moduleIndex >= 0) {
			controller.OnModuleHover(moduleIndex, this, false);
		}
	}

	void OnMouseDown() {
		Debug.Log("On Mouse Down: "+this.ToString());
		// Trigger the anchor selection for the current parent anchor.
		if (moduleIndex >= 0) {
			Debug.Log("Sent Module Selected");
			controller.OnModuleSelected(moduleIndex, this);
		} else if (this.parentAnchor != null) {
			Debug.Log("Sent Anchor Selected");
			controller.OnAnchorSelected(this.parentAnchor);
		}
	}

	public void OnAnchorHover(Anchor anchor) {
		controller.OnAnchorHover(anchor);
	}

	public void OnAnchorSelected(Anchor anchor) {
		//Debug.Log("On Anchor Selected: "+this.ToString());
		//controller.OnAnchorSelected(anchor);
	}

	public void SetParentAnchor(Anchor anchor) {
		this.parentAnchor = anchor;

		if (parentAnchor != null) {
			parentAnchor.Child = null;
			parentAnchor = null;
		}

		if (anchor != null) {
			parentAnchor = anchor;
			parentAnchor.Child = this;

			this.transform.SetParent(anchor.transform, false);
			this.transform.localRotation = Quaternion.identity; // this.Root.transform.localRotation;
			this.transform.localPosition = -this.rootAnchor.transform.localPosition;
			this.gameObject.SetActive(true);
		} else {
			this.transform.SetParent(null);
			this.gameObject.SetActive(false);
		}
	}

	public void EnableAnchors() {
		foreach (Anchor anchor in anchors.Values) {
			if (anchor != this.rootAnchor || this.parentAnchor == null) {
				anchor.gameObject.SetActive(true);
			} else {
				anchor.gameObject.SetActive(false);
			}
		}
	}

}
