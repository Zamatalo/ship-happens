using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

[Preserve]
[UxmlElement]
public partial class InventorySlot : VisualElement
{
    public Image Icon;
    public string ItemId = "";

    public InventorySlot()
    {
        Icon = new Image();
        Add(Icon);

        Icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");
    }

    public void HoldItem(ItemDetails item)
    {
        Icon.image = item.Icon.texture;
        ItemId = item.id;
        
    }

    public void DropItem()
    {
        ItemId = "";
        Icon.image = null;
    }
}

