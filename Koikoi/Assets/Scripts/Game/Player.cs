using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player :  Hand
{
    private Card selectedCard;
    public bool CanPlay = false;
    public void SelectCard(Card card)
    {
        if(CanPlay)
        {
            selectedCard = card;
            Debug.Log(selectedCard.id);
        }
    }
}
