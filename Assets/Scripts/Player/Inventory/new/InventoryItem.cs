using System;
using Unity.Netcode;
using UnityEngine;
/*
 
 */
[Serializable]
public struct InventoryItem : INetworkSerializable, IEquatable<InventoryItem>
{
    public string itemName;
    public int quantity;
    
    public bool IsEmpty => string.IsNullOrEmpty(itemName) || quantity <= 0;
    
    public InventoryItem(string itemName, int quantity)
    {
        this.itemName = string.IsNullOrEmpty(itemName) ? string.Empty : itemName;
        this.quantity = Mathf.Max(0, quantity);
    }

  
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref quantity);
        
        if (serializer.IsReader && IsEmpty)
        {
            this = default; 
        }
    }
    
    public bool Equals(InventoryItem other)
    {
        return itemName == other.itemName && quantity == other.quantity;
    }
    
    public override string ToString()
    {
        return IsEmpty ? "[Empty]" : $"{itemName} (x{quantity})";
    }
    
    public static readonly InventoryItem Empty = new InventoryItem("", 0);
}