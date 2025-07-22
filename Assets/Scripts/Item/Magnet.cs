using UnityEngine;

namespace Item {
    public class Magnet : MonoBehaviour, IUsableItem 
    {
        public Transform notActualOrigin;
        [SerializeField] private float pullSpeed = 25f;
        [SerializeField] private float maxDistance = 1000000f;
        [SerializeField] private float grabDistance = 2f;
        [SerializeField] private LineRenderer beamLine;
        [SerializeField] private AudioSource magnetSound;
        
        private Transform _origin;
        private Loot.Loot _currentLoot;
        private bool _isActive;

        public void Init(Transform origin) 
        {
            _origin = origin;
            transform.rotation = _origin.rotation * Quaternion.Euler(-11f, -90f, 96f);
            beamLine.enabled = false; // Скрыть лазер при старте
        }

        public void Use()
        {
            if (_isActive) return;
            _isActive = true;
            magnetSound?.Play();
        }

        public void Unuse()
        {
            if (!_isActive) return;
            _isActive = false;
            magnetSound?.Stop();
            beamLine.enabled = false;
        }

        private void FixedUpdate() 
        {
            if (!_isActive || !_origin) {
                beamLine.enabled = false;
                return;
            }

            bool hitSomething = Physics.Raycast(_origin.position, _origin.forward, out RaycastHit laserHit, maxDistance);

            if (_currentLoot)
            {
                float distance = Vector3.Distance(_currentLoot.transform.position, _origin.position);
                
                if (distance <= grabDistance)
                {
                    _currentLoot = null;
                    beamLine.enabled = false;
                    return;
                }

                _currentLoot.transform.position = Vector3.MoveTowards(
                    _currentLoot.transform.position,
                    _origin.position,
                    pullSpeed * Time.fixedDeltaTime
                );
            }
            else if (hitSomething)
            {
                if (laserHit.collider.TryGetComponent(out _currentLoot))
                {
                    Debug.Log($"Pulling loot: {_currentLoot.name}");
                }
            }

            // === ЛАЗЕР ===
            if (_currentLoot || hitSomething)
            {
                beamLine.enabled = true;
                beamLine.SetPosition(0, notActualOrigin.position);
                beamLine.SetPosition(1, _currentLoot ? _currentLoot.transform.position : laserHit.point);
            }
            else
            {
                beamLine.enabled = false;
            }
        }

        private void OnDisable() 
        {
            Unuse(); 
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
