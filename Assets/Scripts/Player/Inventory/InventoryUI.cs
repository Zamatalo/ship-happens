using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

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

        ghostIcon.style.position = Position.Absolute;
        ghostIcon.style.display = DisplayStyle.None;

        InitializeSlots();
        RegisterEvents();
    }

    private void InitializeSlots()
    {
        slots.Clear();
        slots = slotsContainer.Query<InventoryElement>().ToList();

        foreach (var slot in slots)
        {
            slot.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }
    }

    private void RegisterEvents()
    {
        root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        if (isDragging)
            return;

        var target = evt.target as VisualElement;

        while (target != null && !(target is InventoryElement))
            target = target.parent;

        if (target is InventoryElement slot && !string.IsNullOrEmpty(slot.itemName))
        {
            Debug.Log($"Start dragging item {slot.itemName}");
            dragSource = slot;
            isDragging = true;

            ghostIcon.style.backgroundImage = new StyleBackground(slot.Sprite);
            ghostIcon.style.display = DisplayStyle.Flex;
            UpdateGhostIconPosition(evt.position);
            evt.StopPropagation();
        }
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!isDragging) return;
        UpdateGhostIconPosition(evt.position);
    }

    private void UpdateGhostIconPosition(Vector2 screenPosition)
    {
        Vector2 localPos = root.WorldToLocal(screenPosition);
        ghostIcon.style.left = localPos.x - ghostIcon.layout.width / 2;
        ghostIcon.style.top = localPos.y - ghostIcon.layout.height / 2;
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!isDragging) return;

        var slotUnderCursor = slots.FirstOrDefault(s => s.worldBound.Contains(evt.position));

        if (slotUnderCursor != null && dragSource != null)
        {
            Debug.Log($"Dropped item {dragSource.itemName} to slot");
            var tempName = slotUnderCursor.itemName;
            var tempQty = slotUnderCursor._quantity;
            var tempSprite = slotUnderCursor.Sprite;
            
            
            slotUnderCursor.SetItem(new InventoryItem(dragSource.itemName,1), dragSource.Sprite);
            dragSource.SetItem(new InventoryItem("as", 1),InventoryDatabase.Instance.GetItemByName("Magnet").icon);
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
