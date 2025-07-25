using System.Collections.Generic;
using Notifications;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace Item {
    public class Magnet_smol : MonoBehaviour, IUsableItem {
        [SerializeField] private float pullSpeed = 25f;
        [SerializeField] private float maxDistance = 1000000f;
        [SerializeField] private float grabDistance = 2f;
        [SerializeField] private List<VisualEffect> beamVFX;
        [SerializeField] private List<Transform> beamOrigins;
        [SerializeField] private AudioSource magnetSound;
        [SerializeField] private Animator animator;


        [Header("Beam Correction")]
        [SerializeField] private float horizontalCorrection;
        [SerializeField] private bool showDebugLines = true;
        private InputSystem_Actions _ia;
        private bool _isActive;
        private NotificationManager _notificationManager;
        private Transform _origin;

        private void Awake() {
            _ia = new InputSystem_Actions();
        }

        private void FixedUpdate() {
            if (!_isActive || !_origin) {
                DisableBeams();
                return;
            }

            bool hitSomething = Physics.Raycast(_origin.position, _origin.forward, out var laserHit, maxDistance);

            if (hitSomething && laserHit.collider.TryGetComponent(out Loot.Loot _currentLoot)) 
            {
                animator.SetBool("isPulling", true);
                if (!magnetSound.isPlaying) 
                {
                    magnetSound.Play();
                }

                float distance = Vector3.Distance(_currentLoot.transform.position, _origin.position);
                if (distance <= grabDistance) 
                {
                    DisableBeams();
                    return;
                }

                _currentLoot.transform.position = Vector3.MoveTowards(
                    _currentLoot.transform.position,
                    _origin.position,
                    pullSpeed * Time.fixedDeltaTime
                );

                for (int i = 0; i < beamVFX.Count; i++)
                {
                    if (beamVFX[i] == null || beamOrigins[i] == null) continue;

                    var vfx = beamVFX[i];
                    var origin = beamOrigins[i];

                    Vector3 targetPoint = laserHit.point;
                    Vector3 direction = targetPoint - origin.position;

                    Quaternion rotationAdjustment = Quaternion.Euler(0, -90, 0); 
                    Vector3 adjustedDirection = rotationAdjustment * direction;

                    vfx.transform.position = origin.position;
                    vfx.SetVector3("TargetPosition", targetPoint);
                    vfx.SetFloat("BeamLength", adjustedDirection.magnitude);

                    // Set fixed axis orientation (normalized vector)
                    Vector3 fixedAxis = new Vector3(0.577f, 0.577f, 0.577f).normalized;
                    vfx.SetVector3("Orient: Fixed Axis", fixedAxis);

                    vfx.gameObject.SetActive(true);
                }
            }
            else 
            {
                animator.SetBool("isPulling", false);
                magnetSound.Stop();
                DisableBeams();
            }
        }

        private void DisableBeams() {
            foreach (var vfx in beamVFX) {
                if (vfx != null)
                    vfx.gameObject.SetActive(false);
            }
        }

        public void OnEnable() {
            _ia.Player.Enable();
            _ia.Player.UseItem.started += Use;
            _ia.Player.UseItem.canceled += Unuse;
        }

        public void OnDisable() {
            _ia.Player.UseItem.started -= Use;
            _ia.Player.UseItem.canceled -= Unuse;
            _ia.Player.Disable();
        }

        private void OnDrawGizmosSelected() {
            if (_origin != null) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(_origin.position, _origin.position + _origin.forward * maxDistance);
            }
        }

        public void Init(Transform origin, List<ItemParameter> itemParameters) {
            _origin = origin;

            transform.rotation = _origin.rotation * Quaternion.Euler(-11f, -90f, 96f);
            DisableBeams();
            _notificationManager = FindFirstObjectByType<NotificationManager>();
        }

        private void Use(InputAction.CallbackContext context) {
            if (_isActive) return;
            _isActive = true;
            _notificationManager?.ShowNotification(
                "Aim at loot and hold the button to pull it toward you!");
        }

        private void Unuse(InputAction.CallbackContext context) {
            if (!_isActive) return;
            animator.SetBool("isPulling", false);
            magnetSound.Stop();
            _isActive = false;
            DisableBeams();
        }
    }
}