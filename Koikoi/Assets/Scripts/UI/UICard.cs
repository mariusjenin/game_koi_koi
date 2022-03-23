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
        Player player = GameManager.instance.player;
        AI ai = GameManager.instance.ai;

        // Ajout des 2 cartes au Yakus du joueur
        ((Board)cardZone).AddCardsToYakus(card, player.selectedCard, player);

        // Réinitialisation 
        GameManager.instance.FadeOutGame();
        GameManager.instance.player.selectedCard = null;

        // Au tour de l'IA de jouer
        GameManager.instance.player.CanPlay(false);
        GameManager.instance.ai.CanPlay(true);


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
