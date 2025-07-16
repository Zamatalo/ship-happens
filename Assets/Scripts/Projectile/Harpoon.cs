using UnityEngine;
using UnityEngine.InputSystem;

namespace Projectile {
    public class Harpoon : MonoBehaviour {
        private Rigidbody _rb;
        private Transform _initPos;
        private Vector3 _dir;
        private float _force;
        private LineRenderer _lr;
        private Harpoon _currentHarpoon;
        private InputSystem_Actions _ia;
        public void Init(Vector3 dir, float force, Transform initPos) {
            _dir = dir;
            _force = force;
            _initPos = initPos;
        }

        private void Start() {
            _rb = GetComponent<Rigidbody>();
            _lr = GetComponent<LineRenderer>();
            
            _rb.AddForce(_dir * _force);
                
            
        }

        private void Update() {
            _lr.SetPosition(0, _initPos.position);
            _lr.SetPosition(1, transform.position);
        }
        
    }
}