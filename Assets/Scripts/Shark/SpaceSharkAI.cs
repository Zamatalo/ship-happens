using System.Collections;
using UnityEngine;

namespace Shark
{
    public class SpaceSharkAI : MonoBehaviour
    {
        public float orbitDistance = 15f;     
        public float orbitSpeed = 20f;        
        public float attackSpeed = 50f;       
        public float attackCooldown = 60f;    

        private GameObject _player;
        private float _attackTimer;
        private bool _isAttacking = false;
        private Vector3 _orbitOffset;

        private enum State { Patrolling, Attacking, Returning }
        private State _currentState = State.Patrolling;

        private Vector3 _returnPoint;

        void Start()
        {
            StartCoroutine(PollPlayer());
        
            _attackTimer = attackCooldown;
        }

        void Update()
        {
            if (_player == null) return;

            switch (_currentState)
            {
                case State.Patrolling:
                    Patrol();
                    break;

                case State.Attacking:
                    Attack();
                    break;

                case State.Returning:
                    ReturnToOrbit();
                    break;
            }
        }
    
        private IEnumerator PollPlayer() {
            while (true) {
                _player = GameObject.FindGameObjectWithTag("Player");
                if (_player == null) {
                    yield return new WaitForSeconds(2f);
                    continue;
                }

                break;
            }

            _orbitOffset = (transform.position - _player.transform.position).normalized * orbitDistance;
        }

        void Patrol()
        {
            _attackTimer -= Time.deltaTime;

        
            _orbitOffset = Quaternion.Euler(0, orbitSpeed * Time.deltaTime, 0) * _orbitOffset;
            transform.position = _player.transform.position + _orbitOffset;

            LookAtPlayer();

            if (_attackTimer <= 0f)
            {
                _currentState = State.Attacking;
            }
        }

        void Attack()
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            transform.position += direction * attackSpeed * Time.deltaTime;

            LookAtPlayer();

            if (Vector3.Distance(transform.position, _player.transform.position) < 1.5f)
            {
            

                _returnPoint = _player.transform.position + _orbitOffset.normalized * orbitDistance;
                _currentState = State.Returning;
            }
        }

        void ReturnToOrbit()
        {
            Vector3 direction = (_returnPoint - transform.position).normalized;
            transform.position += direction * orbitSpeed * Time.deltaTime;

            LookAtPlayer();

            if (Vector3.Distance(transform.position, _returnPoint) < 1f)
            {
                _attackTimer = attackCooldown;
                _currentState = State.Patrolling;
            }
        }

        void LookAtPlayer()
        {
            Vector3 dir = (_player.transform.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }
    }
}
