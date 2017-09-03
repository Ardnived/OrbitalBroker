using System;
using UnityEngine;

public class TestGraphShader : MonoBehaviour {

	public Material graphMaterial;
	public float[] Values;

	public void OnValidate() {
		graphMaterial.SetFloatArray("_Data", new float[20]);

		graphMaterial.SetInt("_Data_Count", Values.Length);
		graphMaterial.SetFloatArray("_Data", Values);
	}

}