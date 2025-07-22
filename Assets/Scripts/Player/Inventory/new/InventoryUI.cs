using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class InventoryUI : MonoBehaviour {
	public static InventoryUI Instance;
	public Transform handTransform;
	private InputSystem_Actions _ia;

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
	/*TEMP VALUES*/
	// private string tempItemName;
	// private int tempItemQuantity;
	// private Sprite tempItemSprite;
	// private Image tempItemImage;
	private InventoryElement tempTarget;

	/**/
	private void Awake() {
		_ia = new InputSystem_Actions();
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
		if (isDragging) UpdateGhostIconPosition(Input.mousePosition);
	}

	private void OnEnable() {
		_ia.Enable();
		_ia.Inventory._1.performed += _1;
		_ia.Inventory._2.performed += _2;
		_ia.Inventory._3.performed += _3;
		_ia.Inventory._4.performed += _4;
		_ia.Inventory._5.performed += _5;
		_ia.Inventory.Toggle.performed += Toggle;
		//_ia.Inventory.Iter.performed += Iter;
		//_ia.Inventory.DropItem.performed += DropItem;
	}

	private void OnDisable() {
		_ia.Inventory._1.performed -= _1;
		_ia.Inventory._2.performed -= _2;
		_ia.Inventory._3.performed -= _3;
		_ia.Inventory._4.performed -= _4;
		_ia.Inventory._5.performed -= _5;
		_ia.Inventory.Toggle.performed -= Toggle;
		//_ia.Inventory.Iter.performed -= Iter;
		//_ia.Inventory.DropItem.performed -= DropItem;
		_ia.Disable();
	}

	private void _1(InputAction.CallbackContext _) {
		OnActiveSlotChange(0);
	}

	private void _2(InputAction.CallbackContext _) {
		OnActiveSlotChange(1);
	}

	private void _3(InputAction.CallbackContext _) {
		OnActiveSlotChange(2);
	}

	private void _4(InputAction.CallbackContext _) {
		OnActiveSlotChange(3);
	}

	private void _5(InputAction.CallbackContext _) {
		OnActiveSlotChange(4);
	}

	private void OnActiveSlotChange(int index) {
		var hotbarSlots = slots
			.Where(s => s.ElementType == InventoryElementType.Hotbar)
			.ToList();

		if (index < 0 || index >= hotbarSlots.Count)
			return;

		selectedHotbarSlot?.RemoveFromClassList("selected");

		selectedHotbarSlot = hotbarSlots[index];
		selectedHotbarSlot.AddToClassList("selected");
		EquipItemFromSlot(selectedHotbarSlot);
		Debug.Log($"Selected hotbar slot {index + 1}: {selectedHotbarSlot.ItemName}");
	}

	private void InitializeSlots() {
		slots.Clear();
		slots = slotsContainer.Query<InventoryElement>().ToList();
		slots.AddRange(hotbarContainer.Query<InventoryElement>().ToList());

		Debug.Log($"Total slots initialized: {slots.Count}");
		for (var i = 0; i < slots.Count; i++) {
			Debug.Log($"Slot {i}: {slots[i].name}");
			slots[i].RegisterCallback<PointerDownEvent>(OnPointerDown);
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

		var slotUnderCursor = slots
			.FirstOrDefault(s => s.worldBound.Contains(evt.position));

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
				new InventoryItem(dragSource.ItemName, int.Parse(dragSource.Quantity.text))
			);

			dragSource.SetItem(new InventoryItem(targetName, targetQty));
		}
		ghostIcon.style.display = DisplayStyle.None;
		isDragging = false;
		dragSource = null;
	}

	public void RefreshInventory(List<InventoryItem> items) {
		if (slots == null || slots.Count == 0 || items == null) return;

		foreach (var t in slots) t.ClearItem();

		for (var i = 0; i < items.Count && i < slots.Count; i++)
			if (!items[i].IsEmpty) {
				slots[i].SetItem(items[i]);
				Debug.Log($"Added {items[i].itemName} to slot {i}");
			} else {
				Debug.LogWarning($"Skipping empty item at index {i}");
			}
	}

	private void Toggle(InputAction.CallbackContext _) {
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