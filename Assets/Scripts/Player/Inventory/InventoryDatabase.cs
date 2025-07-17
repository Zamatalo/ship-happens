using System.Collections.Generic;
using UnityEngine;

public class InventoryDatabase : MonoBehaviour
{
    public static InventoryDatabase Instance;
    public List<ItemData> items;

    private void Awake()
    {
        Instance = this;
        populateItems();
    }

    private void populateItems()
    {
        items = new List<ItemData>(Resources.LoadAll<ItemData>("Items"));
        Debug.Log($"Loaded {items.Count} items into InventoryDatabase.");
    }

    public ItemData GetItemByName(string name)
    {
        return items.Find(i => i.itemName == name);
    }
}