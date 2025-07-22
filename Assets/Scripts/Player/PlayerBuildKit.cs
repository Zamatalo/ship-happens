using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerBuildKit : MonoBehaviour {
		public GameObject planePrefab;
		public GameObject planePreviewPrefab;
		public Transform cameraTransform;
		public float snapSize = 2f;
		private bool _building;
		private InputSystem_Actions _ia;
		private GameObject _preview;
		private Vector3 _rot = Vector3.zero;
		private Transform _shipTransform;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			_shipTransform = GameObject.Find("Ship").transform;
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

			var snapped = new Vector3(
				Mathf.Round(hit.point.x / snapSize) * snapSize,
				hit.point.y,
				Mathf.Round(hit.point.z / snapSize) * snapSize
			);

			var forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z)
				.normalized;
			snapped += forward * snapSize;

			snapped = new Vector3(
				Mathf.Round(snapped.x / snapSize) * snapSize,
				Mathf.Round(snapped.y / snapSize) * snapSize,
				Mathf.Round(snapped.z / snapSize) * snapSize
			);

			var rot = _rot;
			if (_rot.x != 0f) {
				var halfSnapSize = snapSize / 2;
				snapped -= forward * snapSize / 2;
				snapped.y += snapSize / 2;
				snapped.x = Mathf.Round(snapped.x / halfSnapSize) * halfSnapSize;
				snapped.z = Mathf.Round(snapped.z / halfSnapSize) * halfSnapSize;

				var rot2 = Quaternion.LookRotation(forward, Vector3.forward).eulerAngles;
				rot.y = Mathf.Round(rot2.y / 90f) * 90f;
			}

			_preview.transform.position = snapped;
			_preview.transform.rotation = Quaternion.Euler(rot);
		}

		private void OnEnable() {
			_ia.Enable();
			_ia.BuildKit.Start.performed += OnStart;
			_ia.BuildKit.Place.performed += OnPlace;
			_ia.BuildKit.Cancel.performed += OnCancel;
			_ia.BuildKit.Rotate.performed += OnRotate;
		}

		private void OnDisable() {
			_ia.BuildKit.Start.performed -= OnStart;
			_ia.BuildKit.Place.performed -= OnPlace;
			_ia.BuildKit.Cancel.performed -= OnCancel;
			_ia.BuildKit.Rotate.performed -= OnRotate;
			_ia.Disable();
		}

		private void OnStart(InputAction.CallbackContext _) {
			if (_building) return;

			_building = true;
			_preview = Instantiate(planePreviewPrefab);
		}

		private void OnPlace(InputAction.CallbackContext _) {
			if (!_building || !_preview.activeSelf) return;

			Instantiate(
				planePrefab,
				_preview.transform.position,
				_preview.transform.rotation,
				_shipTransform
			);
			Destroy(_preview);

			_building = false;
			_rot = Vector3.zero;
		}

		private void OnCancel(InputAction.CallbackContext _) {
			if (!_building) return;

			Destroy(_preview);
			_building = false;
			_rot = Vector3.zero;
		}

		private void OnRotate(InputAction.CallbackContext _) {
			if (!_building || !_preview.activeSelf) return;
			_rot.x = _rot.x == 0f ? 90f : 0f;
		}
	}
}