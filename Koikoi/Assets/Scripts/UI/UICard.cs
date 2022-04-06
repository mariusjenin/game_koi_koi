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
        else if (cardZone is AI) GetComponent<Button>().enabled = false;
        else if (cardZone is Deck) GetComponent<Button>().onClick.AddListener(OnClickDeck);
        image = GetComponent<Image>();
    }
    public void Init(CardZone cz, Card c, Canvas cvs)
    {
        this.image = GetComponent<Image>();
        this.cardZone = cz;
        this.card = c;
        this.canvas = cvs;
    }

    public void Display()
    {
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
        Deck deck = GameManager.instance.deck;

        // Si le joueur effectue une association avec la carte du deck
        if (deck.topCard != null)
        {
            // Ajout des 2 cartes au Yakus du joueur
            ((Board)cardZone).AddCardsToYakus(card, deck.topCard, player);

            // Réinitialisation 
            deck.topCard = null;
            GameManager.instance.FadeOutGame();

            // Au tour de l'IA de jouer
            GameManager.instance.player.CanPlay(false);
            GameManager.instance.ai.CanPlay(true);
        } 
        // Si le joueur effectue une association avec une carte de sa main
        else
        {

            // Ajout des 2 cartes au Yakus du joueur
            ((Board)cardZone).AddCardsToYakus(card, player.selectedCard, player);

            // Réinitialisation 
            player.selectedCard = null;
            ((Board)cardZone).Cards.ForEach(c => c.GetUI().Hide());

            // Affichage de la carte sur le deck, et des cartes du joueur associables
            deck.DisplayOnTop();
            deck.DisplayTopCardAssociable();
        }
    }

    void OnClickDeck()
    {
        // Ajout de la carte au board
        GameManager.instance.player.AddCardToBoard(((Deck)cardZone).topCard);

        // Réinitialisation 
        ((Deck)cardZone).topCard = null;
        GameManager.instance.FadeOutGame();

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
