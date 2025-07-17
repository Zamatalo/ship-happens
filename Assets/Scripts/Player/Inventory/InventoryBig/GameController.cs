using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemDetails
{
    public string Name;
    public string id;
    public Sprite sprite;
    public bool CanDrop;
    public bool isStackable;
}

public enum InventoryChangeType
{
    Pickup,
    Drop
}

public enum ItemEnums{
    None,
    Magnet,
    Harpoon,
    Metal,
    GravityGun
}

public delegate void OnInventoryChangedDelegate(string[] itemid, InventoryChangeType change);

public class GameController : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> IconSprites;
    private static Dictionary<string, ItemDetails> m_ItemDatabase = new Dictionary<string, ItemDetails>();
    private List<ItemDetails> m_PlayerInventory = new List<ItemDetails>();
    public static event OnInventoryChangedDelegate OnInventoryChanged = delegate { };


    private void Awake()
    {
        PopulateDatabase();
    }

    private void Start()
    {
        m_PlayerInventory.AddRange(m_ItemDatabase.Values);
        OnInventoryChanged.Invoke(m_PlayerInventory.Select(x=> x.id).ToArray(), InventoryChangeType.Pickup);
    }
    
    public void PopulateDatabase()
    {
        m_ItemDatabase.Add("0", new ItemDetails()
        {
            Name = "Magnet",
            id = "0",
            sprite = IconSprites.FirstOrDefault(x => x.name.Equals("Magnet")),
            CanDrop = true,
            isStackable = false
        });

        m_ItemDatabase.Add("1", new ItemDetails()
        {
            Name = "Harpoon",
            id = "1",
            sprite = IconSprites.FirstOrDefault(x => x.name.Equals("Harpoon")),
            CanDrop = true,
            isStackable = false
        });

        m_ItemDatabase.Add("2", new ItemDetails()
        {
            Name = "Metal",
            id = "2",
            sprite = IconSprites.FirstOrDefault(x => x.name.Equals("Metal")),
            CanDrop = true,
            isStackable = true
        });
        m_ItemDatabase.Add("3", new ItemDetails()
        {
            Name = "Poison",
            id = "3",
            sprite = IconSprites.FirstOrDefault(x => x.name.Equals("poison")),
            CanDrop = true,
            isStackable = true
        });
        m_ItemDatabase.Add("4", new ItemDetails()
        {
            Name = "GravityGun",
            id = "4",
            sprite = IconSprites.FirstOrDefault(x => x.name.Equals("gravity_gun")),
            CanDrop = true,
            isStackable = false
        });
    }
    
    public static ItemDetails GetItemById(string id)
    {
        return m_ItemDatabase.GetValueOrDefault(id);
    }
    public static ItemDetails GetItemByName(string name)
    {
        return m_ItemDatabase.Values
            .FirstOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

}