using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    private Canvas canvas;
    private CardZone cardZone;
    private Card card;
    private Image image;

    private void Start()
    {
        if (cardZone is Player) GetComponent<Button>().onClick.AddListener(OnClickPlayer);
        else if (cardZone is Board)
        {
            GetComponent<Button>().interactable = false;
            GetComponent<Button>().onClick.AddListener(OnClickBoard);
        }
        else if (cardZone is AI) Destroy(gameObject.GetComponent<Button>());
        image = GetComponent<Image>();
    }
    public void Init(CardZone cz, Card c, Canvas cvs)
    {
        this.cardZone = cz;
        this.card = c;
        this.canvas = cvs;
    }

    public void Display()
    {
        if (cardZone is Player || cardZone is Board) 
            image.sprite = card.sprite;
    }

    void OnClickPlayer()
    {
        ((Player)cardZone).SelectCard(card);
        
    }
    void OnClickBoard()
    {
        Player player = ((Board)cardZone).Player;

        ((Board)cardZone).AddCardsToYakus(card, player.selectedCard, player);

        player.GameManager.FadeOutGame();
        player.selectedCard = null;
        player.CanPlay(false);
    }

    public void Show()
    {
        canvas.sortingOrder = 2;
        GetComponent<Button>().interactable = true;
    }
    public void Hide()
    {
        canvas.sortingOrder = 1;
        GetComponent<Button>().interactable = false;
    }
}
