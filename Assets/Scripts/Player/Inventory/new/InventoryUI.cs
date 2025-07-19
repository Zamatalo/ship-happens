using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    private VisualElement root;
    private VisualElement inventoryRoot;
    private VisualElement slotsContainer;
    private VisualElement hotbarContainer;
    private VisualElement ghostIcon;

    private List<InventoryElement> slots = new();
    private InventoryElement selectedHotbarSlot;
    
    private bool isDragging;
    private InventoryElement dragSource;
    
    private GameObject currentHeldItem;
    public Transform handTransform;
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
        inventoryRoot = root.Q<VisualElement>("inventory-root");
        ghostIcon = root.Q<VisualElement>("GhostIcon");
        
        inventoryRoot.style.display = DisplayStyle.None;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        InitializeSlots();
        root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        
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
        EquipItemFromSlot(selectedHotbarSlot);
        Debug.Log($"Selected hotbar slot {hotbarIndex + 1}: {selectedHotbarSlot.ItemName}");
    }

    private void InitializeSlots()
    {
        slots.Clear();
        slots = slotsContainer.Query<InventoryElement>().ToList();
        slots.AddRange(hotbarContainer.Query<InventoryElement>().ToList());
    
        Debug.Log($"Total slots initialized: {slots.Count}");
        for (int i = 0; i < slots.Count; i++)
        {
            Debug.Log($"Slot {i}: {slots[i].name}");
            slots[i].RegisterCallback<PointerDownEvent>(OnPointerDown);
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
        
        ghostIcon.style.backgroundImage = new StyleBackground(slot.inventoryItem.icon);
        ghostIcon.style.display = DisplayStyle.Flex;
        UpdateGhostIconPosition(evt.position);
        evt.StopPropagation();
    }
    
    private void UpdateGhostIconPosition(Vector2 screenPosition)
    {
        if (root.panel == null)
            return;

        var screenPositionFlipped = new Vector2(
            screenPosition.x,
            Screen.height - screenPosition.y
        );

        var panelPosition = RuntimePanelUtils.ScreenToPanel(root.panel, screenPositionFlipped);

        Debug.Log($"ScreenPos: {screenPosition}, PanelPos: {panelPosition}");
        
        ghostIcon.style.left = panelPosition.x - 32f;
        ghostIcon.style.top = panelPosition.y - 32f;
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
            
            slotUnderCursor.SetItem(
                new InventoryItem(dragSource.ItemName,int.Parse(dragSource.Quantity.text))
                );

            dragSource.SetItem(new InventoryItem(targetName, targetQty));
        }
        ghostIcon.style.display = DisplayStyle.None;
        isDragging = false;
        dragSource = null;
    }

    public void RefreshInventory(List<InventoryItem> items)
    {
        if (slots == null || slots.Count == 0 || items == null) return;

        foreach (var t in slots)
        {
            t.ClearItem();
        }

        for (int i = 0; i < items.Count && i < slots.Count; i++)
        {
            if (!items[i].IsEmpty)
            {
                slots[i].SetItem(items[i]);
                Debug.Log($"Added {items[i].itemName} to slot {i}");
            }
            else
            {
                Debug.LogWarning($"Skipping empty item at index {i}");
            }
        }
    }
    private void ToggleInventory()
    {
        if (inventoryRoot == null) return;

        if (inventoryRoot.style.display == DisplayStyle.None)
        {
            inventoryRoot.style.display = DisplayStyle.Flex;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Inventory opened");
        }
        else
        {
            inventoryRoot.style.display = DisplayStyle.None;
            Cursor.lockState = CursorLockMode.Locked;
            ghostIcon.style.display = DisplayStyle.None;
            Cursor.visible = false;
            Debug.Log("Inventory closed");
        }
    }
    
    private void EquipItemFromSlot(InventoryElement slot)
    {
        if (currentHeldItem)
        {
            Destroy(currentHeldItem);
            currentHeldItem = null;
        }

        if (slot == null || !slot.inventoryItem|| slot.inventoryItem.itemName == null)
            return;

        currentHeldItem = Instantiate(
            slot.inventoryItem.itemPrefab,
            handTransform.position,
            handTransform.rotation,
            handTransform
        );
    }

}
