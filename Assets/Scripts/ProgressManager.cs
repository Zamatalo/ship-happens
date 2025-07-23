using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour {
	public List<GameObject> milestoneObjects;
	private int _i = 1;
	private LineRenderer _lr;
	private List<Milestone> _milestones;

	private void Start() {
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

		_lr.SetPosition(0, milestone.transform.position);
		_lr.SetPosition(1, _milestones[_i].transform.position);
	}
}