using System;
using UnityEngine;
using UnityEngine.UIElements;
/*
 * UI ELEMENT
 */
[UxmlElement]
public partial class InventoryElement : VisualElement
{
    [UxmlAttribute]
    public InventoryElementType ElementType
    {
        get => _elementType;
        set
        {
            if (_elementType != value)
            {
                _elementType = value;
                ApplyElementType();
            }
        }
    }
    private InventoryElementType _elementType = InventoryElementType.Inventory;
    
    public Image Icon;
    public Label Quantity;
    public InventoryItem InventoryItem;
    public string ItemName { get; private set; }
    

    public InventoryElement()
    {
        CreateIconAndQuantity();
        ApplyElementType();
    }

    private void ApplyElementType()
    {
        ClearClassList();

        switch (ElementType)
        {
            case InventoryElementType.Inventory:
                AddToClassList("inventoryElement");
                AddToClassList("slotBackground");
                AddToClassList("slotBackground2");
                break;
            case InventoryElementType.Hotbar:
                AddToClassList("hotbar-slot");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CreateIconAndQuantity()
    {
        Icon = new Image();
        Icon.AddToClassList("slotIcon");
        Add(Icon);

        Quantity = new Label();
        Quantity.AddToClassList("slotQuantity");
        Add(Quantity);
    }

    public void SetItem(InventoryItem item)
    {
        if (item.IsEmpty || string.IsNullOrEmpty(item.itemName) || item.quantity <= 0)
        {
            ClearItem();
            return;
        }

        var itemData = InventoryDatabase.Instance.GetItemByName(item.itemName);
        if (itemData == null)
        {
            Debug.LogError($"Item data not found for: {item.itemName}");
            ClearItem();
            return;
        }

        if (itemData.icon == null)
        {
            Debug.LogError($"No icon assigned for item: {item.itemName}");
            ClearItem();
            return;
        }

        InventoryItem = item;
        Icon.image = itemData.icon.texture;
        ItemName = item.itemName;
        Icon.style.display = DisplayStyle.Flex;

        Quantity.text = item.quantity >= 1 ? item.quantity.ToString() : "";
        Quantity.style.display = string.IsNullOrEmpty(Quantity.text) 
            ? DisplayStyle.None 
            : DisplayStyle.Flex;
    }

    public void ClearItem()
    {
        InventoryItem = null;
        Icon.image = null;
        ItemName = null;
        Icon.style.display = DisplayStyle.None;
        Quantity.text = "";
        Quantity.style.display = DisplayStyle.None;
    }
}

public enum InventoryElementType
{
    Inventory,
    Hotbar
}