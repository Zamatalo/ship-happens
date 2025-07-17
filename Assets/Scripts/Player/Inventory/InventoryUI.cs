using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public VisualTreeAsset slotTemplate;
    private VisualElement slotsContainer;
    private bool _isDragging = false;
    
    private void Awake()
    {
        Instance = this;
        var root = GetComponent<UIDocument>().rootVisualElement;
        slotsContainer = root.Q<VisualElement>("slots-container");
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
            icon.sprite = item.icon;
            quantity.text = slotData.quantity.ToString();

            MakeDraggable(slot, icon.sprite);

            slotsContainer.Add(slot);
        }
    }

    private void MakeDraggable(VisualElement element, Sprite sprite)
    {
        Vector2 startMousePos = Vector2.zero;

        element.RegisterCallback<PointerDownEvent>(evt =>
        {
            startMousePos = evt.position;
            _isDragging = true;
        });

        element.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (!_isDragging) return;
            
            var delta = (Vector2)evt.position - startMousePos;
            element.style.left = delta.x;
            element.style.top = delta.y;
            element.style.position = Position.Absolute;
        });

    }
}