using System;
using System.Collections.Generic;
using System.Linq;
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
 

    [ServerRpc]
    public void AddItemServerRpc(string itemName, int amount, ItemParameter[] parameters)
    {
        if (string.IsNullOrEmpty(itemName)) return;
    
        var itemData = InventoryDatabase.Instance.GetItemByName(itemName);
        if (!itemData) return;

        var paramList = parameters != null 
            ? new List<ItemParameter>(parameters) 
            : new List<ItemParameter>();

        var existingIndex = inventory.FindIndex(i => i.itemName == itemName);
    
        if (existingIndex == -1)
        {
            inventory.Add(new InventoryItem(
                itemName, 
                amount, 
                paramList
            ));
        }
        else
        {
            var existing = inventory[existingIndex];
            inventory[existingIndex] = new InventoryItem(
                existing.itemName,
                existing.quantity + amount,
                existing.parameters
            );
        }
    
        SyncInventory();
    }

    [ServerRpc]
    public void RemoveItemServerRpc(string itemName, int amount)
    {
        for (var i = 0; i < inventory.Count; i++)
        {
            var item = inventory[i];
            if (item.itemName != itemName) continue;
            item.quantity -= amount;

            if (item.quantity <= 0)
            {
                inventory.RemoveAt(i);
                _currentItemName = null;
                Destroy(currentHeldItem);
                currentHeldItem = null;
            }

            SyncInventory();
            return;
        }
    }


    private void SyncInventory()
    {
        var wrapper = new InventoryWrapper { 
            slots = inventory.Where(item => !item.IsEmpty).ToList() 
        };
        inventoryJson.Value = new FixedString4096Bytes(JsonUtility.ToJson(wrapper));
        InventoryUI.Instance?.RefreshInventory(inventory);
    }

    private void OnInventoryUpdated(FixedString4096Bytes oldVal, FixedString4096Bytes newVal)
    {
        var wrapper = JsonUtility
            .FromJson<InventoryWrapper>(newVal.ToString());
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

        var droppedItem = Instantiate(
            itemData.collectableItemPrefab,
            currentHeldBox.position,
            currentHeldBox.rotation
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


        RemoveItemServerRpc(_currentItemName,1);
        _currentItemName = null;
        Destroy(currentHeldItem);
        currentHeldItem = null;
    }
    
    private void OnAddAllItems(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        AddItemServerRpc("BuildingTool", 1, null);
        AddItemServerRpc("Jetpack", 1, null);
        AddItemServerRpc("Edible", 3, null);
        AddItemServerRpc("GravityGun", 1, null);
        AddItemServerRpc("HarpoonGun", 1, null);
        AddItemServerRpc("Metal", 4, null);
        AddItemServerRpc("Planks", 5, null);
        AddItemServerRpc("Poison", 6, null);
        AddItemServerRpc("Potable", 7, null);
        AddItemServerRpc("Tablet", 1, null);
    }

    public InventoryItem FindItemByName(string name) => 
        inventory.FirstOrDefault(s => s.itemName == name);

    public InventoryItem FindItemByID(string id) => 
        inventory.FirstOrDefault(s => s.id == id);
    private void EnableInput()
    {
        _playerInputActions.Inventory.Enable();
        _playerInputActions.Player.Enable();
        _playerInputActions.Inventory.DropItem.performed += OnDropItem;
        _playerInputActions.Player.AddAllItems.performed += OnAddAllItems;
    }

    private void DisableInput()
    {
        _playerInputActions.Inventory.DropItem.performed -= OnDropItem;
        _playerInputActions.Player.AddAllItems.performed -= OnAddAllItems;
        _playerInputActions.Player.Disable();
        _playerInputActions.Inventory.Disable();
    }
    [Serializable]
    public class InventoryWrapper
    {
        public List<InventoryItem> slots;
    }
}