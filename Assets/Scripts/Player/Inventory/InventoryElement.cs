using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class InventoryElement : VisualElement
{
    public VisualElement _background;
    public Image _icon;
    public Label _quantity;
    public string itemName { get; private set; }
    public Sprite Sprite { get; private set; }
    
    public InventoryElement()
    {
        _icon = new Image();
        _icon.AddToClassList("slotIcon");
        Add(_icon);

        _quantity = new Label();
        _quantity.AddToClassList("slotQuantity");
        Add(_quantity);
        
        
        AddToClassList("inventoryElement");
        AddToClassList("slotBackground");
        AddToClassList("slotBackground2");
    }
    
    public void SetItem(InventoryItem item, Sprite sprite)
    {
        
        
        if (item.itemName != null && sprite != null)
        {
            _icon.image = sprite.texture;
            itemName = item.itemName;
            Sprite = sprite;
            _icon.style.display = DisplayStyle.Flex;
        }
        else
        {
            _icon.image = null;
            itemName = null;
            Sprite = null;
            _icon.style.display = DisplayStyle.None;
        }
        
        _quantity.text = item.quantity >=1 
            ? item.quantity.ToString()
            : "";
        _quantity.style.display = string.IsNullOrEmpty(_quantity.text) 
            ? DisplayStyle.None 
            : DisplayStyle.Flex;
    }

    public void ClearItem()
    {
        _icon.image = null;
        itemName = null;
        Sprite = null;
        _icon.style.display = DisplayStyle.None;
        _quantity.text = "";
        _quantity.style.display = DisplayStyle.None;
    }
}