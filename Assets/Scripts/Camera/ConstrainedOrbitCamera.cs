using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstrainedOrbitCamera : OrbitCamera {

	[SerializeField]
	private Transform constraint;
	private Vector3 oldConstraintPosition;

	// TODO: Make this constrained orbit camera actually do something.
	protected new void LateUpdate() {
		base.LateUpdate();

		if (constraint.position != oldConstraintPosition) {
			transform.position += constraint.position - oldConstraintPosition;
			oldConstraintPosition = constraint.position;
		}

		// TODO: Allow the controls to still work.
		transform.LookAt(target.position);
	}

}
