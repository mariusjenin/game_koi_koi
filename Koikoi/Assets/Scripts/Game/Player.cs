using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                StartCoroutine(GameManager.instance.FadeInGame());
                selectedCard = card;
                selectedCard.GetUI().Show();
                Board.DisplayAssociableCards(selectedCard);
            }
            // Le Joueur drop sa carte sélectionnée
            else if (card.Equals(selectedCard))
            {
                // Dépôt de la carte sélectionnée dans le board
                AddCardToBoard(selectedCard);
                RemoveCard(selectedCard);

                // Réinitialisation 
                selectedCard = null;
                Board.Cards.ForEach(c => c.GetUI().Hide());

                // Affichage de la carte sur le deck, et des cartes du joueur associables
                deck.DisplayOnTop();
                deck.DisplayTopCardAssociable();
            }
            // Le Joueur sélectionne une autre carte
            else
            {
                selectedCard.GetUI().Hide();
                selectedCard.GetUI().GetComponent<Button>().interactable = true;
                selectedCard = card;
                selectedCard.GetUI().Show();
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
