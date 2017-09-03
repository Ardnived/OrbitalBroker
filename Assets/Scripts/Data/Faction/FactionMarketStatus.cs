using UnityEngine;
using System;
using System.Collections.Generic;

public struct FactionMarketStatus {

	[SerializeField]
	private int[] totalSupply;
	[SerializeField]
	private int[] supply;

	public void Init() {
		supply = new int[Constant.Market.TypeCount * GameController.Map.Width * GameController.Map.Height];
		totalSupply = new int[Constant.Market.TypeCount];
	}

	public void AdjustSupply(MarketType type, OrbitData orbit, int value) {
		if (value == 0) {
			return;
		}

		int t = (int)type;
		int total = 0;
		int index;

		Debug.Log("Orbit has "+orbit.Coverage.Length+" coverage points.");
		for (int i = 0; i < orbit.Coverage.Length; i++) {
			OrbitData.CoveragePoint point = orbit.Coverage[i];

			for (int x = -point.Radius; x <= point.Radius; x++) {
				for (int y = -point.Radius; y <= point.Radius; y++) {
					LatLong latlong = new LatLong(point.Latitude + x, point.Longitude + y);

					int latDiff = latlong.Latitude - point.Latitude;
					int lonDiff = latlong.Longitude - point.Longitude;
					float distance = Mathf.Sqrt(latDiff * latDiff + lonDiff * lonDiff);

					if (distance <= point.Radius) {
						try {
							index = GetIndex(t, latlong.Latitude - Constant.MinLatitude, latlong.Longitude - Constant.MinLongitude);
							supply[index] += value;
							total += value;
						} catch (Exception exc) {
							Debug.Log("Attempting to set "+t+" "+(latlong.Latitude - Constant.MinLatitude)+", "+(latlong.Longitude - Constant.MinLongitude)+" += "+value);
							throw exc;
						}
					}
				}
			}
		}

		Debug.Log("Added "+value+" -> "+total+" supply to faction #"+ID+"'s "+type+" market");

		totalSupply[t] += total;
	}

	public int GetSupply(MarketType type, int x, int y) {
		int index = GetIndex((int)type, x, y);
		return supply[index];
	}

	private int GetIndex(int t, int x, int y) {
		return x + GameController.Map.Width * (y + GameController.Map.Height * t);
	}

}
