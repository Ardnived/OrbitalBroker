using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_LoadingSpinner : MonoBehaviour {

	private RectTransform rectTransform;

	void Awake() {
		rectTransform = this.transform as RectTransform;
	}

	void Update() {
		rectTransform.Rotate(0, 0, -270 * Time.deltaTime);
	}

}

