using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Hand
{
    public Board board;
    public override void CanPlay(bool canPlay)
    {
        base.CanPlay(canPlay);
        if(canPlay) Play();
    }
    public void Play()
    {
        if(canPlay)
        {
            Debug.Log("L'IA joue !");
            // TODO

            Card boardCard = board.Cards[0];
            Card aiCard = Cards[0];

            PlayCards(boardCard, aiCard);


            // Au tour du joueur de jouer
            CanPlay(false);
            GameManager.instance.player.CanPlay(true);
        }
    }

    private void PlayCards(Card boardCard, Card aiCard)
    {
        aiCard.GetUI().Display();

        // Anime les deux cartes vers la bonne zone Yakus
        AddCardToYakus(boardCard);
        AddCardToYakus(aiCard);

        // Supprime les cartes de la main et du board
        board.RemoveCard(boardCard);
        RemoveCard(aiCard);

    }
}
