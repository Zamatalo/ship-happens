using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class UIManager : MonoBehaviour
{
    private VisualElement inventoryRoot;
    private VisualElement craftingContainer;
    private VisualElement ghostIcon;
    private ListView list;
    private Button craftingButton;
    
    private bool isCraftingVisible;
    private PlayerInventoryNew inventory;
    private bool wasCraftingVisibleBeforeClose;
    public List<RecipeData> recipeList;
    
    private void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        
        recipeList = new List<RecipeData>(Resources.LoadAll<RecipeData>("Items/Recipe"));
        
        inventoryRoot = root.Q<VisualElement>("inventory-root");
        craftingContainer = root.Q<VisualElement>("crafting-container");
        ghostIcon = root.Q<VisualElement>("GhostIcon");
        craftingButton = root.Q<Button>("crafting-button");
        list = root.Q<ListView>("recipesList");
        inventory = FindFirstObjectByType<PlayerInventoryNew>();

        inventoryRoot.style.display = DisplayStyle.None;
        craftingContainer.style.display = DisplayStyle.None;
        isCraftingVisible = false;
        wasCraftingVisibleBeforeClose = false;

        if (craftingButton != null)
        {
            craftingButton.RegisterCallback<ClickEvent>(OnCraftingButtonClicked);
        }
        FillRecipeList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryRoot == null) return;

        bool isInventoryVisible = inventoryRoot.style.display == DisplayStyle.Flex;
        
        if (isInventoryVisible)
        {
            wasCraftingVisibleBeforeClose = isCraftingVisible;
            inventoryRoot.style.display = DisplayStyle.None;
            craftingContainer.style.display = DisplayStyle.None; 
            Cursor.lockState = CursorLockMode.Locked;
            ghostIcon.style.display = DisplayStyle.None;
            Cursor.visible = false;
        }
        else
        {
            inventoryRoot.style.display = DisplayStyle.Flex;
            craftingContainer.style.display = wasCraftingVisibleBeforeClose 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnCraftingButtonClicked(ClickEvent evt)
    {
        ToggleCrafting();
        evt.StopPropagation();
    }

    private void ToggleCrafting()
    {
        if (craftingContainer == null) return;
        
        isCraftingVisible = !isCraftingVisible;
        craftingContainer.style.display = isCraftingVisible 
            ? DisplayStyle.Flex 
            : DisplayStyle.None;
        
        Debug.Log($"Crafting panel {(isCraftingVisible ? "shown" : "hidden")}");
    }

    void FillRecipeList()
    {
        list.itemsSource = recipeList;

        list.makeItem = () =>
        {
            var container = new VisualElement
            {
                style =
                {
                    minHeight = 96,
                    minWidth = 96,
                    flexDirection = FlexDirection.Row
                }
            };

            var icon = new Image
            {
                name = "icon",
                style =
                {
                    width = 96,
                    height = 96
                }
            };
            var label = new Label();
            label.name = "label";
            container.Add(icon);
            container.Add(label);

            return container;
        };

        list.bindItem = (element, i) =>
        {
            var recipe = recipeList[i];
            var label = element.Q<Label>("label");
            var icon = element.Q<Image>("icon");

            label.text = recipe.outputItem.itemName;
            icon.image = recipe.outputItem.icon?.texture;
        };

        // list.selectionType = SelectionType.Single;
        // list.fixedItemHeight = 96; // Optional optimization
        list.Rebuild();
    }

    

    private void OnDisable()
    {
        if (craftingButton != null)
        {
            craftingButton.UnregisterCallback<ClickEvent>(OnCraftingButtonClicked);
        }
    }
}