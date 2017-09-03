using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArrayLoop<T> {

	[SerializeField]
	protected T[] data;
	[SerializeField]
	protected int startIndex = 0;
	[SerializeField]
	protected int count = 0;

	public int Length {
		get { return count; }
	}

	public T Latest {
		get {
			int index = startIndex == 0 ? count - 1 : startIndex - 1;
			return data[index];
		}
	}

	public ArrayLoop(int capacity) {
		data = new T[capacity];
	}

	public T this[int index] {
		get {
			index += startIndex;

			if (index >= data.Length) {
				index = index % data.Length;
			}

			return data[index];
		}
	}

	public void Push(T element) {
		if (count < data.Length) {
			data[count] = element;
			count++;
		} else {
			data[startIndex] = element;
			startIndex++;

			if (startIndex >= data.Length) {
				startIndex = 0;
			}
		}
	}

	public void Clear() {
		count = 0;
		startIndex = 0;
	}
}

