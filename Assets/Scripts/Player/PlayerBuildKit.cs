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
		private Vector3 _rot = Vector3.zero;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Update() {
			if (!_building) return;

			if (!Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward),
				    out var hit,
				    Mathf.Infinity,
				    LayerMask.GetMask("BuildingBlock")
			    )) {
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
			_ia.BuildKit.RotateLeft.performed += OnRotateLeft;
			_ia.BuildKit.RotateForward.performed += OnRotateForward;
		}

		private void OnDisable() {
			_ia.BuildKit.Start.performed -= OnStart;
			_ia.BuildKit.Place.performed -= OnPlace;
			_ia.BuildKit.Cancel.performed -= OnCancel;
			_ia.BuildKit.RotateLeft.performed -= OnRotateLeft;
			_ia.BuildKit.RotateForward.performed -= OnRotateForward;
			_ia.Disable();
		}

		private void OnStart(InputAction.CallbackContext _) {
			if (_building) return;

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

		private void OnRotateLeft(InputAction.CallbackContext _) {
			if (!_building || !_preview.activeSelf) return;
			_rot += Vector3.up * 90f;
		}

		private void OnRotateForward(InputAction.CallbackContext _) {
			if (!_building || !_preview.activeSelf) return;
			_rot += Vector3.left * 90f;
		}
	}
}