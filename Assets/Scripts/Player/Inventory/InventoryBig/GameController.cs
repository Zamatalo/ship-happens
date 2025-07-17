using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemDetails
{
    public string Name;
    public string id;
    public Sprite Icon;
    public bool CanDrop;
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
    Metal
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
            Icon = IconSprites.FirstOrDefault(x => x.name.Equals("Magnet")),
            CanDrop = true
        });

        m_ItemDatabase.Add("1", new ItemDetails()
        {
            Name = "Harpoon",
            id = "1",
            Icon = IconSprites.FirstOrDefault(x => x.name.Equals("Harpoon")),
            CanDrop = true
        });

        m_ItemDatabase.Add("2", new ItemDetails()
        {
            Name = "Metal",
            id = "2",
            Icon = IconSprites.FirstOrDefault(x => x.name.Equals("Metal")),
            CanDrop = true
        });
        m_ItemDatabase.Add("3", new ItemDetails()
        {
            Name = "Poison",
            id = "3",
            Icon = IconSprites.FirstOrDefault(x => x.name.Equals("poison")),
            CanDrop = true
        });
        m_ItemDatabase.Add("3", new ItemDetails()
        {
            Name = "GravityGun",
            id = "4",
            Icon = IconSprites.FirstOrDefault(x => x.name.Equals("Gravity_Gun")),
            CanDrop = true
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