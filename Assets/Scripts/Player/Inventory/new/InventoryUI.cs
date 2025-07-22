using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class InventoryUI : MonoBehaviour {
	public static InventoryUI Instance;
	public Transform handTransform;

	private GameObject currentHeldItem;
	private InventoryElement dragSource;
	private VisualElement ghostIcon;
	private VisualElement hotbarContainer;
	private VisualElement inventoryRoot;

	private bool isDragging;
	private VisualElement root;
	private InventoryElement selectedHotbarSlot;

	private List<InventoryElement> slots = new();
	private List<InventoryElement> hotbarSlots = new();

	private VisualElement slotsContainer;
	private InventoryElement tempTarget;
	/**/
	private void Awake() {
		if (Instance != null && Instance != this) {
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

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Tab)) ToggleInventory();

		if (isDragging) UpdateGhostIconPosition(Input.mousePosition);

		for (var i = 1; i <= 5; i++)
			if (Input.GetKeyDown(i.ToString()))
				SelectHotbarSlot(i - 1);
	}

	private void SelectHotbarSlot(int hotbarIndex) {
		// var hotbarSlots = slots
		// 	.Where(s => s.ElementType == InventoryElementType.Hotbar)
		// 	.ToList();

		if (hotbarIndex < 0 || hotbarIndex >= hotbarSlots.Count)
			return;
		selectedHotbarSlot?.RemoveFromClassList("selected");

		selectedHotbarSlot = hotbarSlots[hotbarIndex];
		selectedHotbarSlot.AddToClassList("selected");
		EquipItemFromSlot(selectedHotbarSlot);
		Debug.Log($"Selected hotbar slot {hotbarIndex + 1}: {selectedHotbarSlot.ItemName}");
	}

	private void InitializeSlots() {
		slots.Clear();
		slots = slotsContainer.Query<InventoryElement>().ToList();
		hotbarSlots.AddRange(hotbarContainer.Query<InventoryElement>().ToList());

		Debug.Log($"Total slots initialized: {slots.Count}");
		for (var i = 0; i < slots.Count; i++) {
			Debug.Log($"Slot {i}: {slots[i].name}");
			slots[i].RegisterCallback<PointerDownEvent>(OnPointerDown);
		}
		Debug.Log($"Total HotbarSlots initialized: {slots.Count}");
		for (var i = 0; i < hotbarSlots.Count; i++) {
			Debug.Log($"Slot {i}: {hotbarSlots[i].name}");
			hotbarSlots[i].RegisterCallback<PointerDownEvent>(OnPointerDown);
		}
	}

	private void OnPointerDown(PointerDownEvent evt) {
		if (isDragging)
			return;

		var target = evt.target as VisualElement;

		while (target != null && target is not InventoryElement)
			target = target.parent;

		if (target is not InventoryElement slot || string.IsNullOrEmpty(slot.ItemName)) return;
		//setting target cell to non-visible 
		tempTarget = target as InventoryElement;
		if (tempTarget != null) {
			tempTarget.Icon.style.display = DisplayStyle.None;
			tempTarget.Quantity.style.display = DisplayStyle.None;
		}

		Debug.Log($"Start dragging item {slot.ItemName}");
		dragSource = slot;
		isDragging = true;

		var icon = InventoryDatabase.Instance.GetItemByName(slot.ItemName).icon;
		ghostIcon.style.backgroundImage = new StyleBackground(icon);
		ghostIcon.style.display = DisplayStyle.Flex;
		UpdateGhostIconPosition(evt.position);
		evt.StopPropagation();
	}
	
	private void UpdateGhostIconPosition(Vector2 screenPosition) {
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

	private void OnPointerUp(PointerUpEvent evt) {
		if (!isDragging) return;
		List<InventoryElement> allSlots = new List<InventoryElement>();
		allSlots.AddRange(slots);
		allSlots.AddRange(hotbarSlots);
		var slotUnderCursor = allSlots.FirstOrDefault(s => s.worldBound.Contains(evt.position));

		
		//out ot bounds
		if (slotUnderCursor == null || dragSource == null) {
			tempTarget.Icon.style.display = DisplayStyle.Flex;
			tempTarget.Quantity.style.display = DisplayStyle.Flex;
		}
		//same cell
		else if (slotUnderCursor == dragSource) {
			tempTarget.Icon.style.display = DisplayStyle.Flex;
			tempTarget.Quantity.style.display = DisplayStyle.Flex;
		}
		//moving to another cell
		else {
			var targetName = string.IsNullOrEmpty(slotUnderCursor.ItemName)
				? string.Empty
				: slotUnderCursor.ItemName;

			var targetQty = string.IsNullOrEmpty(slotUnderCursor.Quantity.text)
				? 0
				: int.Parse(slotUnderCursor.Quantity.text);
			
						
			slotUnderCursor.SetItem(
				new InventoryItem(dragSource.ItemName, int.Parse(dragSource.Quantity.text),new List<ItemParameter>())
				);

			dragSource.SetItem(new InventoryItem(targetName, targetQty,new List<ItemParameter>()));
		}
		ghostIcon.style.display = DisplayStyle.None;
		isDragging = false;
		dragSource = null;
	}

	public void RefreshInventory(List<InventoryItem> items)
	{
		if (slots == null || slots.Count == 0 || items == null) return;

		foreach (var slot in slots)
		{
			if (!hotbarSlots.Contains(slot))
			{
				slot.ClearItem();
			}
		}

		foreach (var item in items)
		{
			if (item.IsEmpty) continue;

			var existingHotbarSlot = hotbarSlots.FirstOrDefault(s => s.ItemName == item.itemName);
        
			if (existingHotbarSlot != null)
			{
				existingHotbarSlot.Quantity.text = item.quantity.ToString();
            
				if (item.quantity <= 0)
				{
					existingHotbarSlot.ClearItem();
				}
			}
		}

		foreach (var item in items)
		{
			if (item.IsEmpty) continue;

			if (hotbarSlots.Any(s => s.ItemName == item.itemName)) continue;

			var emptySlot = slots.FirstOrDefault(s => 
				s.InventoryItem.IsEmpty && !hotbarSlots.Contains(s));

			emptySlot?.SetItem(item);
		}
	}


	private void ToggleInventory() {
		if (inventoryRoot == null) return;

		if (inventoryRoot.style.display == DisplayStyle.None) {
			inventoryRoot.style.display = DisplayStyle.Flex;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		} else {
			inventoryRoot.style.display = DisplayStyle.None;
			Cursor.lockState = CursorLockMode.Locked;
			ghostIcon.style.display = DisplayStyle.None;
			Cursor.visible = false;
		}
	}

	private void EquipItemFromSlot(InventoryElement slot) {
		if (currentHeldItem) {
			Destroy(currentHeldItem);
			currentHeldItem = null;
		}
		if (slot == null || slot.InventoryItem.IsEmpty || string.IsNullOrEmpty(slot.InventoryItem.itemName))
			return;
		
		var prejab =InventoryDatabase.Instance.GetItemByName(slot.InventoryItem.itemName).itemPrefab;
		currentHeldItem = Instantiate(
			prejab,
			handTransform.position,
			handTransform.rotation,
			handTransform
		);
		var usableItem = currentHeldItem.GetComponent<IUsableItem>();
		if (usableItem != null) usableItem.Init(handTransform,null);
	}
}