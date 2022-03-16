using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    // public enum MONTH
    // {
    //     JAN, FEB
    // };
    //
    // public MONTH month;
    public int id;
    public Sprite sprite;

}
