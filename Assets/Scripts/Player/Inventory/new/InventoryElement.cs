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
    public ItemData inventoryItem;
    public string ItemName { get; private set; }
    

    public InventoryElement()
    {
        CreateIconAndQuantity();
        ApplyElementType();
    }

    private void ApplyElementType()
    {
        ClearClassList();  // очищаем все предыдущие классы

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
            Debug.LogWarning($"Trying to set invalid item: {(item.IsEmpty ? "NULL" : item.itemName)}");
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

        inventoryItem = itemData;
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
        Icon.image = null;
        ItemName = null;
        inventoryItem=null;
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