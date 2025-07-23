using System.Collections.Generic;
using Player;
using Unity.Netcode;
using UnityEngine;

public class CollectableItem2 : NetworkBehaviour
    {
        [Header("Item Settings")]
        public string itemName = "";
        public int quantity = 1;
        public List<ItemParameter> parameters = new();
        public bool canBePickedUp = true;
        public float pickupRadius = 1.5f;
        public float rotationSpeed = 50f;
        public float hoverHeight = 0.5f;
        public float hoverSpeed = 2f;

        private Vector3 startPos;
        private float initialY;
        private Collider itemCollider;
        private Rigidbody rb;

        private void Awake()
        {
            itemCollider = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
            startPos = transform.position;
            initialY = startPos.y;
        }

        private void Update()
        {
            if (!IsServer) return;

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            HoverAnimation();
        }

        private void HoverAnimation()
        {
            float newY = initialY + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer || !canBePickedUp) return;

            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    TryPickupItem(player);
                }
            }
        }

        private void TryPickupItem(PlayerController player)
        {
            if (player.IsOwner)
            {
                var inventory = player.GetComponent<PlayerInventoryNew>();
                if (inventory != null)
                {
                    ItemParameter[] parametersArray = parameters.ToArray();
                    
                    inventory.AddItemServerRpc(itemName, quantity, parametersArray);
                    GetComponent<NetworkObject>().Despawn();
                }
            }
        }
        
        // Method to add or update a parameter
        public void SetParameter(string key, string value)
        {
            var existingParam = parameters.Find(p => p.key == key);
            if (existingParam is { key: not null, value: not null })
            {
                existingParam.value = value;
            }
            else{
                parameters.Add(new ItemParameter(key, value));
            }
        }

        public string GetParameter(string key)
        {
            var param = parameters.Find(p => p.key == key);
            return param.value ?? string.Empty;
        }

        public void SetPickupable(bool canPickup, float delay = 0f)
        {
            canBePickedUp = canPickup;
            
            if (itemCollider != null)
            {
                itemCollider.enabled = canPickup;
            }
            
            if (!canPickup && delay > 0)
            {
                Invoke(nameof(EnablePickup), delay);
            }
        }

        private void EnablePickup()
        {
            SetPickupable(true);
        }

        public void ThrowItem(Vector3 direction, float force)
        {
            if (rb != null)
            {
                SetPickupable(false, 1f); 
                rb.AddForce(direction * force, ForceMode.Impulse);
            }
        }
    }
