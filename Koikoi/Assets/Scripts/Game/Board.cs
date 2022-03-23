using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class Board : CardZone
{
    public void DisplayAssociableCards(Card card)
    {
        Cards.ForEach(c =>
        {
            if (c.month.Equals(card.month))
                c.GetUI().Show();
            else
                c.GetUI().Hide();
        });
    }

    public void AddCardsToYakus(Card boardCard, Card handCard, Hand hand)
    {
        hand.AddCardToYakus(boardCard);
        hand.AddCardToYakus(handCard);

        RemoveCard(boardCard);
        hand.RemoveCard(handCard);            
    }
}
