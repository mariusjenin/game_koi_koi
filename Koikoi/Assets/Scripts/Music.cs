using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip begin;
    public AudioClip loop;
    public AudioClip end;

    public AudioSource music;
    private void Start()
    {
        if(music == null)
            music = GetComponent<AudioSource>();
    }
    public void Play()
    {
        StartCoroutine(MusicCoroutine());
    }

    private IEnumerator MusicCoroutine()
    {
        music.PlayOneShot(begin);
        while (music.isPlaying)
            yield return new WaitForSeconds(Time.deltaTime);
        music.loop = true;
        music.clip = loop;
        music.Play();
    }

    public void Stop(bool FadeOut)
    {
        music.loop = false;
        if (FadeOut)
        {
            music.clip = end;
            music.Play();
        }
        else
            music.Stop();
    }
}
