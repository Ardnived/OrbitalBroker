using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tooltip : MonoBehaviour {

	public static List<UI_Tooltip> Instances = new List<UI_Tooltip>();

	public Text textBox;

	private RectTransform rectTransform;

	void Awake() {
		Instances.Add(this);
		rectTransform = this.transform as RectTransform;
		Hide();
	}

	public void Show(Rect rect, string text) {
		rectTransform.position = new Vector3(rect.position.x, rect.position.y);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
		textBox.text = text;
		this.gameObject.SetActive(true);
	}

	public void Hide() {
		this.gameObject.SetActive(false);
	}

	public void SetAlignment(TextAnchor alignment) {
		textBox.alignment = alignment;
	}

}

