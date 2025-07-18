using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    private VisualElement root;
    private VisualElement slotsContainer;
    private VisualElement hotbarContainer;
    private VisualElement ghostIcon;

    private List<InventoryElement> slots = new();
    private InventoryElement selectedHotbarSlot;
    
    private bool isDragging;
    private InventoryElement dragSource = null;
    
    /*TEMP VALUES*/
    // private string tempItemName;
    // private int tempItemQuantity;
    // private Sprite tempItemSprite;
    // private Image tempItemImage;
    private InventoryElement tempTarget;
    /**/
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        slotsContainer = root.Q<VisualElement>("slotsContainer");
        hotbarContainer = root.Q<VisualElement>("hotbarContainer");
        ghostIcon = root.Q<VisualElement>("GhostIcon");
        
        InitializeSlots();
        root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    private void Update()
    {
        if (isDragging)
        {
            UpdateGhostIconPosition(Input.mousePosition);
        }

        for (int i = 1; i <= 5; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                SelectHotbarSlot(i - 1);
            }
        }
    }
    
    private void SelectHotbarSlot(int hotbarIndex)
    {
        var hotbarSlots = slots
            .Where(s => s.ElementType == InventoryElementType.Hotbar)
            .ToList();
    
        if (hotbarIndex < 0 || hotbarIndex >= hotbarSlots.Count)
            return;

        selectedHotbarSlot?.RemoveFromClassList("selected");

        selectedHotbarSlot = hotbarSlots[hotbarIndex];
        selectedHotbarSlot.AddToClassList("selected");

        Debug.Log($"Selected hotbar slot {hotbarIndex + 1}: {selectedHotbarSlot.ItemName}");
    }

    private void InitializeSlots()
    {
        slots.Clear();
        //adding normal slots
        slots = slotsContainer.Query<InventoryElement>()
            .ToList();
        //adding hotbar slots
        slots.AddRange(hotbarContainer.Query<InventoryElement>().ToList());
        
        foreach (var slot in slots)
        {
            slot.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }
    }
    
    private void OnPointerDown(PointerDownEvent evt)
    {
        if (isDragging)
            return;

        var target = evt.target as VisualElement;

        while (target != null && target is not InventoryElement)
            target = target.parent;

        if (target is not InventoryElement slot || string.IsNullOrEmpty(slot.ItemName)) return;
        //setting target cell to non-visible 
        tempTarget = target as InventoryElement;
        if (tempTarget != null)
        {
            tempTarget.Icon.style.display = DisplayStyle.None;
            tempTarget.Quantity.style.display = DisplayStyle.None;
        }
        
        Debug.Log($"Start dragging item {slot.ItemName}");
        dragSource = slot;
        isDragging = true;
        
        ghostIcon.style.backgroundImage = new StyleBackground(slot.Sprite);
        ghostIcon.style.display = DisplayStyle.Flex;
        UpdateGhostIconPosition(evt.position);
        evt.StopPropagation();
    }
    
    private void UpdateGhostIconPosition(Vector2 screenPosition)
    {
        if (root.panel == null)
            return;

        Vector2 screenPositionFlipped = new Vector2(
            screenPosition.x,
            Screen.height - screenPosition.y
        );

        Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(root.panel, screenPositionFlipped);


        Debug.Log($"ScreenPos: {screenPosition}, PanelPos: {panelPosition}");

        var offsetX = ghostIcon.resolvedStyle.width / 2;
        var offsetY = ghostIcon.resolvedStyle.height / 2;

        ghostIcon.style.left = panelPosition.x - offsetX;
        ghostIcon.style.top = panelPosition.y - offsetY;
    }
    
    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!isDragging) return;

        var slotUnderCursor = slots
            .FirstOrDefault(s => s.worldBound.Contains(evt.position));
        
        //out ot bounds
        if (slotUnderCursor == null || dragSource == null)
        { 
            tempTarget.Icon.style.display = DisplayStyle.Flex;
            tempTarget.Quantity.style.display = DisplayStyle.Flex;
        }
        //same cell
        else if (slotUnderCursor == dragSource)
        {
            tempTarget.Icon.style.display = DisplayStyle.Flex;
            tempTarget.Quantity.style.display = DisplayStyle.Flex;

        }
        //moving to another cell
        else
        {
            var targetName = string.IsNullOrEmpty(slotUnderCursor.ItemName)
                ? string.Empty
                : slotUnderCursor.ItemName;

            var targetQty = string.IsNullOrEmpty(slotUnderCursor.Quantity.text)
                ? 0
                : int.Parse(slotUnderCursor.Quantity.text);
            
            var targetSprite = slotUnderCursor.Sprite;

            slotUnderCursor.SetItem(new InventoryItem(dragSource.ItemName,int.Parse(dragSource.Quantity.text)), dragSource.Sprite);

            dragSource.SetItem(new InventoryItem(targetName, targetQty), targetSprite);
        }
        ghostIcon.style.display = DisplayStyle.None;
        isDragging = false;
        dragSource = null;
        
        // if (slotUnderCursor != null && dragSource != null)
        // {
        //     Debug.Log($"Dropped item {dragSource.ItemName} to slot");
        //     var tempName = string.IsNullOrEmpty(slotUnderCursor.ItemName)
        //         ? string.Empty
        //         : slotUnderCursor.ItemName;
        //     
        //     var tempQty = string.IsNullOrEmpty(slotUnderCursor.Quantity.text)
        //         ? 0
        //         : int.Parse(slotUnderCursor.Quantity.text);
        //         
        //     var tempSprite = slotUnderCursor.Sprite;
        //     
        //     
        //     slotUnderCursor.SetItem(new InventoryItem(dragSource.ItemName,int.Parse(dragSource.Quantity.text)), dragSource.Sprite);
        //     dragSource.SetItem(new InventoryItem(tempName,tempQty),tempSprite);
        // }
        // else
        // {
        //     Debug.Log("Dropped outside slots, returning item back");
        // }
        //
        // ghostIcon.style.display = DisplayStyle.None;
        // isDragging = false;
        // dragSource = null;
    }

    public void RefreshInventory(List<InventoryItem> items)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                var item = items[i];
                var sprite = InventoryDatabase.Instance.GetItemByName(item.itemName).icon;
                slots[i].SetItem(new InventoryItem(item.itemName, item.quantity), sprite);
            }
            else
            {
                slots[i].ClearItem();
            }
        }
    }
}
