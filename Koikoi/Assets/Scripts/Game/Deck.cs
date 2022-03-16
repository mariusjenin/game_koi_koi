using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> Cards;

    public Card Draw()
    {
        int random = Random.Range(0,Cards.Count);
        Card tmp = Cards[random];
        Cards.RemoveAt(random);
        return tmp;
    }
}
