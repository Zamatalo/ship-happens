using System.Collections.Generic;
using Notifications;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Item 
{
    public class Magnet1 : MonoBehaviour, IUsableItem 
    {
        [Header("Настройки магнита")]
        [SerializeField] private float pullSpeed = 25f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float grabDistance = 2f;
        [SerializeField] private LayerMask lootLayerMask;

        [Header("Визуальные эффекты")]
        [SerializeField] private List<LineRenderer> beamLines;
        [SerializeField] private float beamWidth = 0.1f;
        [SerializeField] private Gradient beamGradient;

        [Header("Аудио")]
        [SerializeField] private AudioSource magnetSound;
        [SerializeField] private AudioClip activationSound;
        [SerializeField] private AudioClip pullingSound;

        [Header("Анимация")]
        [SerializeField] private Animator animator;
        [SerializeField] private string pullAnimParameter = "isPulling";

        private InputSystem_Actions _inputActions;
        private bool _isActive;
        private NotificationManager _notificationManager;
        private Transform _origin;
        private Loot.Loot _currentLoot;

        private void Awake() 
        {
            _inputActions = new InputSystem_Actions();
            InitializeBeams();
        }

        private void InitializeBeams()
        {
            foreach(var beam in beamLines)
            {
                beam.positionCount = 2;
                beam.startWidth = beamWidth;
                beam.endWidth = beamWidth;
                beam.colorGradient = beamGradient;
                beam.enabled = false;
            }
        }

        private void FixedUpdate() 
        {
            if (!_isActive || _origin == null) 
            {
                SetBeamsActive(false);
                return;
            }

            ProcessMagnet();
        }

        private void ProcessMagnet()
        {
            bool hasTarget = CheckForTarget(out Vector3 hitPoint);

            animator.SetBool(pullAnimParameter, hasTarget);
            UpdateAudio(hasTarget);
            UpdateBeams(hasTarget, hitPoint);

            if (hasTarget && _currentLoot != null)
            {
                PullLoot();
            }
        }

        private bool CheckForTarget(out Vector3 hitPoint)
        {
            if (Physics.Raycast(_origin.position, _origin.forward, 
                out RaycastHit hit, maxDistance, lootLayerMask))
            {
                if (hit.collider.TryGetComponent(out _currentLoot))
                {
                    hitPoint = hit.point;
                    return true;
                }
            }

            _currentLoot = null;
            hitPoint = Vector3.zero;
            return false;
        }

        private void PullLoot()
        {
            float distance = Vector3.Distance(_currentLoot.transform.position, _origin.position);
            
            if (distance > grabDistance)
            {
                _currentLoot.transform.position = Vector3.MoveTowards(
                    _currentLoot.transform.position,
                    _origin.position,
                    pullSpeed * Time.fixedDeltaTime
                );
            }
        }

        private void UpdateBeams(bool isActive, Vector3 endPosition)
        {
            foreach(var beam in beamLines)
            {
                beam.enabled = isActive;
                
                if (isActive)
                {
                    beam.SetPosition(0, beam.transform.position);
                    beam.SetPosition(1, endPosition);
                }
            }
        }

        private void UpdateAudio(bool isPulling)
        {
            if (isPulling)
            {
                if (!magnetSound.isPlaying || magnetSound.clip != pullingSound)
                {
                    magnetSound.clip = pullingSound;
                    magnetSound.Play();
                }
            }
            else
            {
                magnetSound.Stop();
            }
        }

        private void SetBeamsActive(bool isActive)
        {
            foreach(var beam in beamLines)
            {
                beam.enabled = isActive;
            }
        }

        public void OnEnable() 
        {
            _inputActions.Player.Enable();
            _inputActions.Player.UseItem.started += OnUseStarted;
            _inputActions.Player.UseItem.canceled += OnUseCanceled;
        }

        public void OnDisable() 
        {
            _inputActions.Player.UseItem.started -= OnUseStarted;
            _inputActions.Player.UseItem.canceled -= OnUseCanceled;
            _inputActions.Player.Disable();
        }

        private void OnUseStarted(InputAction.CallbackContext context)
        {
            if (_isActive) return;
            
            _isActive = true;
            magnetSound.PlayOneShot(activationSound);
            _notificationManager?.ShowNotification(
                "Наведитесь на добычу и удерживайте кнопку для притягивания");
        }

        private void OnUseCanceled(InputAction.CallbackContext context)
        {
            if (!_isActive) return;
            
            _isActive = false;
            animator.SetBool(pullAnimParameter, false);
            magnetSound.Stop();
            SetBeamsActive(false);
        }

        public void Init(Transform origin, List<ItemParameter> itemParameters) 
        {
            _origin = origin;
            transform.rotation = _origin.rotation * Quaternion.Euler(-11f, -90f, 96f);
            _notificationManager ??= FindObjectOfType<NotificationManager>();
            SetBeamsActive(false);
        }

        private void OnDrawGizmosSelected() 
        {
            if (_origin != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(_origin.position, _origin.position + _origin.forward * maxDistance);
            }
        }
    }
}