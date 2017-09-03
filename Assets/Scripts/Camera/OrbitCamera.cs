using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour {

	public Transform target;

	public int minY = -20, maxY = 80;
	public float smoothTime = 0.3f;
	public float currentZ = 10f;
	public float minZ = 5f, maxZ = 12f;

	public int zoomSensitivity = 5;
	public Vector2 sensitivity = new Vector2(5.0f, 2.4f);

	private float targetX = 0f, targetY = 0f;
	private float currentX = 0f, currentY = 0f;
	private float velocityX = 0f, velocityY = 0f;

	protected void LateUpdate() {
		Quaternion rotation;
		Vector3 position;

		if (Input.GetAxis("Fire2") != 0) {
			targetX += Input.GetAxis("Mouse X") * sensitivity.x;
			targetY -= Input.GetAxis("Mouse Y") * sensitivity.y;

			currentX = Mathf.SmoothDamp(currentX, targetX, ref velocityX, smoothTime);
			currentY = Mathf.SmoothDamp(currentY, targetY, ref velocityY, smoothTime);
			currentY = ClampAngle(currentY, minY, maxY);

			rotation = Quaternion.Euler(currentY, currentX, 0);
		}

		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			currentZ -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
			currentZ = Mathf.Clamp(currentZ, minZ, maxZ);
		}

		rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(currentY, currentX, 0), Time.deltaTime * 3);
		position = rotation * new Vector3(0, 0, -currentZ) + target.position;

		transform.rotation = rotation;
		transform.position = position;
	}

	private static float ClampAngle(float value, float min, float max) {
		if (value < -360) {
			value += 360;
		} else if (value > 360) {
			value -= 360;
		}

		return Mathf.Clamp(value, min, max);
	}

}

