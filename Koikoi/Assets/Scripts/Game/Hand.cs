using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class Hand : CardZone
{
    public Deck deck;
    public ZoneYakus yakus;

    protected bool canPlay = false;
    public virtual void CanPlay(bool canPlay)
    {
        this.canPlay = canPlay;
    }

    public void AddCardToYakus(Card card)
    {
        Destroy(card.GetUI().GetComponent<Button>());
        yakus.AddCard(card);

        // Animation
        switch (card.type)
        {
            case Card.Type.Hikari:
                StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, yakus.HikariGrid.transform));
                break;
            case Card.Type.Kasu:
                StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, yakus.KasuGrid.transform));
                break;
            case Card.Type.Tan:
                StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, yakus.TanGrid.transform));
                break;
            case Card.Type.Tane:
                StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, yakus.TaneGrid.transform));
                break;
        }
    }
    
    public void AddCardToBoard(Card card)
    {
        Destroy(card.GetUI().GetComponent<Button>());
        GameManager.instance.board.AddCard(card);

        StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, GameManager.instance.BoardGrid.transform));
    }

}