using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject {
	public string itemName;
	public Sprite icon;
	public int maxStack;
	public GameObject itemPrefab;
	public GameObject collectableItemPrefab;
}


//shows that item is usable

public interface IUsableItem {
	string Name();

	void Init(Transform cameraTransform, List<ItemParameter> itemParameters);
	void Use();
	void Unuse();
}