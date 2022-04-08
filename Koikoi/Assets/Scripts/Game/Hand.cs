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

    protected bool canPlay = false;
    private void Awake()
    {
        yakus.score = new ScoreManager(yakus.Cards);
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
        yakus.score.UpdateYakus();
        return yakus.score.yakus.Count > lastYakusCount;
    }

    public void AddCardToYakus(Card card)
    {
        StartCoroutine(AddCardToYakusCoroutine(card));
    }

    private IEnumerator AddCardToYakusCoroutine(Card card)
    {

        // Animation
        switch (card.type)
        {
            case Card.Type.Hikari:
                yield return StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, yakus.HikariGrid.transform));
                break;
            case Card.Type.Kasu:
                yield return StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, yakus.KasuGrid.transform));
                break;
            case Card.Type.Tan:
                yield return StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, yakus.TanGrid.transform));
                break;
            case Card.Type.Tane:
                yield return StartCoroutine(GameManager.instance.AddCardCouroutine(card.GetUI().transform, yakus.TaneGrid.transform));
                break;
        }

        Destroy(card.GetUI().canvas.gameObject);

        Destroy(card.GetUI().GetComponent<Button>());
        yakus.AddCard(card);
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