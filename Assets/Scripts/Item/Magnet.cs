using UnityEngine;

namespace Item {
    public class Magnet : MonoBehaviour, IUsableItem 
    {
        [SerializeField] private float pullSpeed = 25f;
        [SerializeField] private float maxDistance = 100;
        [SerializeField] private float grabDistance = 2f;
        
        [SerializeField] private AudioSource magnetSound;
        
        private Transform _origin;
        private Loot.Loot _currentLoot;
        private bool _isActive;

        public void Init(Transform origin) 
        {
            _origin = origin;
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
        }

        private void FixedUpdate() 
        {
            if (!_isActive || !_origin) return;

            if (_currentLoot)
            {
                float distance = Vector3.Distance(_currentLoot.transform.position, _origin.position);
                
                if (distance <= grabDistance)
                {
                    _currentLoot = null;
                    return;
                }

                _currentLoot.transform.position = Vector3.MoveTowards(
                    _currentLoot.transform.position,
                    _origin.position,
                    pullSpeed * Time.fixedDeltaTime
                );
                return;
            }

            // Find new loot to pull
            if (Physics.Raycast(_origin.position, _origin.forward, out var hit, maxDistance))
            {
                if (hit.collider.TryGetComponent(out _currentLoot))
                {
                    Debug.Log($"Pulling loot: {_currentLoot.name}");
                }
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