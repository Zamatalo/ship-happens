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

		ghostIcon.style.backgroundImage = new StyleBackground(slot.inventoryItem.icon);
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
		// var slotUnderCursor =
		// 	slots.FirstOrDefault(s => s.worldBound.Contains(evt.position))!=null
		// 	?slots.FirstOrDefault(s => s.worldBound.Contains(evt.position))
		// 	:hotbarSlots.FirstOrDefault(e => e.worldBound.Contains(evt.position));
		
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

			// int slotType = slotUnderCursor.ElementType == InventoryElementType.Hotbar
			// 	? hotbarSlots.FindIndex(slot => slot.ItemName == targetName) //if hotbar
			// 	: 0; // if inventory
			
						
			slotUnderCursor.SetItem(
				new InventoryItem(dragSource.ItemName, int.Parse(dragSource.Quantity.text))
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

		foreach (var t in slots) t.ClearItem();

		foreach (var item in items)
		{
			if (item.IsEmpty) continue;

			var item1 = item;
			var existingHotbarSlot = hotbarSlots.FirstOrDefault(e => e.ItemName == item1.itemName);
			if (existingHotbarSlot != null)
			{
				existingHotbarSlot.Quantity.text = item.quantity.ToString();
			}
			else
			{
				var emptySlot = slots.FirstOrDefault(s => s.IsEmpty());
				emptySlot?.SetItem(item);
			}
		}
	}


	private void ToggleInventory() {
		if (inventoryRoot == null) return;

		if (inventoryRoot.style.display == DisplayStyle.None) {
			inventoryRoot.style.display = DisplayStyle.Flex;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Debug.Log("Inventory opened");
		} else {
			inventoryRoot.style.display = DisplayStyle.None;
			Cursor.lockState = CursorLockMode.Locked;
			ghostIcon.style.display = DisplayStyle.None;
			Cursor.visible = false;
			Debug.Log("Inventory closed");
		}
	}

	private void EquipItemFromSlot(InventoryElement slot) {
		if (currentHeldItem) {
			Destroy(currentHeldItem);
			currentHeldItem = null;
		}

		if (slot == null || !slot.inventoryItem ||
		    string.IsNullOrEmpty(slot.inventoryItem.itemName))
			return;

		currentHeldItem = Instantiate(
			slot.inventoryItem.itemPrefab,
			handTransform.position,
			handTransform.rotation,
			handTransform
		);

		// Initialize the item if it's usable
		var usableItem = currentHeldItem.GetComponent<IUsableItem>();
		if (usableItem != null) usableItem.Init(handTransform);
	}
}