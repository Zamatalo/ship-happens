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
        _background = new VisualElement();
        _background.AddToClassList("slotBackground");
        Add(_background);
        
        _icon = new Image
        {
            name = "icon",
            image = null,
        };
        _icon.AddToClassList("slotIcon");
        Add(_icon);
        
        _quantity = new Label
        {
            name = "quantity",
            text = "",
        };
        _quantity.AddToClassList("slotQuantity");
        Add(_quantity);
        
        AddToClassList("inventoryElement");
        style.width = 64;
        style.height = 64;
    }
    
    public void SetItem(InventoryItem item, Sprite sprite)
    {
        itemName = item.itemName;
        Sprite = sprite;
        
        if (item.itemName != null)
        {
            _icon.image = sprite.texture;
            _icon.style.display = DisplayStyle.Flex;
        }
        else
        {
            _icon.image = null;
            _icon.style.display = DisplayStyle.None;
        }
        
        _quantity.text = item.quantity > 1 ? item.quantity.ToString() : "";
        _quantity.style.display = string.IsNullOrEmpty(_quantity.text) 
            ? DisplayStyle.None 
            : DisplayStyle.Flex;
    }

    public void ClearItem()
    {
        _icon.image = null;
        _icon.style.display = DisplayStyle.None;
        _quantity.text = "";
        _quantity.style.display = DisplayStyle.None;
    }
}