using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct InventoryItem : INetworkSerializable, IEquatable<InventoryItem>
{
    public string id;
    public string itemName;
    public int quantity;
    public List<ItemParameter> parameters;

    public bool IsEmpty => string.IsNullOrEmpty(itemName) || quantity <= 0; 
    
    public InventoryItem(string itemName, int quantity, List<ItemParameter> parameters)
    {
        id = Guid.NewGuid().ToString();
        this.itemName = string.IsNullOrEmpty(itemName) ? string.Empty : itemName;
        this.quantity = Mathf.Max(0, quantity);
        this.parameters = parameters ?? new List<ItemParameter>();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref id);
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref quantity);

        int count = parameters?.Count ?? 0;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            // //COZ SERIALIZER DONT CALL CONSTRUCTOR, AND  FILLS FIELDS DIRECTLY ASDASDASDADa 
            // if (string.IsNullOrEmpty(id))
            // {
            //     id = Guid.NewGuid().ToString();
            // }

            parameters = new List<ItemParameter>(count);
            for (int i = 0; i < count; i++)
            {
                ItemParameter param = default;
                param.NetworkSerialize(serializer);
                parameters.Add(param);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                parameters[i].NetworkSerialize(serializer);
            }
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

    public static readonly InventoryItem Empty = new InventoryItem("", 0, new List<ItemParameter>());
}
[Serializable]
public struct ItemParameter : INetworkSerializable
{
    public string key;
    public string value;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref key);
        serializer.SerializeValue(ref value);
    }
}

