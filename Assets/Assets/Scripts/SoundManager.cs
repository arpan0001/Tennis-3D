using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // References to audio clips
    public AudioClip hitSound;  // Sound for when the ball is hit by the player or bot
    public AudioClip collisionSound;  // Sound for when the ball collides with any object

    private AudioSource audioSource;

    private void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
    }

    // Method to play hit sound after a delay
    public void PlayHitSoundWithDelay(float delay)
    {
        StartCoroutine(PlayHitSoundDelayed(delay));
    }

    private IEnumerator PlayHitSoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    // Method to play sound when the ball collides
    public void PlayCollisionSound()
    {
        if (collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }
    }
}
