using System.Collections.Generic;
using Player;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class UIManager : NetworkBehaviour {
	public List<RecipeData> recipeList;
	private readonly Dictionary<string, InventoryItem> inventoryCache = new();
	private Button craftingButton;
	private VisualElement craftingContainer;
	private RecipeData currentlySelectedRecipe;
	private Button dynamicCraftButton;
	private VisualElement ghostIcon;
	private PlayerInventoryNew inventory;
	private VisualElement inventoryRoot;

	private bool isCraftingVisible;
	private ListView list;

	private PlayerController playerController;
	private VisualElement recipesCraftingContainer;
	private bool wasCraftingVisibleBeforeClose;

	private void Awake() {
		InitializeUI();
		LoadRecipes();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Tab)) ToggleInventory();
	}

	private void OnDisable() {
		if (craftingButton != null)
			craftingButton.UnregisterCallback<ClickEvent>(OnCraftingButtonClicked);

		if (dynamicCraftButton != null)
			dynamicCraftButton.UnregisterCallback<ClickEvent>(OnCraftingButtonConformationClicked);

		if (inventory != null && IsOwner)
			inventory.inventoryJson.OnValueChanged -= OnInventoryUpdated;
	}

	private void InitializeUI() {
		var uiDocument = GetComponent<UIDocument>();
		var root = uiDocument.rootVisualElement;

		inventoryRoot = root.Q<VisualElement>("inventory-root");
		craftingContainer = root.Q<VisualElement>("crafting-container");
		ghostIcon = root.Q<VisualElement>("GhostIcon");
		craftingButton = root.Q<Button>("crafting-button");
		list = root.Q<ListView>("recipesList");
		recipesCraftingContainer = root.Q<VisualElement>("recipesCraftingContainer");

		inventoryRoot.style.display = DisplayStyle.None;
		craftingContainer.style.display = DisplayStyle.None;

		craftingButton?.RegisterCallback<ClickEvent>(OnCraftingButtonClicked);

		dynamicCraftButton = new Button { text = "Craft", name = "ConfirmCraft" };
		dynamicCraftButton.AddToClassList("ConfirmCraft");
		dynamicCraftButton.RegisterCallback<ClickEvent>(OnCraftingButtonConformationClicked);
	}

	private void LoadRecipes() {
		recipeList = new List<RecipeData> {
			Resources.Load<RecipeData>("Items/Recipe/Platform"),
			Resources.Load<RecipeData>("Items/Recipe/Magnet")
		};
		InitializeRecipeList();
	}

	protected override void OnNetworkPostSpawn() {
		if (!IsOwner || NetworkManager.Singleton == null ||
		    NetworkManager.Singleton.LocalClient == null)
			return;

		var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
		if (playerObject == null) return;

		inventory = playerObject.GetComponentInChildren<PlayerInventoryNew>(true);
		if (inventory == null) return;
		playerController = playerObject.GetComponentInChildren<PlayerController>();
		if (inventory.inventoryJson != null)
			inventory.inventoryJson.OnValueChanged += OnInventoryUpdated;

		UpdateInventoryCache();
	}

	public override void OnNetworkDespawn() {
		if (inventory != null && inventory.inventoryJson != null)
			inventory.inventoryJson.OnValueChanged -= OnInventoryUpdated;
	}

	public void LoadRecipe(string name) {
		recipeList.Add(Resources.Load<RecipeData>($"Items/Recipe/{name}"));
		InitializeRecipeList();
	}

	private void InitializeRecipeList() {
		list.itemsSource = recipeList;
		list.makeItem = () => {
			var cont = new VisualElement();
			cont.AddToClassList("recipeList");

			var img = new Image();
			img.name = "recipeIcon";
			img.AddToClassList("recipeIcon");
			cont.Add(img);

			var label = new Label { name = "recipeLabel" };
			label.AddToClassList("ingredient-label");
			cont.Add(label);
			return cont;
		};

		list.bindItem = (element, i) => {
			var recipe = recipeList[i];
			var icon = element.Q<Image>("recipeIcon");
			icon.image = recipe?.outputItem?.icon?.texture;

			var label = element.Q<Label>("recipeLabel");
			label.text = recipe?.outputItem?.name;
		};

		list.selectionChanged += OnRecipeSelected;
		list.Rebuild();
	}

	private void OnRecipeSelected(IEnumerable<object> selectedItems) {
		recipesCraftingContainer.Clear();

		if (dynamicCraftButton.parent != null) dynamicCraftButton.RemoveFromHierarchy();

		var allIngredientsAvailable = true;
		UpdateInventoryCache();

		foreach (RecipeData selectedRecipe in selectedItems) {
			if (selectedRecipe == null) continue;

			currentlySelectedRecipe = selectedRecipe;
			allIngredientsAvailable = RenderRecipeIngredients(selectedRecipe);
		}

		if (allIngredientsAvailable) recipesCraftingContainer.Add(dynamicCraftButton);
	}

	private bool RenderRecipeIngredients(RecipeData recipe) {
		if (recipe.requiredItems == null) return false;

		var allAvailable = true;

		foreach (var requiredItem in recipe.requiredItems) {
			if (requiredItem.item == null) continue;

			var itemContainer = new VisualElement();
			itemContainer.AddToClassList("crafting-icon");

			var icon = new Image {
				image = requiredItem.item.icon?.texture
			};
			// icon.AddToClassList("crafting-icon");
			var label = new Label($"{requiredItem.quantity}x {requiredItem.item.itemName}");

			var hasEnough = CheckInventory(requiredItem.item.itemName, requiredItem.quantity);
			label.EnableInClassList("ingredient-available", hasEnough);
			label.EnableInClassList("ingredient-missing", !hasEnough);
			label.AddToClassList("ingredient-label");

			if (!hasEnough) allAvailable = false;
			itemContainer.Add(icon);
			itemContainer.Add(label);
			recipesCraftingContainer.Add(itemContainer);
		}

		return allAvailable;
	}

	private void UpdateInventoryCache() {
		if (!inventory) return;

		inventoryCache.Clear();
		foreach (var item in inventory.inventory)
			if (!item.IsEmpty)
				inventoryCache[item.itemName] = item;
	}

	public bool CheckInventory(string itemName, int requiredQuantity) {
		return inventoryCache.TryGetValue(itemName, out var item)
		       && !item.IsEmpty
		       && item.quantity >= requiredQuantity;
	}

	private void OnInventoryUpdated(FixedString4096Bytes previousValue,
		FixedString4096Bytes newValue) {
		UpdateInventoryCache();

		if (currentlySelectedRecipe == null) return;
		list.selectedIndex = -1;
		list.selectedIndex = recipeList.IndexOf(currentlySelectedRecipe);
	}

	private void ToggleInventory() {
		if (inventoryRoot == null) return;

		var isInventoryVisible = inventoryRoot.style.display == DisplayStyle.Flex;

		if (isInventoryVisible) {
			wasCraftingVisibleBeforeClose = isCraftingVisible;
			inventoryRoot.style.display = DisplayStyle.None;
			craftingContainer.style.display = DisplayStyle.None;
			Cursor.lockState = CursorLockMode.Locked;
			playerController.SetLookEnabled(true);
			ghostIcon.style.display = DisplayStyle.None;
			Cursor.visible = false;
		} else {
			inventoryRoot.style.display = DisplayStyle.Flex;
			craftingContainer.style.display = wasCraftingVisibleBeforeClose
				? DisplayStyle.Flex
				: DisplayStyle.None;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			playerController.SetLookEnabled(false);
			UpdateInventoryCache();
		}
	}

	private void OnCraftingButtonClicked(ClickEvent evt) {
		ToggleCrafting();
		evt.StopPropagation();
	}

	private void ToggleCrafting() {
		if (craftingContainer == null) return;

		isCraftingVisible = !isCraftingVisible;
		craftingContainer.style.display = isCraftingVisible
			? DisplayStyle.Flex
			: DisplayStyle.None;
	}

	private void OnCraftingButtonConformationClicked(ClickEvent evt) {
		if (currentlySelectedRecipe == null || inventory == null) return;

		foreach (var requiredItem in currentlySelectedRecipe.requiredItems)
			inventory.RemoveItemServerRpc(requiredItem.item.itemName, requiredItem.quantity);

		inventory.AddItemServerRpc(
			currentlySelectedRecipe.outputItem.itemName,
			1,
			null
		);
	}
}