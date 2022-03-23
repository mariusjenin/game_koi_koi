using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Hand
{
    public override void CanPlay(bool canPlay)
    {
        base.CanPlay(canPlay);
        Play();
    }

    public void Play()
    {
        Debug.Log("L'IA joue !");
        // TODO
    }
}
