using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Item {
	public class Tablet : MonoBehaviour, IUsableItem {
		public AudioClip prologueSound;
		public AudioClip marsSound;
		public AudioClip venusSound;
		public AudioClip mercurySound;
		public AudioClip epilogueSound;
		public TMP_Text text;
		private AudioSource _audio;
		private ProgressManager.State _lastState;
		private ProgressManager _pm;

		private void Start() {
			_audio = GetComponent<AudioSource>();
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

		public void Init(Transform cameraTransform, List<ItemParameter> itemParameters) { }

		private void UpdateText(ProgressManager.State state) {
			switch (state) {
				case ProgressManager.State.Prologue:
					_audio.PlayOneShot(prologueSound);
					text.text =
						@"The Drift\n\nPilot, you have fallen on a barend asteroid. Weakness will be your undoing. Anchor yourself. The shadows hunt. Failure is not an option. You will return... or be lost in a void forever. Move! Now!";
					break;
				case ProgressManager.State.Mars:
					_audio.PlayOneShot(marsSound);
					text.text =
						@"Mars\n\nAnchor yourself with the harpoon. The sharks are circling — silent, sudden and hungry. One wrong move and they'll strike. Recover what you must, and keep moving.";
					break;
				case ProgressManager.State.Venus:
					_audio.PlayOneShot(venusSound);
					text.text =
						@"Venus\n\nHeat and pressure are tearing everything apart. Use the gravity gun to clear the path. Reach the oxygen generator before your suit gives out. Every second counts.";
					break;
				case ProgressManager.State.Mercury:
					_audio.PlayOneShot(mercurySound);
					text.text =
						@"Mercury\n\nYou have the jetpack now. One final leap stands between you and the ship. No room for error. You have almost made it.";
					break;
				case ProgressManager.State.Epilogue:
					_audio.PlayOneShot(epilogueSound);
					text.text =
						@"Epilogue\n\nYou have returned to the ship. The drive has ended. Now... you will face what comes next. Home is no sanctuary. It is your destiny.";
					break;
				default:
					text.text = "";
					break;
			}
		}
	}
}