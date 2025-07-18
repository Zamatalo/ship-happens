using System;
using UnityEngine;
using UnityEngine.UIElements;

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
    public string ItemName { get; private set; }
    public Sprite Sprite { get; private set; }

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

    public void SetItem(InventoryItem item, Sprite sprite)
    {
        if (item.itemName == null || sprite == null || sprite.texture == null)
        {
            ClearItem();
            return;
        }

        Icon.image = sprite.texture;
        ItemName = item.itemName;
        Sprite = sprite;
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
        Sprite = null;
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