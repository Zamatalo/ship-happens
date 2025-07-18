using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct InventoryItem : INetworkSerializable
{
    public string itemName;
    public int quantity;
    
    public InventoryItem(string itemName, int quantity)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            itemName = string.Empty;
            //throw new ArgumentException("Item name cannot be null or empty")    
        }

        ;
        if (quantity <= 0)
        {
           // quantity = 1;
            Debug.Log("quant must be positive");
            //throw new ArgumentException("Quantity must be positive");

        }

        this.itemName = itemName;
        this.quantity = quantity;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref quantity);
        
        if (serializer.IsReader)
        {
            if (string.IsNullOrEmpty(itemName) || quantity <= 0)
            {
                itemName = "";
                quantity = 0;
            }
        }
    }
}
