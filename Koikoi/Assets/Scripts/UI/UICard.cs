using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    public Canvas canvas;
    private CardZone cardZone;
    public Card card;
    private Image image;

    private void Start()
    {
        SetCardZone(cardZone);
        image = GetComponent<Image>();
    }
    public void SetCardZone(CardZone cz)
    {
        this.cardZone = cz;
        if(GetComponent<Button>() != null)
            GetComponent<Button>().onClick.RemoveAllListeners();

        if (cz is Player) GetComponent<Button>().onClick.AddListener(OnClickPlayer);
        else if (cz is AI) GetComponent<Button>().enabled = false;
        else if (cz is Deck) GetComponent<Button>().onClick.AddListener(OnClickDeck);
        else if (cz is Board)
        {
            GetComponent<Button>().interactable = false;
            GetComponent<Button>().onClick.AddListener(OnClickBoard);
        }
    }
    public void Init(CardZone cz, Card c, Canvas cvs)
    {
        SetCardZone(cz);
        this.image = GetComponent<Image>();
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

        StartCoroutine(BoardCoroutine(player, ai, deck));
    }

    void OnClickDeck()
    {
        StartCoroutine(DeckCoroutine());
    }
    public IEnumerator BoardCoroutine(Player player, AI ai, Deck deck)
    {
        // Si le joueur effectue une association avec la carte du deck
        if (deck.topCard != null)
        {
            // Ajout des 2 cartes au Yakus du joueur
            ((Board)cardZone).AddCardsToYakus(card, deck.topCard, player);

            // R�initialisation 
            deck.topCard = null;
            yield return StartCoroutine(GameManager.instance.FadeOutGame());

            // Au tour de l'IA de jouer
            GameManager.instance.HandFinishTurn(player);
        }
        // Si le joueur effectue une association avec une carte de sa main
        else if (player.selectedCard != null)
        {

            // Ajout des 2 cartes au Yakus du joueur
            ((Board)cardZone).AddCardsToYakus(card, player.selectedCard, player);

            // R�initialisation 
            player.selectedCard = null;
            ((Board)cardZone).Cards.ForEach(c => c.GetUI().Hide());

            // Affichage de la carte sur le deck, et des cartes du joueur associables
            deck.DisplayOnTop();
        }
    }

    public IEnumerator DeckCoroutine()
    {
        // Ajout de la carte au board
        GameManager.instance.player.AddCardToBoard(((Deck)cardZone).topCard);

        // R�initialisation 
        GameManager.instance.deck.topCard = null;
        yield return StartCoroutine(GameManager.instance.FadeOutGame());

        // Au tour de l'IA de jouer
        GameManager.instance.HandFinishTurn(GameManager.instance.player);
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
