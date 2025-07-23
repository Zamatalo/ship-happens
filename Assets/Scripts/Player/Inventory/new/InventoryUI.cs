using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class InventoryUI : MonoBehaviour
{
	public static InventoryUI Instance;
	private readonly List<InventoryElement> hotbarSlots = new();
	private PlayerInventoryNew inventory;
	
	private GameObject currentHeldItem;
	private InventoryElement dragSource;
	private VisualElement ghostIcon;
	private VisualElement hotbarContainer;
	private VisualElement inventoryRoot;

	private bool isDragging;
	private VisualElement root;
	private InventoryElement selectedHotbarSlot;

	private List<InventoryElement> slots = new();

	private VisualElement slotsContainer;
	private InventoryElement tempTarget;

	/**/
	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		inventory = FindFirstObjectByType<PlayerInventoryNew>();
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
		if (isDragging) UpdateGhostIconPosition(Input.mousePosition);

		for (var i = 1; i <= 5; i++)
			if (Input.GetKeyDown(i.ToString()))
				SelectHotbarSlot(i - 1);
	}

	private void SelectHotbarSlot(int hotbarIndex) {
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
		hotbarSlots.Clear();

		slots = slotsContainer.Query<InventoryElement>().ToList();
		hotbarSlots.AddRange(hotbarContainer.Query<InventoryElement>().ToList());

		Debug.Log($"Total Inventory Slots initialized: {slots.Count}");
		for (var i = 0; i < slots.Count; i++) {
			Debug.Log($"Inventory Slot {i}: {slots[i].name}");
			slots[i].RegisterCallback<PointerDownEvent>(OnPointerDown);
		}

		Debug.Log($"Total Hotbar Slots initialized: {hotbarSlots.Count}");
		for (var i = 0; i < hotbarSlots.Count; i++) {
			Debug.Log($"Hotbar Slot {i}: {hotbarSlots[i].name}");
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
				new InventoryItem(dragSource.ItemName, int.Parse(dragSource.Quantity.text),
					new List<ItemParameter>())
			);

			dragSource.SetItem(new InventoryItem(targetName, targetQty, new List<ItemParameter>()));
		}
		ghostIcon.style.display = DisplayStyle.None;
		isDragging = false;
		dragSource = null;
	}

	public void RefreshInventory(List<InventoryItem> items) {
		if (slots == null || slots.Count == 0 || items == null) return;
		var allSlots =  new List<InventoryElement>();
		allSlots.AddRange(slots);
		allSlots.AddRange(hotbarSlots);
		
		var currentItemNames = items
			.Where(i => !i.IsEmpty)
			.Select(i => i.itemName)
			.ToHashSet();



		foreach (var item in items) {
			if (item.IsEmpty) continue;

			var existingSlot = allSlots.FirstOrDefault(s => s.ItemName == item.itemName);
			existingSlot?.SetItem(item);
		}

		foreach (var item in items) {
			if (item.IsEmpty) continue;

			if (allSlots.Any(s => s.ItemName == item.itemName)) continue;

			var emptySlot = allSlots.FirstOrDefault(s =>
				s.InventoryItem.IsEmpty && s.ElementType == InventoryElementType.Inventory);

			emptySlot?.SetItem(item);
		}
		foreach (var slot in allSlots
			         .Where(slot => !currentItemNames.Contains(slot.ItemName)))
		{
			slot.ClearItem();
		}
	}
	

	private void EquipItemFromSlot(InventoryElement slot) {
		if (!inventory) return;
        
		inventory.EquipItem(slot.ItemName);
	}
}