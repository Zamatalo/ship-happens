using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProgressManager : MonoBehaviour {
	public enum State {
		Prologue,
		Mars,
		Venus,
		Mercury,
		Epilogue
	}

	public List<GameObject> milestoneObjects;
	[HideInInspector] public State state;
	private int _i = 1;
	private LineRenderer _lr;
	private List<Milestone> _milestones;

	public void Start() {
		_lr = GetComponent<LineRenderer>();

		_milestones = new List<Milestone>();
		foreach (var obj in milestoneObjects) {
			var milestone = obj.GetComponent<Milestone>();
			milestone.AddEnterListener(() => OnMilestoneEnter(milestone));
			_milestones.Add(milestone);
		}

		_lr.SetPosition(0, _milestones[0].transform.position);
		_lr.SetPosition(1, _milestones[1].transform.position);
	}

	private void OnMilestoneEnter(Milestone milestone) {
		if (milestone != _milestones[_i]) return;

		_i += 1;

		state = state switch {
			State.Prologue => State.Mars,
			State.Mars => State.Venus,
			State.Venus => State.Mercury,
			State.Mercury => State.Epilogue,
			_ => throw new ArgumentOutOfRangeException()
		};

		var inventory = NetworkManager.Singleton.LocalClient.PlayerObject
			.GetComponentInChildren<PlayerInventoryNew>();

		switch (state) {
			case State.Mars:
				inventory.AddItemServerRpc("Spear", 1, null);
				inventory.AddItemServerRpc("OxygenGenerator", 1, null);
				break;
			case State.Venus:
				inventory.AddItemServerRpc("HarpoonGun", 1, null);
				inventory.AddItemServerRpc("GravityGun", 1, null);
				break;
			case State.Mercury:
				inventory.AddItemServerRpc("Jetpack", 1, null);
				break;
		}

		_lr.SetPosition(0, milestone.transform.position);
		_lr.SetPosition(1, _milestones[_i].transform.position);
	}
}