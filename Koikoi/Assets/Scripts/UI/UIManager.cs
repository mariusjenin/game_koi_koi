using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton
    public static UIManager instance;

    // Values
    public int MaxTurn = 12;
    public float speed = 20;

    // UI GameObjects
    public TextMeshProUGUI Turn;
    public TextMeshProUGUI PlayerScore;
    public TextMeshProUGUI AIScore;

    public GameObject Deck;
    public GameObject template;
    public GameObject PlayerGrid;
    public GameObject AIGrid;
    public GameObject BoardGrid;

    public Player player;
    public AI ai;
    public Board board;
    public Card debugCard;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ClearGame();
        // Debug
        StartCoroutine(InitGame());
    }

    void ClearGame()
    {
        // Clear Player hand
        for (var i = PlayerGrid.transform.childCount - 1; i >= 0; i--)
            Destroy(PlayerGrid.transform.GetChild(i).gameObject);

        // Clear AI hand
        for (var i = AIGrid.transform.childCount - 1; i >= 0; i--)
            Destroy(AIGrid.transform.GetChild(i).gameObject);

        // Clear Board
        for (var i = BoardGrid.transform.childCount - 1; i >= 0; i--)
            Destroy(BoardGrid.transform.GetChild(i).gameObject);
    }

    IEnumerator InitGame()
    {
        for (int i = 0; i < 8; i++)
        {
            yield return StartCoroutine(NewCard(player));
            yield return StartCoroutine(NewCard(ai));
            yield return StartCoroutine(NewCard(board));
        }
        NextTurn();
        player.CanPlay = true;
    }

    // Fonction incrémentant le tour et renvoyant la valeur du nouveau tour 
    int NextTurn()
    {
        int CurrentTurn = Int32.Parse(Turn.text); 

        if(CurrentTurn != MaxTurn)
        {
            CurrentTurn++;
            if(CurrentTurn < 10) Turn.SetText("0" + CurrentTurn);
            else Turn.SetText("" + CurrentTurn);
        }
        return CurrentTurn;
    }

    // Fonction mettant à jour le score du joueur ou de l'IA
    void UpdateScore(bool player, int score)
    {
        if (player) PlayerScore.SetText("" + score);
        else AIScore.SetText("" + score);
    }

    private IEnumerator NewCard(Hand hand)
    {
        yield return new WaitForSeconds(0.02f);

        // Création d'un template image à la position du deck
        GameObject image = Instantiate(template, Deck.transform.position, Deck.transform.rotation); // Utiliser card.image et faire animation
        image.transform.SetParent(Deck.transform.parent.transform);
        image.GetComponent<UICard>().Init(hand, hand.deck.Draw());
        

        // Animation
        if(hand is Player)
            yield return StartCoroutine(AddCardCouroutine(image.transform, PlayerGrid.transform)); // Déplacement vers Player
        else if(hand is AI)
            yield return StartCoroutine(AddCardCouroutine(image.transform, AIGrid.transform)); // Déplacement vers AI
        else if (hand is Board)
            yield return StartCoroutine(AddCardCouroutine(image.transform, BoardGrid.transform)); // Déplacement vers Board
        image.GetComponent<UICard>().Display();
        hand.AddCard(hand.deck.Draw());
    }

    private IEnumerator AddCardCouroutine(Transform initial, Transform destination)
    {
        Debug.Log(Vector3.Distance(initial.position, destination.position));

        while (Vector3.Distance(initial.position, destination.position) > 0.1f)
        {
            initial.position = Vector3.MoveTowards(initial.position, destination.position, speed / 0.4f);
            yield return new WaitForSeconds(0.01f);
        }
        initial.SetParent(destination);

    }

}
