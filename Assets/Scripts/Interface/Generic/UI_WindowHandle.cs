using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WindowHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public UI_Window window;
	public int borderWidth = 10;

	//private Image imageComponent;
	private DragType dragType = DragType.None;
	private Vector3 dragOffset;

	void Awake() {
		//imageComponent = GetComponent<UnityEngine.UI.Image>();
	}

	private DragType GetDragType(Vector2 position) {
		RectTransform rectTransform = this.transform as RectTransform;
		Rect bounds = rectTransform.rect;
		DragType result;

		bool onLeftBorder   = Mathf.Abs(position.x - bounds.xMin) < borderWidth;
		bool onBottomBorder = Mathf.Abs(position.y - bounds.yMin) < borderWidth;
		bool onRightBorder  = Mathf.Abs(position.x - bounds.xMax) < borderWidth;
		bool onTopBorder    = Mathf.Abs(position.y - bounds.yMax) < borderWidth;
		bool onBorder = onLeftBorder || onTopBorder || onRightBorder || onBottomBorder;

		if (!onBorder) {
			result = DragType.None;
		} else if (onLeftBorder && onTopBorder) {
			result = DragType.TopLeftResize;
		} else if (onRightBorder && onTopBorder) {
			result = DragType.TopRightResize;
		} else if (onLeftBorder && onBottomBorder) {
			result = DragType.BottomLeftResize;
		} else if (onRightBorder && onBottomBorder) {
			result = DragType.BottomRightResize;
		} else {
			result = DragType.Reposition;
		}

		Debug.Log("Drag type set to " + result + ", left: " + onLeftBorder + ", top: " + onTopBorder + ", right: " + onRightBorder + ", bottom: " + onBottomBorder);
		return result;
	}

	public void OnBeginDrag(PointerEventData data) {
		Vector2 localPosition;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, data.pressPosition, data.pressEventCamera, out localPosition);
		RectTransformUtility.ScreenPointToWorldPointInRectangle(window.transform.parent as RectTransform, localPosition, data.pressEventCamera, out dragOffset);
		dragType = GetDragType(localPosition);
		//imageComponent.enabled = true;
	}

	public void OnEndDrag(PointerEventData data) {
		//imageComponent.enabled = false;
	}

	public void OnDrag(PointerEventData data) {
		if (dragType == DragType.None) {
			return;
		}

		Vector3 mousePosition;
		RectTransform windowTransform = window.transform as RectTransform;
		RectTransform parentTransform = windowTransform.parent as RectTransform;

		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(windowTransform.parent as RectTransform, data.position, data.pressEventCamera, out mousePosition)) {
			float x, y;

			if (dragType == DragType.Reposition) {
				mousePosition -= dragOffset + this.transform.localPosition;
				x = mousePosition.x;
				y = Mathf.Clamp(mousePosition.y, 0, 1000);
			} else {
				x = mousePosition.x;
				y = Mathf.Clamp(mousePosition.y, 0, 1000);
				Debug.Log("Position is " + mousePosition + " vs " + windowTransform.offsetMin + " vs " + windowTransform.anchorMin);
			}

			switch (dragType) {
			case DragType.Reposition:
				windowTransform.position = new Vector3(x, y, mousePosition.z);
				break;
			case DragType.TopLeftResize:
				x -= parentTransform.rect.width * windowTransform.anchorMin.x;
				y -= parentTransform.rect.height * windowTransform.anchorMax.y;

				windowTransform.offsetMin = new Vector2(x, windowTransform.offsetMin.y);
				windowTransform.offsetMax = new Vector2(windowTransform.offsetMax.x, y);
				break;
			case DragType.TopRightResize:
				x -= parentTransform.rect.width * windowTransform.anchorMax.x;
				y -= parentTransform.rect.height * windowTransform.anchorMax.y;

				windowTransform.offsetMax = new Vector2(x, y);
				break;
			case DragType.BottomLeftResize:
				x -= parentTransform.rect.width * windowTransform.anchorMin.x;
				y -= parentTransform.rect.height * windowTransform.anchorMin.y;

				windowTransform.offsetMin = new Vector2(x, y);
				break;
			case DragType.BottomRightResize:
				x -= parentTransform.rect.width * windowTransform.anchorMax.x;
				y -= parentTransform.rect.height * windowTransform.anchorMin.y;

				windowTransform.offsetMin = new Vector2(windowTransform.offsetMin.x, y);
				windowTransform.offsetMax = new Vector2(x, windowTransform.offsetMax.y);
				break;
			}
		}
	}

	public void OnMouseOver(PointerEventData data) {
		Vector2 localPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, data.pressPosition, data.pressEventCamera, out localPosition);

		Debug.Log("On Mouse Over on handler");

		/*
		switch (GetDragType()) {

		}
		*/
	}

	private enum DragType {
		Reposition,
		TopLeftResize,
		TopRightResize,
		BottomLeftResize,
		BottomRightResize,
		None,
	}

}

