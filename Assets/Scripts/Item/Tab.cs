using TMPro;
using UnityEngine;

public class Tab : MonoBehaviour {
	public TMP_Text text;
	private ProgressManager.State _lastState;

	private ProgressManager _pm;

	private void Start() {
		_pm = FindAnyObjectByType<ProgressManager>();
		_lastState = _pm.state;
		UpdateText(_pm.state);
	}

	private void Update() {
		if (_pm.state != _lastState) {
			_lastState = _pm.state;
			UpdateText(_pm.state);
		}
	}

	private void UpdateText(ProgressManager.State state) {
		switch (state) {
			case ProgressManager.State.Prologue:
				text.text =
					@"The Drift\n\nYour ship is lost — scattered somewhere across the void. You're stranded on a fragile space raft, drifting between stars — but not helpless.\n\nSpawn and collect loot using your magnet to build new blocks onto your raft. Use this growing platform to survive the drift and reach the next planet.\n\nThere, you’ll find your next critical item — a tool that might bring you one step closer to reclaiming your ship.";
				break;
			case ProgressManager.State.Mars:
				text.text =
					@"Mars\n\nAnchor yourself with the harpoon.\n\nSharks are circling — silent, sudden. One wrong move, and they'll strike.\n\nRecover what you can, and keep moving.";
				break;
			case ProgressManager.State.Venus:
				text.text =
					@"Venus\n\nHeat and pressure are tearing everything apart.\n\nUse the gravity gun to clear the path. Reach the oxygen generator before your suit gives out.\n\nEvery second counts.";
				break;
			case ProgressManager.State.Mercury:
				text.text =
					@"Mercury\n\nYou have the jetpack now.\nOne last leap stands between you and your ship.\nYou're almost home.";
				break;
			case ProgressManager.State.Epilogue:
				text.text =
					@"Epilogue\n\nYou made it back to the ship.\nThe journey is over.\nIt’s time to go home.e";
				break;
			default:
				text.text = "";
				break;
		}
	}
}