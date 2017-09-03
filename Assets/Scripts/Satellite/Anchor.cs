using UnityEngine;
using System.Collections;

public class Anchor : MonoBehaviour {
	public string ID = null;
	public bool Root;
	public Anchor.Size size;
	public ModuleLink Parent;
	public ModuleLink Child;

	void Start() {
		if (ID == null) {
			Debug.LogError("Anchor does not have an ID");
		}
	}

	void OnMouseEnter() {
		if (this.Child == null) {
			this.Parent.OnAnchorHover(this);
		}
	}

	void OnMouseDown() {
		if (this.Child != null) {
			this.Parent.OnAnchorSelected(this);
		}
	}

	public enum Size {
		Small,
		Medium,
		Large
	}

	#if UNITY_EDITOR
	void OnDrawGizmos() {
		float radius = 0.05f + 0.02f * (int)this.size;

		if (this.Root) {
			Gizmos.color = Color.red;
		} else {
			Gizmos.color = Color.green;
		}

		Vector3 startPoint = transform.position;
		Vector3 endPoint = startPoint + (this.transform.localToWorldMatrix.MultiplyVector(Vector3.up * 0.5f));
		Gizmos.DrawLine(startPoint, endPoint);
		Gizmos.DrawSphere(endPoint, radius);
	}
	#endif
}

/*
public class Anchor : MonoBehaviour {
	public string ID = null;
	public bool Root;
	public Anchor.Size size;

	public Module Parent;
	public Module Child;

	void Start() {
		if (ID == null) {
			ID = this.transform.GetSiblingIndex().ToString();
		}
	}

	void OnMouseEnter() {
		if (this.Child == null) {
			this.Parent.OnAnchorHover(this);
		}
	}

	void OnMouseDown() {
		if (this.Child != null) {
			this.Parent.OnAnchorSelected(this);
		}
	}

	public enum Size {
		Small,
		Medium,
		Large
	}

	#if UNITY_EDITOR
	void OnDrawGizmos() {
		float radius = 0.05f + 0.02f * (int)this.size;

		if (this.Root) {
			Gizmos.color = Color.red;
		} else {
			Gizmos.color = Color.green;
		}

		Vector3 startPoint = transform.position;
		Vector3 endPoint = startPoint + (this.transform.localToWorldMatrix.MultiplyVector(Vector3.up * 0.5f));
		Gizmos.DrawLine(startPoint, endPoint);
		Gizmos.DrawSphere(endPoint, radius);
	}
	#endif
}
*/