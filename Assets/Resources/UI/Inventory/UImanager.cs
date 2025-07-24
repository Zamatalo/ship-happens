using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class UIManager : MonoBehaviour
{
    private VisualElement inventoryRoot;
    private VisualElement recipesCraftingContainer;
    private VisualElement ghostIcon;
    private ListView list;
    private Button craftingButton;
    private VisualElement craftingContainer;
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
        recipesCraftingContainer = root.Q<VisualElement>("recipesCraftingContainer");
        inventory = FindFirstObjectByType<PlayerInventoryNew>();

        inventoryRoot.style.display = DisplayStyle.None;
        craftingContainer.style.display = DisplayStyle.None;
        isCraftingVisible = false;
        wasCraftingVisibleBeforeClose = false;

        craftingButton?.RegisterCallback<ClickEvent>(OnCraftingButtonClicked);
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
            var container = new VisualElement();
            container.AddToClassList("crafting-icon");
        
            var icon = new Image
            {
                name = "icon",
                style =
                {
                    width = 96,
                    height = 96,
                }
            };
            icon.AddToClassList("recipeIcon");
        
            container.Add(icon);
            return container;
        };

        list.bindItem = (element, i) =>
        {   
            var recipe = recipeList[i];
            var icon = element.Q<Image>("icon");
        
            if (recipe != null && recipe.outputItem != null && recipe.outputItem.icon != null)
            {
                icon.image = recipe.outputItem.icon.texture;
            }
            else
            {
                Debug.LogWarning($"Missing recipe data at index {i}");
                icon.image = null;
            }
        };
        list.selectionChanged += (selectedItems) =>
        {
            recipesCraftingContainer.Clear();
        
            foreach (RecipeData selectedRecipe in selectedItems)
            {
                if (selectedRecipe == null || selectedRecipe.requiredItems == null) continue;
                
                foreach (var requiredItem in selectedRecipe.requiredItems)
                {
                    if (requiredItem.item == null) continue;
                        
                    var itemContainer = new VisualElement();
                    itemContainer.AddToClassList("recipesCraftingContainer");
                        
                    var icon = new Image
                    {
                        image = requiredItem.item.icon?.texture,
                        style =
                        {
                            width = 96,
                            height = 96
                        }
                    };
                        
                    var label = new Label($"{requiredItem.quantity}x {requiredItem.item.itemName}");
                   // var hasItem = inventory.HasItem(requiredItem.item, requiredItem.quantity);
                   
                 //   label.AddToClassList(hasItem ? "ingredient-available" : "ingredient-missing");
                    label.AddToClassList("ingredient-label");
                        
                    itemContainer.Add(icon);
                    itemContainer.Add(label);
                    recipesCraftingContainer.Add(itemContainer);
                }
            }
        };
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