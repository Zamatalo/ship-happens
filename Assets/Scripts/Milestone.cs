using System.Collections.Generic;
using Player;
using UnityEngine;

public class Milestone : MonoBehaviour {
	public delegate void OnMilestoneEnter();
	private readonly List<OnMilestoneEnter> _fns = new();

	private void OnTriggerEnter(Collider other) {
		if (!other.TryGetComponent(out PlayerController _)) return;

		foreach (var fn in _fns) fn.Invoke();
	}

	public void AddEnterListener(OnMilestoneEnter fn) {
		_fns.Add(fn);
	}
}