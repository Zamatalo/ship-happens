using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public List<InventorySlot> inventory = new();

    public NetworkVariable<FixedString512Bytes> inventoryJson = new NetworkVariable<FixedString512Bytes>();

    [ServerRpc]
    public void AddItemServerRpc(string itemName, int amount)
    {
        var index = inventory.FindIndex(s => s.itemName == itemName);
        if (index >= 0)
        {
            var slot = inventory[index];
            slot.quantity += amount;
            inventory[index] = slot;
        }
        else
        {
            var a = new InventorySlot { itemName = itemName, quantity = amount };
            inventory.Add(a);
        }

        SyncInventory();
    }

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

    private void OnInventoryUpdated(FixedString512Bytes oldVal, FixedString512Bytes newVal)
    {
        string jsonString = newVal.ToString(); // Можно сделать более надёжно, если нужно
        var wrapper = JsonUtility.FromJson<InventoryWrapper>(jsonString);
        inventory = wrapper.slots;
        InventoryUI.Instance?.Refresh(inventory);
    }

    [System.Serializable]
    public class InventoryWrapper
    {
        public List<InventorySlot> slots;
    }
}
