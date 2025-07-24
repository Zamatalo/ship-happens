using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Inventory/recipeData")]
public class RecipeData : ScriptableObject {
	public List<ItemQuantity> requiredItems = new List<ItemQuantity>();
	public ItemData outputItem;
}
[System.Serializable]
public class ItemQuantity {
	public ItemData item;
	public int quantity;
}