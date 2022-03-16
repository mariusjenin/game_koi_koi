using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CardZone: MonoBehaviour
    {
        [System.NonSerialized]
        protected List<Card> Cards = new List<Card>();
        
        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }
    }
}