using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStack;
    public GameObject itemPrefab;
}


//shows that item is usable

public interface IUsableItem
{
    void Init(Transform origin,List<ItemParameter> itemParameters);
    void Use();
    void Unuse();
}
