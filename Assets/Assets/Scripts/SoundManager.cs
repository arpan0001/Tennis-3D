using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    
    public AudioClip hitSound;  
    public AudioClip collisionSound;  

    private AudioSource audioSource;

    private void Start()
    {
       
        audioSource = GetComponent<AudioSource>();
    }

    
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

    
    public void PlayCollisionSound()
    {
        if (collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }
    }
}
