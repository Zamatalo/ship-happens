using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventoryNew : NetworkBehaviour
{
    public List<InventoryItem> inventory = new();
    // public event Action<InventoryItem> OnItemAdded;
    // public event Action<InventoryItem> OnItemRemoved;
    public NetworkVariable<FixedString4096Bytes> inventoryJson = new();

    [ServerRpc]
    public void AddItemServerRpc(string itemName, int amount, ItemParameter[] parameter)
    {
        if (string.IsNullOrEmpty(itemName) || amount <= 0) return;
        var maxStack = InventoryDatabase.Instance.GetItemByName(itemName).maxStack;
        
        
        var index = inventory.FindIndex(s => s.itemName == itemName);
        if (index >= 0){
            var slot = inventory[index];
            
            slot.quantity += amount;
            if (slot.quantity > maxStack) return;
            
            inventory[index] = slot;
        }
        else{
            var a = new InventoryItem
            {   
                id = Guid.NewGuid().ToString(),
                itemName = itemName,
                quantity = amount,
                parameters = new List<ItemParameter>(parameter ?? Array.Empty<ItemParameter>())
            };
            inventory.Add(a);
        }
        SyncInventory();
    }
    
    public InventoryItem FindItemByID(string id)
    {
        return inventory.FirstOrDefault(s => s.id == id);

    }
    [ServerRpc]
    public void RemoveItemServerRpc(string itemName, int amount)
    {
        var index = inventory.FindIndex(s => s.itemName == itemName);
        if (index >= 0)
        {
            var slot = inventory[index];
            slot.quantity -= amount;
            
            if (slot.quantity <= 0)
            {
                inventory.RemoveAt(index);
            }
            else
            {
                inventory[index] = slot;
            }
            
            SyncInventory();
        }
    }
    
    public void SyncInventory()
    {
        var itemsList = inventory.ToArray().Where(item => !item.IsEmpty).ToList();

        var wrapper = new InventoryWrapper { slots = itemsList };
        string jsonString = JsonUtility.ToJson(wrapper);

        if (IsServer)
        {
            inventoryJson.Value = new FixedString4096Bytes(jsonString);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            inventoryJson.OnValueChanged += OnInventoryUpdated;
    }
    
    public override void OnNetworkDespawn()
    {
        if (IsOwner)
            inventoryJson.OnValueChanged -= OnInventoryUpdated;
    }

    private void OnInventoryUpdated(FixedString4096Bytes oldVal, FixedString4096Bytes newVal)
    {
        string jsonString = newVal.ToString(); 
        var wrapper = JsonUtility.FromJson<InventoryWrapper>(jsonString);
        inventory = wrapper.slots;

        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.RefreshInventory(inventory);
        }
    }


    [Serializable]
    public class InventoryWrapper
    {
        public List<InventoryItem> slots;
    }
}
