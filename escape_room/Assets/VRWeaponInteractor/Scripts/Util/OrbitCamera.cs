using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeaponInteractor
{
	public class OrbitCamera : MonoBehaviour 
	{
		[Tooltip("Will find Camera.main if not specified")]
		public Transform cameraTransform;

		void LateUpdate()
		{
			if (Camera.main != null && (cameraTransform == null || !cameraTransform.root.gameObject.activeSelf)) cameraTransform = Camera.main.transform;
			if (cameraTransform == null) return;
			Vector3 newForward = cameraTransform.forward;
			newForward.y = 0;
			transform.position = cameraTransform.position;
			transform.forward = newForward;
		}
	}
}