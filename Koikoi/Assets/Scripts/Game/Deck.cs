using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class Deck : CardZone
{
    public Card topCard;
    public Card Draw()
    {
        int random = Random.Range(0,Cards.Count);
        Card tmp = Cards[random];
        Cards.RemoveAt(random);
        return tmp;
    }

    public void DisplayOnTop()
    {
        // Création d'un template image à la position du deck
        GameObject gObject = Instantiate(GameManager.instance.template, transform.position, transform.rotation, transform.parent.transform);

        Card card = Draw();
        UICard uiCard = gObject.GetComponentInChildren<UICard>();

        card.SetUICard(uiCard);
        uiCard.Init(this, card, gObject.GetComponent<Canvas>());

        topCard = card;
        uiCard.Display();
    }

    public void DisplayTopCardAssociable()
    {
        topCard.GetUI().Show();

        GameManager.instance.board.Cards.ForEach(c =>
        {
            if (c.month.Equals(topCard.month))
                c.GetUI().Show();
            else
                c.GetUI().Hide();
        });
    }
}
