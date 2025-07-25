using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item {
	public class BuildKit : MonoBehaviour, IUsableItem {
		public GameObject planePrefab;
		public GameObject planePreviewPrefab;
		public float snapSize = 2f;
		private Transform _cameraTransform;
		private InputSystem_Actions _ia;
		private PlayerInventoryNew _inventory;
		private GameObject _preview;
		private Vector3 _rot = Vector3.zero;
		private Transform _shipTransform;
		private UIManager _ui;

		private void Awake() {
			_ia = new InputSystem_Actions();
		}

		private void Start() {
			_inventory = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponentInChildren<PlayerInventoryNew>();
			_ui = NetworkManager.Singleton.LocalClient.PlayerObject
				.GetComponentInChildren<UIManager>();
			_shipTransform = GameObject.Find("Ship").transform;
			_preview = Instantiate(planePreviewPrefab);
		}

		private void Update() {
			if (!_ui.CheckInventory("Platform", 1)) {
				_preview.SetActive(false);
				return;
			}

			if (!Physics.Raycast(new Ray(_cameraTransform.position, _cameraTransform.forward),
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

			var forward = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z)
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
			_ia.BuildKit.Place.performed += OnPlace;
			_ia.BuildKit.Rotate.performed += OnRotate;
		}

		private void OnDisable() {
			_ia.BuildKit.Place.performed -= OnPlace;
			_ia.BuildKit.Rotate.performed -= OnRotate;
			_ia.Disable();
		}

		public void Init(Transform cameraTransform, List<ItemParameter> itemParameters) {
			_cameraTransform = cameraTransform;
		}

		private void OnPlace(InputAction.CallbackContext _) {
			if (!_preview.activeSelf) return;

			Instantiate(
				planePrefab,
				_preview.transform.position,
				_preview.transform.rotation,
				_shipTransform
			);
			_rot = Vector3.zero;

			_inventory.RemoveItemServerRpc("Platform", 1);
		}

		private void OnRotate(InputAction.CallbackContext _) {
			if (!_preview.activeSelf) return;
			_rot.x = _rot.x == 0f ? 90f : 0f;
		}
	}
}