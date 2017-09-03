using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OrbitData {
	[Range(0f, 360f)]
	public float CurrentAngle; // in degrees

	[Header("Parameters")]
	[Range(0f, 0.8f)]
	public float Eccentricity;
	[Range(0, 360)]
	public int Rotation; // in degrees
	[Range(-90, 90)]
	public int Inclination; // in degrees
	[Range(160, 35786)]
	public int Altitude; // in kilometers

	private const int RESOLUTION = 144; // 10 minutes per step
	private const float SECONDS_PER_STEP = (24f / RESOLUTION) * 60 * 60;

	public CoveragePoint[] Coverage;

	[NonSerialized]
	private Vector4[] coverageForShaders;
	// TODO: Shouldn't we be able to serialize Vector4 directly? And do away with the need for CoverageData. Google it.

	public void RecalculateCoverage() {
		this.Coverage = new CoveragePoint[RESOLUTION];

		float currentOrbitAngle = 0;

		for (int i = 0; i < RESOLUTION; i++) {
			float planetRotation = 0;//(float)i * 360f / (float)orbitResolution;

			LatLong latlong = GetLatLong(currentOrbitAngle, planetRotation);
			//float orbitDistance = GetDistance(currentOrbitAngle);
			//int coverageRadius = (int) orbitDistance / 10000;

			this.Coverage[i] = new CoveragePoint(latlong.Latitude, latlong.Longitude, 3);

			float currentOrbitSpeed = GetAngularVelocity(currentOrbitAngle);
			currentOrbitAngle += currentOrbitSpeed * SECONDS_PER_STEP;
		}

		/*
		for (int i = 0; i < RESOLUTION; i++) {
			currentOrbitAngle = i;
			LatLong latlong = GetLatLong(currentOrbitAngle, 0);
			float orbitDistance = GetDistance(currentOrbitAngle);
			int coverageRadius = (int) orbitDistance / 10000;

			Coverage[i] = new Vector4(latlong.Latitude, latlong.Longitude, 3);

			float currentOrbitSpeed = GetAngularVelocity(currentOrbitAngle);
			currentOrbitAngle += currentOrbitSpeed * SECONDS_PER_STEP;
		}
		*/

		coverageForShaders = null;
	}

	public Vector4[] GetCoverage() {
		if (coverageForShaders == null) {
			coverageForShaders = new Vector4[Coverage.Length];

			for (int i = 0; i < Coverage.Length; i++) {
				coverageForShaders[i] = Coverage[i].ToVector4();
			}
		}

		return coverageForShaders;
	}

	public Vector3 GetPositionAtAngle(float angle) {
		float distance = GetDistance(angle);
		angle *= Mathf.Deg2Rad;

		float x = distance * Mathf.Cos(angle);
		float y = 0;
		float z = distance * Mathf.Sin(angle);

		return Quaternion.Euler(new Vector3(0, -this.Rotation, this.Inclination)) * new Vector3(x, y, z);
	}

	public LatLong GetLatLong(float angle, float planetRotation) {
		float latitude = Mathf.Cos(angle * Mathf.Deg2Rad) * this.Inclination;
		float longitude = angle + this.Rotation + planetRotation;

		//Debug.Log("Got LatLong "+latitude+","+longitude+" -> "+new LatLong(latitude, longitude)+" from A: "+angle+" R: "+Rotation+" I: "+Inclination);
		return new LatLong(latitude, longitude);
	}

	public float GetDistance(float angle) {
		angle *= Mathf.Deg2Rad;

		float radius = Constant.PlanetRadius + Altitude;
		float semiMajorAxis = radius / (1 - Eccentricity);
		return semiMajorAxis * (1 - Mathf.Pow(Eccentricity, 2)) / (1 - Eccentricity * Mathf.Cos(angle));
	}

	public float GetAngularVelocity(float angle) {
		float distance = GetDistance(angle);
		angle *= Mathf.Deg2Rad;

		return Mathf.Sqrt(Constant.GravitationalParameter / Mathf.Pow(distance, 3));
	}

	[System.Serializable]
	public struct CoveragePoint {
		public int Latitude;
		public int Longitude;
		public int Radius;

		public CoveragePoint(int latitude, int longitude, int radius) {
			this.Latitude = latitude;
			this.Longitude = longitude;
			this.Radius = radius;
		}

		public Vector4 ToVector4() {
			return new Vector4(Latitude, Longitude, Radius);
		}
	}
}
