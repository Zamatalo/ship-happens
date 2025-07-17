using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepSound;
    public AudioClip[] jumpSounds;
    

    public void PlayFootstep()
    {
        if (footstepSound.Length > 0)
        {
            int index = Random.Range(0, footstepSound.Length);
            audioSource.PlayOneShot(footstepSound[index]);
        }
    }
    public void PlayJump()
    {
        if (jumpSounds.Length > 0)
        {
            int index = Random.Range(0, jumpSounds.Length);
            audioSource.PlayOneShot(jumpSounds[index]);
        }
    }
}