using System;
using Unity.Collections;
using Unity.Netcode;

[Serializable]
public struct InventorySlot : INetworkSerializable
{
    public string itemName;
    public int quantity;
    
    public InventorySlot(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref quantity);
    }
}