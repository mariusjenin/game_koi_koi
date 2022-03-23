using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class CardZone: MonoBehaviour
    {

        [SerializeField]
        protected List<Card> Cards = new List<Card>();
        
        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }

        public void DesactivateButtons()
        {
            Cards.ForEach(c => c.GetUI().GetComponent<Button>().interactable = false);
        }

        public void ActivateButtons()
        {
            Cards.ForEach(c => c.GetUI().GetComponent<Button>().interactable = true);
        }
    }
}