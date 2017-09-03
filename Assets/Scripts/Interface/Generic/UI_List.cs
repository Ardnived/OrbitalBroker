using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class UI_List<T> : MonoBehaviour  {

	public Transform container;

	public GameObject listItemTemplate;
	public GameObject headerTemplate;

	protected List<UI_ListHeader> headerPool = new List<UI_ListHeader>();
	protected float headerHeight;
	protected int headerCount;

	protected List<Item> itemPool = new List<Item>();
	protected float itemHeight;
	protected int itemCount;

	protected void Awake() {
		for (int i = container.childCount - 1; i >= 0; i--) {
			Destroy(container.GetChild(i).gameObject);
		}

		itemHeight = (listItemTemplate.transform as RectTransform).rect.height;

		if (headerTemplate != null) {
			headerHeight = (headerTemplate.transform as RectTransform).rect.height;
		}
	}

	// TODO: Combine these two functions, the only difference is List vs IEnumerable... isn't List an IEnumerable???
	public void SetData(Dictionary<string, List<T>> data) {
		itemCount = 0;
		headerCount = 0;

		foreach (KeyValuePair<string, List<T>> pair in data) {
			if (!string.IsNullOrEmpty(pair.Key)) {
				AppendHeader(pair.Key);
			}

			foreach (T element in pair.Value) {
				AppendElement(element);
			}
		}

		for (int i = itemCount; i < itemPool.Count; i++) {
			itemPool[i].gameObject.SetActive(false);
		}

		for (int i = headerCount; i < headerPool.Count; i++) {
			headerPool[i].gameObject.SetActive(false);
		}

		RectTransform rectTransform = container.transform as RectTransform;
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (itemHeight * itemCount) + (headerHeight * headerCount));
	}

	public void SetData(Dictionary<string, IEnumerable<T>> data) {
		itemCount = 0;
		headerCount = 0;

		foreach (KeyValuePair<string, IEnumerable<T>> pair in data) {
			if (!string.IsNullOrEmpty(pair.Key)) {
				AppendHeader(pair.Key);
			}

			foreach (T element in pair.Value) {
				AppendElement(element);
			}
		}

		for (int i = itemCount; i < itemPool.Count; i++) {
			itemPool[i].gameObject.SetActive(false);
		}

		for (int i = headerCount; i < headerPool.Count; i++) {
			headerPool[i].gameObject.SetActive(false);
		}

		RectTransform rectTransform = container.transform as RectTransform;
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (itemHeight * itemCount) + (headerHeight * headerCount));
	}

	public void SetData(IEnumerable<T> data) {
		itemCount = 0;
		headerCount = 0;

		foreach (T element in data) {
			AppendElement(element);
		}

		for (int i = itemCount; i < itemPool.Count; i++) {
			itemPool[i].gameObject.SetActive(false);
		}

		for (int i = 0; i < headerPool.Count; i++) {
			headerPool[i].gameObject.SetActive(false);
		}

		RectTransform rectTransform = container.transform as RectTransform;
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (itemHeight * itemCount));
	}

	private void AppendHeader(string text) {
		if (headerTemplate == null) {
			Debug.LogError("Tried to use a list header, when no header template is defined");
			return;
		}
			
		UI_ListHeader header;
		int index = headerCount; 

		if (headerPool.Count > index) {
			header = headerPool[index];
			header.gameObject.SetActive(true);
		} else {
			header = Instantiate(headerTemplate).GetComponent<UI_ListHeader>();
			header.transform.SetParent(container.transform, false);

			headerPool.Add(header);
		}

		//float yMax = -((itemCount * itemHeight) + (headerCount * headerHeight));
		//float yMin = yMax - headerHeight;
		Vector2 offsetY = GetOffsetY(headerHeight);

		RectTransform rectTransform = header.transform as RectTransform;
		rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, offsetY.y);//yMax);
		rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, offsetY.x);//yMin);

		header.SetTitle(text);
		headerCount++;
	}

	private void AppendElement(T element) {
		Item listItem;
		int index = itemCount;

		if (itemPool.Count > index) {
			listItem = itemPool[index];
			listItem.gameObject.SetActive(true);
		} else {
			listItem = Instantiate(listItemTemplate).GetComponent<Item>();
			listItem.transform.SetParent(container.transform, false);

			listItem.SetMaster(this);
			itemPool.Add(listItem);
		}

		//float yMax = -((itemCount * itemHeight) + (headerCount * headerHeight));
		//float yMin = yMax - itemHeight;
		Vector2 offsetY = GetOffsetY(itemHeight);

		RectTransform rectTransform = listItem.transform as RectTransform;
		rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, offsetY.y);//yMax);
		rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, offsetY.x);//yMin);

		listItem.SetElement(element);
		itemCount++;
	}

	protected virtual Vector2 GetOffsetY(float height) {
		float yMax = -((itemCount * itemHeight) + (headerCount * headerHeight));
		float yMin = yMax - height;
		return new Vector2(yMin, yMax);
	}

	protected virtual UnityEvent<T> GetSelectionHandler() { return null; }

	public abstract class Item : MonoBehaviour, ISelectHandler {

		protected UI_List<T> master;
		protected T element;

		public virtual void SetMaster(UI_List<T> master) {
			this.master = master;
		}

		public void SetElement(T element) {
			this.element = element;
			Set(element);
		}

		// Sometimes this need to be called seperately
		protected abstract void Set(T element);

		public virtual void OnSelect(BaseEventData eventData) {
			UnityEvent<T> callback = master.GetSelectionHandler();

			if (callback != null) {
				callback.Invoke(element);
			}
		}

		public virtual void OnDeselect(BaseEventData eventData) {
			// Do nothing
		}

	}

	#if UNITY_EDITOR
	public class ListEditor : Editor {}
	#endif
}