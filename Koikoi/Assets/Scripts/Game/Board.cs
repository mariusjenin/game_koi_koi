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
        RemoveCard(boardCard);
        hand.RemoveCard(handCard);

        StartCoroutine(hand.AddCardToYakus(boardCard, handCard));
    }
    public bool canDropCard(Card card)
    {
        bool canDrop = true;
        GameManager.instance.board.Cards.ForEach(c =>
        {
            if (canDrop && c.month.Equals(card.month))
            {
                canDrop = false;
            }
        });
        return canDrop;
    }
}
