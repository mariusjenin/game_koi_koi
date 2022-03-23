using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    private CardZone cardZone;
    private Card card;
    private Image image;

    private void Start()
    {
        if (cardZone is Player) GetComponent<Button>().onClick.AddListener(OnClickPlayer);
        else if (cardZone is Board) GetComponent<Button>().onClick.AddListener(OnClickBoard);
        else if(cardZone is AI) Destroy(gameObject.GetComponent<Button>());
        image = GetComponent<Image>();
    }
    public void Init(CardZone cz, Card c)
    {
        this.cardZone = cz;
        this.card = c;
    }

    public void Display()
    {
        if (cardZone is Player || cardZone is Board) 
            image.sprite = card.sprite;
    }

    void OnClickPlayer()
    {
        Button bt = GetComponent<Button>();
        // GetComponent<Image>().color = bt.colors.pressedColor;
        ((Player)cardZone).SelectCard(card);
        
    }
    void OnClickBoard()
    {
        Button bt = GetComponent<Button>();
        // bt.interactable = false;
        GetComponent<Image>().color = bt.colors.pressedColor;
        ((Player)cardZone).SelectCard(card);
        
    }
}
