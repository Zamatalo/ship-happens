using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public VisualTreeAsset slotTemplate;
    private VisualElement slotsContainer;
    private VisualElement hotbarContainer;
    private List<VisualElement> hotbarSlots = new();
    private int selectedHotbarIndex = -1;
    private bool _isDragging = false;

    private void Awake()
    {
        Instance = this;
        var root = GetComponent<UIDocument>().rootVisualElement;
        slotsContainer = root.Q<VisualElement>("slots-container");
        hotbarContainer = root.Q<VisualElement>("hotbarContainer");

        // Clear existing slots and create new ones
        hotbarContainer.Clear();
        hotbarSlots.Clear();

        for (int i = 0; i < 5; i++)
        {
            var slotHotbar = new VisualElement();
            slotHotbar.AddToClassList("hotbar-slot");
            
            int index = i;
            slotHotbar.RegisterCallback<PointerDownEvent>(_ => SelectHotbarSlot(index));
            hotbarContainer.Add(slotHotbar);
            hotbarSlots.Add(slotHotbar);
        }
    }

    private void Update()
    {
        if (hotbarSlots.Count == 0)
            return;

        for (int i = 1; i <= hotbarSlots.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                SelectHotbarSlot(i - 1);
            }
        }
    }

    public void Refresh(List<InventorySlot> slots)
    {
        slotsContainer.Clear();

        foreach (var slotData in slots)
        {   
            var slot = slotTemplate.CloneTree();
            var icon = slot.Q<Image>("icon");
            var quantity = slot.Q<Label>("quantity");

            var item = InventoryDatabase.Instance.GetItemByName(slotData.itemName);
            if (item != null) // Added null check
            {
                icon.sprite = item.icon;
                quantity.text = slotData.quantity.ToString();
                MakeDraggable(slot, icon.sprite);
            }
            slotsContainer.Add(slot);
        }
    }

    private void SelectHotbarSlot(int index)
    {
        if (selectedHotbarIndex >= 0 && selectedHotbarIndex < hotbarSlots.Count)
            hotbarSlots[selectedHotbarIndex].RemoveFromClassList("selected");

        selectedHotbarIndex = index;
        if (selectedHotbarIndex >= 0 && selectedHotbarIndex < hotbarSlots.Count)
            hotbarSlots[selectedHotbarIndex].AddToClassList("selected");

        Debug.Log($"Selected hotbar slot: {index}");
    }

    private void MakeDraggable(VisualElement element, Sprite sprite)
    {
        Vector2 startMousePos = Vector2.zero;
        Vector2 startElementPos = Vector2.zero;
        
        element.RegisterCallback<PointerDownEvent>(evt =>
        {
            startMousePos = evt.position;
            startElementPos = new Vector2(element.resolvedStyle.left, element.resolvedStyle.top);
            _isDragging = true;
        });

        element.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (!_isDragging) return;
            
            var delta = (Vector2)evt.position - startMousePos;
            element.style.left = startElementPos.x + delta.x;
            element.style.top = startElementPos.y + delta.y;
            element.style.position = Position.Absolute;
        });

        element.RegisterCallback<PointerUpEvent>(evt =>
        {
            _isDragging = false;
        });
    }
}