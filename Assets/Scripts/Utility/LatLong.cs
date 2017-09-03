using System;
using UnityEngine;

[Serializable]
public struct LatLong {
	public int Latitude;
	public int Longitude;

	public LatLong(float latitude, float longitude) : this(Mathf.RoundToInt(latitude), Mathf.RoundToInt(longitude)) {
		// Do Nothing
	}

	public LatLong(int latitude, int longitude) {
		if (latitude > Constant.MaxLatitude) {
			int flips = (latitude - Constant.MinLatitude) / Constant.LatitudeCount;
			int remainder = (latitude - Constant.MinLatitude) % Constant.LatitudeCount;

			if (flips % 2 == 0) { // Meaning it is even
				latitude = Constant.MaxLatitude - remainder;
			} else {
				latitude = Constant.MinLatitude + remainder;
			}

			longitude += flips * Constant.LongitudeCount / 2;
		} else if (latitude < Constant.MinLatitude) {
			int flips = Mathf.Abs(latitude - Constant.MaxLatitude) / Constant.LatitudeCount;
			int remainder = Mathf.Abs(latitude - Constant.MaxLatitude) % Constant.LatitudeCount;

			if (flips % 2 == 0) { // Meaning it is even
				latitude = Constant.MinLatitude + remainder;
			} else {
				latitude = Constant.MaxLatitude - remainder;
			}

			longitude += flips * Constant.LongitudeCount / 2;
		}

		if (longitude >= Constant.MaxLongitude) {
			longitude = (longitude - Constant.MinLongitude) % Constant.LongitudeCount;
			longitude += Constant.MinLongitude;

			// The last longitude should wrap precisely onto the first one.
			if (longitude == 180) {
				longitude = -180;
			}
		} else if (longitude < Constant.MinLongitude) {
			longitude = (longitude - Constant.MaxLongitude) % -Constant.LongitudeCount;
			longitude += Constant.MaxLongitude;
		}

		this.Latitude = latitude;
		this.Longitude = longitude;
	}

	public override string ToString() {
		return string.Format("("+Latitude+", "+Longitude+")");
	}

}
