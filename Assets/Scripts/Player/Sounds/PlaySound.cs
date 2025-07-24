using UnityEngine;

namespace Player.Sounds {
	public class PlaySound : MonoBehaviour {
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private AudioClip[] footstepSound;
		[SerializeField] private AudioClip[] jumpSounds;
		[SerializeField] private AudioClip[] shootSounds_Harpoon;
		[SerializeField] private AudioClip[] reloadSounds_Harpoon;
		[SerializeField] private AudioClip spearAttackSound;


		public void PlayFootstep() {
			if (footstepSound.Length > 0) {
				var index = Random.Range(0, footstepSound.Length);
				audioSource.PlayOneShot(footstepSound[index]);
			}
		}

		public void PlayJump() {
			if (jumpSounds.Length > 0) {
				var index = Random.Range(0, jumpSounds.Length);
				audioSource.PlayOneShot(jumpSounds[index]);
			}
		}

		public void PlayShoot_Harpoon() {
			if (shootSounds_Harpoon.Length > 0) {
				var index = Random.Range(0, shootSounds_Harpoon.Length);
				audioSource.PlayOneShot(shootSounds_Harpoon[index]);
			}
		}

		public void PlayReload_Harpoon() {
			if (reloadSounds_Harpoon.Length > 0) {
				var index = Random.Range(0, reloadSounds_Harpoon.Length);
				audioSource.PlayOneShot(reloadSounds_Harpoon[index]);
			}
		}

		public void PlaySpearAttackSound() {
			audioSource.PlayOneShot(spearAttackSound);
		}
	}
}