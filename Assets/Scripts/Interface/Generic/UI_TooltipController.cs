using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UI_TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public int TooltipIndex = 0;
	public string Text;
	public Rect Rect;
	public TextAnchor Alignment = TextAnchor.MiddleCenter;

	private bool enable = true;
	private bool hovered = false;

	public Rect DrawRect {
		get {
			RectTransform rectTransform = this.transform as RectTransform;
			Vector2 center = rectTransform.TransformPoint(rectTransform.rect.center);
			return new Rect(Rect.x + center.x - Rect.width / 2, Rect.y + center.y - Rect.height / 2, Rect.width, Rect.height);
		}
	}

	public void SetEnabled(bool enabled) {
		this.enable = enabled;

		if (hovered) {
			SetVisible(enabled);
		}
	}

	public void OnPointerEnter(PointerEventData data) {
		hovered = true;

		if (enable) {
			SetVisible(true);
		}
	}

	public void OnPointerExit(PointerEventData data) {
		hovered = false;

		if (enable) {
			SetVisible(false);
		}
	}

	public void SetVisible(bool visible) {
		if (visible) {
			UI_Tooltip.Instances[TooltipIndex].SetAlignment(Alignment);
			UI_Tooltip.Instances[TooltipIndex].Show(DrawRect, Text);
		} else {
			UI_Tooltip.Instances[TooltipIndex].Hide();
		}
	}

	#if UNITY_EDITOR
	public Texture debugTexture;

	void OnDrawGizmosSelected() {
		Gizmos.DrawGUITexture(DrawRect, debugTexture);
	}
	#endif

}

