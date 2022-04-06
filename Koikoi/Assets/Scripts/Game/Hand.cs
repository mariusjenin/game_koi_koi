using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class Hand : CardZone
{
    public Deck deck;
    public ZoneYakus yakus;
    int lastYakusCount = 0;

    protected bool canPlay = false;
    private void Awake()
    {
        yakus.score = new ScoreManager(yakus.Cards);
    }

    public virtual void CanPlay(bool canPlay)
    {
        this.canPlay = canPlay;
    }

    public bool hasYakus()
    {
        yakus.score.UpdateYakus();
        return yakus.score.yakus.Count > lastYakusCount;
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
        if (!card.GetUI().gameObject.GetComponent<Button>().enabled)
            card.GetUI().gameObject.GetComponent<Button>().enabled = true;

        card.GetUI().SetCardZone(GameManager.instance.board);

        GameManager.instance.board.AddCard(card);
        RemoveCard(card);

        StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().canvas.gameObject.transform, GameManager.instance.BoardGrid.transform));
    }

}