using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PlayerChooseCardHand,
        PlayerChooseCardBoard1,
        PlayerChooseCardBoard2
        
    }
    // Singleton
    public static GameManager instance;

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

    public Deck deck;
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
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                yield return StartCoroutine(NewCard(ai));
            }

            for (int j = 0; j < 4; j++)
            {
                yield return StartCoroutine(NewCard(player));
            }

            for (int j = 0; j < 4; j++)
            {
                yield return StartCoroutine(NewCard(board));
            }
        }

        NextTurn();
        player.CanPlay = true;
    }

    // Fonction incr�mentant le tour et renvoyant la valeur du nouveau tour 
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

    // Fonction mettant � jour le score du joueur ou de l'IA
    void UpdateScore(bool player, int score)
    {
        if (player) PlayerScore.SetText("" + score);
        else AIScore.SetText("" + score);
    }

    private IEnumerator NewCard(CardZone cz)
    {
        yield return new WaitForSeconds(0.02f);

        // Cr�ation d'un template image � la position du deck
        GameObject image = Instantiate(template, Deck.transform.position, Deck.transform.rotation); // Utiliser card.image et faire animation
        image.transform.SetParent(Deck.transform.parent.transform);
        Card card = deck.Draw();
        image.GetComponent<UICard>().Init(cz, card);
        

        // Animation
        if(cz is Player)
            yield return StartCoroutine(AddCardCouroutine(image.transform, PlayerGrid.transform)); // D�placement vers Player
        else if(cz is AI)
            yield return StartCoroutine(AddCardCouroutine(image.transform, AIGrid.transform)); // D�placement vers AI
        else if (cz is Board)
            yield return StartCoroutine(AddCardCouroutine(image.transform, BoardGrid.transform)); // D�placement vers Board
        image.GetComponent<UICard>().Display();
        cz.AddCard(card);
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