using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIController : MonoBehaviour
{
    public List<InventoryItem> InventoryItems = new List<InventoryItem>();

    private VisualElement m_Root;
    private VisualElement m_SlotContainer;

    private void Awake()
    {
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        m_SlotContainer = m_Root.Query("SlotContainer");

        
        for (int i = 0; i < 48; i++)
        {
            InventoryItem item = new InventoryItem();

            InventoryItems.Add(item);
            m_SlotContainer.Add(item);
        }
        
        GameController.OnInventoryChanged += GameController_OnInventoryChanged;
    }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.P)) 
    //     {
    //         var emptySlot = InventoryItems.FirstOrDefault(x => x.ItemId.Equals(""));
    //                     
    //         if (emptySlot != null)
    //         {
    //             emptySlot.HoldItem(GameController.GetItemById("0"));
    //         }
    //     }
    // }
    private void GameController_OnInventoryChanged(string[] itemGuid, InventoryChangeType change)
    {
        //Loop through each item and if it has been picked up, add it to the next empty slot
        foreach (string item in itemGuid)
        {
            if (change == InventoryChangeType.Pickup)
            {
                var emptySlot = InventoryItems.FirstOrDefault(x => x.ItemId.Equals(""));
                        
                if (emptySlot != null)
                {
                    emptySlot.HoldItem(GameController.GetItemById(item));
                }
            }
        }
    }
}

