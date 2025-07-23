using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryNew : NetworkBehaviour
{
    [Header("Inventory")]
    public List<InventoryItem> inventory = new();
    public NetworkVariable<FixedString4096Bytes> inventoryJson = new();
    
    [Header("Held Item")]
    private GameObject currentHeldItem;
    private IUsableItem CurrentUsableItem => currentHeldItem?.GetComponent<IUsableItem>();
    [SerializeField] private Transform currentHeldBox;
    private string _currentItemName;
    
    [Header("Input")]
    private InputSystem_Actions _playerInputActions;

    private GameObject _camera;
    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _camera = GameObject.FindGameObjectWithTag("CinemachineTarget");
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            inventoryJson.OnValueChanged += OnInventoryUpdated;
            EnableInput();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            inventoryJson.OnValueChanged -= OnInventoryUpdated;
            DisableInput();
        }
    }

    private void EnableInput()
    {
        _playerInputActions.Inventory.Enable();
        _playerInputActions.Player.Enable();
        _playerInputActions.Inventory.DropItem.performed += OnDropItem;
        _playerInputActions.Player.AddAllItems.performed += OnAddAllItems;
    //    _playerInputActions.Player.UseItem.performed += OnUseItem;
    }

    private void DisableInput()
    {
        _playerInputActions.Inventory.DropItem.performed -= OnDropItem;
        _playerInputActions.Player.AddAllItems.performed -= OnAddAllItems;
        //_playerInputActions.Player.UseItem.performed -= OnUseItem;
        _playerInputActions.Player.Disable();
        _playerInputActions.Inventory.Disable();
    }

    [ServerRpc]
    public void AddItemServerRpc(string itemName, int amount, ItemParameter[] parameters)
    {
        if (string.IsNullOrEmpty(itemName)) return;
        
        var itemData = InventoryDatabase.Instance.GetItemByName(itemName);
        if (!itemData) return;

        var existingItem = inventory.FirstOrDefault(i => i.itemName == itemName);
        
        if (!existingItem.IsEmpty && !existingItem.IsEmpty)
        {
            // Stack existing item
            existingItem.quantity += amount;
            existingItem.quantity = Mathf.Min(existingItem.quantity, itemData.maxStack);
        }
        else
        {
            // Add new item
            inventory.Add(new InventoryItem
            {   
                id = Guid.NewGuid().ToString(),
                itemName = itemName,
                quantity = amount,
                parameters = new List<ItemParameter>(parameters ?? Array.Empty<ItemParameter>())
            });
        }
        
        SyncInventory();
    }

    [ServerRpc]
    public void RemoveItemServerRpc(string itemName, int amount)
    {
        var item = inventory.FirstOrDefault(i => i.itemName == itemName);
        if (item.IsEmpty) return;

        item.quantity -= amount;
        if (item.quantity <= 0)
        {
            inventory.Remove(item);
        }
        
        SyncInventory();
    }

    private void SyncInventory()
    {
        var wrapper = new InventoryWrapper { 
            slots = inventory.Where(item => !item.IsEmpty).ToList() 
        };
        inventoryJson.Value = new FixedString4096Bytes(JsonUtility.ToJson(wrapper));
    }

    private void OnInventoryUpdated(FixedString4096Bytes oldVal, FixedString4096Bytes newVal)
    {
        var wrapper = JsonUtility.FromJson<InventoryWrapper>(newVal.ToString());
        inventory = wrapper.slots;
        InventoryUI.Instance?.RefreshInventory(inventory);
    }

    public void EquipItem(string itemName, List<ItemParameter> itemParameters = null)
    {
        if (currentHeldItem)
        {
            Destroy(currentHeldItem);
            currentHeldItem = null;
        }

        if (string.IsNullOrEmpty(itemName)) return;
        
        var itemData = InventoryDatabase.Instance.GetItemByName(itemName);
        if (!itemData?.itemPrefab) return;

        currentHeldItem = Instantiate(
            itemData.itemPrefab,
            currentHeldBox.position,
            currentHeldBox.rotation,
            currentHeldBox
        );

        _currentItemName = itemName;
        CurrentUsableItem?.Init(_camera.transform, itemParameters ?? new List<ItemParameter>());
    }

    public void OnDropItem(InputAction.CallbackContext context) 
    {
        if (!IsOwner || CurrentUsableItem == null) return;

        var itemData = InventoryDatabase.Instance.GetItemByName(_currentItemName);
        if (itemData?.collectableItemPrefab == null) return;

        // Create dropped item
        var droppedItem = Instantiate(
            itemData.collectableItemPrefab,
            transform.position + transform.forward * 2f,
            transform.rotation
        );

        if (droppedItem.TryGetComponent(out CollectableItem collectable))
        {
            var currentItem = FindItemByName(_currentItemName);
            collectable.Init(_currentItemName, 
                new List<ItemParameter>(currentItem.parameters));
            
            if (droppedItem.TryGetComponent(out NetworkObject netObj))
            {
                netObj.SpawnWithOwnership(OwnerClientId);
            }
        }
        if (droppedItem.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(transform.forward * 10f + transform.up * 5f, ForceMode.Impulse);
        }


        RemoveItemServerRpc(_currentItemName, 1);
        Destroy(currentHeldItem);
        currentHeldItem = null;
    }
    
    private void OnAddAllItems(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        AddItemServerRpc("BuildingTool", 1, null);
        AddItemServerRpc("Jetpack", 1, null);
        AddItemServerRpc("Edible", 1, null);
        AddItemServerRpc("GravityGun", 1, null);
        AddItemServerRpc("HarpoonGun", 1, null);
        AddItemServerRpc("Metal", 1, null);
        AddItemServerRpc("Planks", 1, null);
        AddItemServerRpc("Poison", 1, null);
        AddItemServerRpc("Potable", 1, null);
        AddItemServerRpc("Tablet", 1, null);
    }

    public InventoryItem FindItemByName(string name) => 
        inventory.FirstOrDefault(s => s.itemName == name);

    public InventoryItem FindItemByID(string id) => 
        inventory.FirstOrDefault(s => s.id == id);

    [Serializable]
    public class InventoryWrapper
    {
        public List<InventoryItem> slots;
    }
}