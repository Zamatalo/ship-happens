using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerBuildKit : MonoBehaviour {
		public GameObject planePrefab;
		public GameObject planePreviewPrefab;
		public Transform cameraTransform;
		private bool _building;
		private InputSystem_Actions _ia;
		private GameObject _preview;
		private Vector3 _rot;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Update() {
			if (!_building) return;

			if (!Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward),
				    out var hit)) {
				_preview.SetActive(false);
				return;
			}

			_preview.SetActive(true);
			_preview.transform.position = hit.point;
			_preview.transform.rotation = Quaternion.Euler(_rot);
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
			if (!_building || !_preview.activeSelf) return;

			Instantiate(planePrefab, _preview.transform.position, _preview.transform.rotation);
			Destroy(_preview);

			_building = false;
		}

		private void OnCancel(InputAction.CallbackContext _) {
			if (!_building) return;

			Destroy(_preview);
			_building = false;
		}
	}
}