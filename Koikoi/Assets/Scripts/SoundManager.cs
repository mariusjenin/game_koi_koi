using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> cardClips;

    public AudioSource cardSource;
    public AudioSource otherCardSource;

    public void PlayCardSound(bool twice)
    {
        int random = Random.Range(0, cardClips.Count);
        AudioClip clip = cardClips[random];
        
        cardSource.PlayOneShot(clip);

        // If two cards are being moved
        if(twice)
        {
            random = Random.Range(0, cardClips.Count);
            clip = cardClips[random];

            otherCardSource.PlayOneShot(clip);
        }
    }
}
