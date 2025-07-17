using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerBuildKit : MonoBehaviour {
		public GameObject planePreviewPrefab;
		public Transform cameraTransform;
		private bool _building;
		private InputSystem_Actions _ia;
		private GameObject _preview;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Update() {
			if (!Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward),
				    out var hit)) return;

			_preview.transform.position = hit.point;
			_preview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.BuildKit.Start.performed += OnStart;
			_ia.BuildKit.Place.performed += OnPlace;
			_ia.BuildKit.Cancel.performed += OnCancel;
		}

		private void OnDisable() {
			_ia.BuildKit.Start.performed -= OnStart;
			_ia.BuildKit.Place.performed -= OnPlace;
			_ia.BuildKit.Cancel.performed -= OnCancel;
			_ia.Disable();
		}

		private void OnStart(InputAction.CallbackContext _) {
			_building = true;
			_preview = Instantiate(planePreviewPrefab);
		}

		private void OnPlace(InputAction.CallbackContext _) {
		}

		private void OnCancel(InputAction.CallbackContext _) {
			_building = false;
		}
	}
}