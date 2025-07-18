using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public List<InventoryItem> inventory = new();
   // public event Action<InventoryItem> OnItemAdded;
  //  public event Action<InventoryItem> OnItemRemoved;
    public NetworkVariable<FixedString512Bytes> inventoryJson = new();

    [ServerRpc]
    public void AddItemServerRpc(string itemName, int amount)
    {
        if (string.IsNullOrEmpty(itemName) || amount <= 0) return;
        
        var index = inventory.FindIndex(s => s.itemName == itemName);
        if (index >= 0)
        {
            var slot = inventory[index];
            slot.quantity += amount;
            inventory[index] = slot;
        }
        else
        {
            var a = new InventoryItem { itemName = itemName, quantity = amount};
            inventory.Add(a);
        }

        SyncInventory();
    }
        
    // [ServerRpc]
    // public void RemoveItemServerRpc(string itemName, int amount)
    // {
    //     var index = inventory.FindIndex(s => s.itemName == itemName);
    //     if (index >= 0)
    //     {
    //         var slot = inventory[index];
    //         slot.quantity -= amount;
    //         
    //         if (slot.quantity <= 0)
    //         {
    //             inventory.RemoveAt(index);
    //             OnItemRemoved?.Invoke(slot);
    //         }
    //         else
    //         {
    //             inventory[index] = slot;
    //         }
    //         
    //         SyncInventory();
    //     }
    // }

    private void SyncInventory()
    {
        var wrapper = new InventoryWrapper { slots = inventory };
        inventoryJson.Value = new FixedString512Bytes(JsonUtility.ToJson(wrapper));
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

    

    private void OnInventoryUpdated(FixedString512Bytes oldVal, FixedString512Bytes newVal)
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
