using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject))]
public class CollectableItem : NetworkBehaviour 
{
    [Header("Settings")]
    [SerializeField] private string itemName;
    [SerializeField] private List<ItemParameter> parameters = new();

    private Rigidbody _rb;
    private bool _initialized;
    private bool _canBeCollected = false;

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    public void Init(string name, List<ItemParameter> parameters) 
    {
        itemName = name;
        this.parameters = parameters ?? new List<ItemParameter>();
        _initialized = true;
        StartCoroutine(EnableCollectionAfterDelay());
    }

    private IEnumerator EnableCollectionAfterDelay()
    {
        _canBeCollected = false;
        yield return new WaitForSeconds(3f);
        _canBeCollected = true;
    }

    public List<ItemParameter> GetParameters()
    {
        return !_canBeCollected ? null : parameters;
    }

    public string GetItemName()
    {
        return itemName;
    }
}