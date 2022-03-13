using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public Deck deck;
    [System.NonSerialized]
    public List<Card> Cards = new List<Card>();

    public void AddCard(Card card)
    {
        Cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        Cards.Remove(card);
    }

}
