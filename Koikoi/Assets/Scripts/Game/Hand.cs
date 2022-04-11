using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class Hand : CardZone
{
    public Deck deck;
    public ZoneYakus yakus;
    public int lastYakusCount = 0;
    public int score;

    protected bool canPlay = false;
    private void Awake()
    {
        yakus.scoreManager = new ScoreManager(yakus.Cards);
    }

    public override void Reset()
    {
        base.Reset();
        yakus.Cards.Clear();
        lastYakusCount = 0;

        for (var i = yakus.HikariGrid.transform.childCount - 1; i >= 0; i--)
            Destroy(yakus.HikariGrid.transform.GetChild(i).gameObject);

        for (var i = yakus.KasuGrid.transform.childCount - 1; i >= 0; i--)
            Destroy(yakus.KasuGrid.transform.GetChild(i).gameObject);

        for (var i = yakus.TanGrid.transform.childCount - 1; i >= 0; i--)
            Destroy(yakus.TanGrid.transform.GetChild(i).gameObject);

        for (var i = yakus.TaneGrid.transform.childCount - 1; i >= 0; i--)
            Destroy(yakus.TaneGrid.transform.GetChild(i).gameObject);
    }

    public virtual void CanPlay(bool canPlay)
    {
        this.canPlay = canPlay;

        if (canPlay && Cards.Count == 0 && GameManager.instance.CheckForTie()) 
            StartCoroutine(GameManager.instance.Tie());
    }

    public bool hasYakus()
    {
        yakus.scoreManager.UpdateYakus();
        return yakus.scoreManager.yakus.Count > lastYakusCount;
    }

    public IEnumerator AddCardToYakus(Card card1, Card card2)
    {
        yield return StartCoroutine(AddCardToYakusCoroutine(card1, card2));
    }

    private IEnumerator AddCardToYakusCoroutine(Card card1, Card card2)
    {
        Transform card1pos = card1.GetUI().transform;
        Transform card2pos = card2.GetUI().transform;
        Transform card1dest = null;
        Transform card2dest = null;
       
        switch (card1.type)
        {
            case Card.Type.Hikari:
                card1dest = yakus.HikariGrid.transform;
                break;
            case Card.Type.Kasu:
                card1dest = yakus.KasuGrid.transform;
                break;
            case Card.Type.Tan:
                card1dest = yakus.TanGrid.transform;
                break;
            case Card.Type.Tane:
                card1dest = yakus.TaneGrid.transform;
                break;
        }

        switch (card2.type)
        {
            case Card.Type.Hikari:
                card2dest = yakus.HikariGrid.transform;
                break;
            case Card.Type.Kasu:
                card2dest = yakus.KasuGrid.transform;
                break;
            case Card.Type.Tan:
                card2dest = yakus.TanGrid.transform;
                break;
            case Card.Type.Tane:
                card2dest = yakus.TaneGrid.transform;
                break;
        }
        // Animation
        yield return StartCoroutine(GameManager.instance.AddCardCouroutine(card1pos, card1dest, card2pos, card2dest));

        Destroy(card1.GetUI().canvas.gameObject);
        Destroy(card1.GetUI().GetComponent<Button>());
        Destroy(card2.GetUI().canvas.gameObject);
        Destroy(card2.GetUI().GetComponent<Button>());

        yakus.AddCard(card1);
        yakus.AddCard(card2);
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

    protected bool canDropCard(Card card)
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