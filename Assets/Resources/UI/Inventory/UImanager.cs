using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class UIManager : MonoBehaviour
{
    private VisualElement inventoryRoot;
    private VisualElement craftingContainer;
    private VisualElement ghostIcon;
    private Button craftingButton;
    
    private bool isCraftingVisible;
    private PlayerInventoryNew inventory;
    private bool wasCraftingVisibleBeforeClose;

    private void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        
        inventoryRoot = root.Q<VisualElement>("inventory-root");
        craftingContainer = root.Q<VisualElement>("crafting-container");
        ghostIcon = root.Q<VisualElement>("GhostIcon");
        craftingButton = root.Q<Button>("crafting-button");
        inventory = FindFirstObjectByType<PlayerInventoryNew>();

        inventoryRoot.style.display = DisplayStyle.None;
        craftingContainer.style.display = DisplayStyle.None;
        isCraftingVisible = false;
        wasCraftingVisibleBeforeClose = false;

        if (craftingButton != null)
        {
            craftingButton.RegisterCallback<ClickEvent>(OnCraftingButtonClicked);
        }
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

    private void OnDisable()
    {
        if (craftingButton != null)
        {
            craftingButton.UnregisterCallback<ClickEvent>(OnCraftingButtonClicked);
        }
    }
}