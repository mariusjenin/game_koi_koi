using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class Hand : CardZone
{
    public Deck deck;
    public ZoneYakus yakus;

    public void AddCardToYakus(Card card)
    {
        Destroy(card.GetUI().GetComponent<Button>());
        yakus.AddCard(card);

        // Animation
        switch (card.type)
        {
            case Card.Type.Hikari:
                StartCoroutine(GameManager.AddCardCouroutine(card.GetUI().transform, yakus.HikariGrid.transform));
                break;
            case Card.Type.Kasu:
                StartCoroutine(GameManager.AddCardCouroutine(card.GetUI().transform, yakus.KasuGrid.transform));
                break;
            case Card.Type.Tan:
                StartCoroutine(GameManager.AddCardCouroutine(card.GetUI().transform, yakus.TanGrid.transform));
                break;
            case Card.Type.Tane:
                StartCoroutine(GameManager.AddCardCouroutine(card.GetUI().transform, yakus.TaneGrid.transform));
                break;
        }
    }

}
