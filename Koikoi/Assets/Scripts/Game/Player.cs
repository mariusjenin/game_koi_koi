using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player :  Hand
{
    public Board Board;

    public Card selectedCard;
    public bool canPlay = false;


    public void SelectCard(Card card)
    {
        if(canPlay)
        {
            // Le Joueur n'avait pas de carte sélectionné
            if(selectedCard == null)
            {
                GameManager.FadeInGame();
                selectedCard = card;
                Board.DisplayAssociableCards(selectedCard);
            }
            // Le Joueur retire la sélection de sa carte
            else if (card.Equals(selectedCard))
            {
                GameManager.FadeOutGame();
                selectedCard = null;
            }
            // Le Joueur sélectionne une autre carte
            else
            {
                selectedCard = card;
                Board.DisplayAssociableCards(selectedCard);
            }
        }
    }

    public void CanPlay(bool canPlay)
    {
        this.canPlay = canPlay;

        if (canPlay) GameManager.ActivateButtons();
        else GameManager.DesactivateButtons();
    }
}
