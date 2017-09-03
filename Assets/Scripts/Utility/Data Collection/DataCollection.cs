using System;
using System.Collections;
using System.Collections.Generic;

public class DataCollection<T> : IEnumerable<T> where T : DataElement {

	private On.Event eventChannel;

	protected Dictionary<int, T> data = new Dictionary<int, T>();
	private int nextID = 1;

	public DataCollection(On.Event eventChannel) {
		this.eventChannel = eventChannel;
	}

	public int Count {
		get { return data.Count; }
	}

	public virtual void Set(T element) {
		if (data.ContainsKey(element.ID)) {
			data[element.ID] = element;
		} else {
			element.ID = nextID;
			data.Add(element.ID, element);
			nextID++;
		}

		Publish();
	}

	public T Get(int ID) {
		return data[ID];
	}

	public bool Has(int ID) {
		return data.ContainsKey(ID);
	}

	public void Remove(T element) {
		data.Remove(element.ID);

		Publish();
	}

	public void Remove(int ID) {
		T element = data[ID];
		data.Remove(ID);

		Publish();
	}

	public void Clear() {
		data.Clear();
		nextID = 1;
	}

	public T[] Serialize() {
		T[] result = new T[data.Count];
		data.Values.CopyTo(result, 0);
		return result;
	}

	public void Deserialize(T[] array) {
		for (int i = 0; i < array.Length; i++) {
			T element = array[i];
			data.Add(element.ID, element);
			nextID = UnityEngine.Mathf.Max(nextID, element.ID + 1);
		}

		Publish();
	}

	public virtual void Publish() {
		if (eventChannel != null) {
			eventChannel.Trigger();
		}
	}

	public IEnumerator<T> GetEnumerator() {
		return data.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return data.Values.GetEnumerator();
	}

}

public interface DataObserver {
	void OnDataChanged();
}

public interface DataElement {
	int ID { get; set; }
}

public interface OwnedDataElement : DataElement {
	int OwnerID { get; set; }
}
