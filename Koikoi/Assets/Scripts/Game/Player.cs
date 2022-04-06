using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player :  Hand
{
    public Board Board;

    public Card selectedCard;


    public void SelectCard(Card card)
    {
        if(canPlay)
        {
            // Le Joueur n'avait pas de carte sélectionné
            if(selectedCard == null)
            {
                GameManager.instance.FadeInGame();
                selectedCard = card;
                Board.DisplayAssociableCards(selectedCard);
            }
            // Le Joueur retire la sélection de sa carte
            else if (card.Equals(selectedCard))
            {
                GameManager.instance.FadeOutGame();
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

    public override void CanPlay(bool canPlay)
    {
        base.CanPlay(canPlay);

        if (canPlay) GameManager.instance.ActivateButtons();
        else GameManager.instance.DesactivateButtons();
    }
}
