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
    private VisualElement ghostIcon;

    private List<InventoryElement> slots = new List<InventoryElement>();

    private bool isDragging = false;
    private InventoryElement dragSource = null;

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
        ghostIcon = root.Q<VisualElement>("GhostIcon");
        ghostIcon.style.width = 64;
        ghostIcon.style.height = 64;
        ghostIcon.style.position = Position.Absolute;
        ghostIcon.style.display = DisplayStyle.None;

        
        
        InitializeSlots();
        root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    private void InitializeSlots()
    {
        slots.Clear();
        slots = slotsContainer.Query<InventoryElement>()
            .ToList();

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

        if (target is not InventoryElement slot || string.IsNullOrEmpty(slot.itemName)) return;
        
        Debug.Log($"Start dragging item {slot.itemName}");
        dragSource = slot;
        isDragging = true;

        ghostIcon.style.backgroundImage = new StyleBackground(slot.Sprite);
        ghostIcon.style.display = DisplayStyle.Flex;
        UpdateGhostIconPosition(evt.position);
        evt.StopPropagation();
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!isDragging) return;
        UpdateGhostIconPosition(evt.position);
    }

    private void UpdateGhostIconPosition(Vector2 screenPosition)
    {
        var localPos = root.WorldToLocal(screenPosition);
    
        var offsetX = ghostIcon.resolvedStyle.width / 2;
        var offsetY = ghostIcon.resolvedStyle.height / 2;
    
        ghostIcon.style.left = localPos.x - offsetX;
        ghostIcon.style.top = localPos.y - offsetY;
    }


    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!isDragging) return;

        var slotUnderCursor = slots.FirstOrDefault(s => s.worldBound.Contains(evt.position));

        if (slotUnderCursor != null && dragSource != null)
        {
            Debug.Log($"Dropped item {dragSource.itemName} to slot");
            var tempName = string.IsNullOrEmpty(slotUnderCursor.itemName)
                ? string.Empty
                : slotUnderCursor.itemName;
            
            var tempQty = string.IsNullOrEmpty(slotUnderCursor._quantity.text)
                ? 0
                : int.Parse(slotUnderCursor._quantity.text);
                
            var tempSprite = slotUnderCursor.Sprite;
            
            
            slotUnderCursor.SetItem(new InventoryItem(dragSource.itemName,int.Parse(dragSource._quantity.text)), dragSource.Sprite);
            dragSource.SetItem(new InventoryItem(tempName,tempQty),tempSprite);
        }
        else
        {
            Debug.Log("Dropped outside slots, returning item back");
        }

        ghostIcon.style.display = DisplayStyle.None;
        isDragging = false;
        dragSource = null;
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
