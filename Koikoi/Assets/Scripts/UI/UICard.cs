using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    private CardZone hand;
    private Card card;
    private Image image;

    private void Start()
    {
        if (hand is Player) GetComponent<Button>().onClick.AddListener(OnClick);
        else if(hand is AI) Destroy(gameObject.GetComponent<Button>());
        image = GetComponent<Image>();
    }
    public void Init(CardZone cz, Card card)
    {
        this.hand = cz;
        this.card = card;
    }

    public void Display()
    {
        if (hand is Player || hand is Board) 
            image.sprite = card.sprite;
    }

    void OnClick()
    {
        ((Player)hand).SelectCard(card);
    }
}
