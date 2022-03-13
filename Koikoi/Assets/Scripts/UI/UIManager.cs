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
    public GameObject Player;
    public GameObject AI;

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
        for (var i = Player.transform.childCount - 1; i >= 0; i--)
            Destroy(Player.transform.GetChild(i).gameObject);

        // Clear AI hand
        for (var i = AI.transform.childCount - 1; i >= 0; i--)
            Destroy(AI.transform.GetChild(i).gameObject);

        // Clear Board
    }

    IEnumerator InitGame()
    {
        for (int i = 0; i < 8; i++)
        {
            yield return StartCoroutine(NewCard(false, debugCard));
            yield return StartCoroutine(NewCard(true, debugCard));
        }
        NextTurn();
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

    private IEnumerator NewCard(bool player, Card card)
    {
        yield return new WaitForSeconds(0.02f);
        // Création d'un template image à la position du deck
        GameObject image = Instantiate(template, Deck.transform.position, Deck.transform.rotation); // Utiliser card.image et faire animation
        image.transform.SetParent(Deck.transform.parent.transform);
        image.GetComponent<UICard>().Init(player, card);
        

        // Animation
        if(player)
        {
            yield return StartCoroutine(AddCardCouroutine(image.transform, Player.transform)); // Déplacement vers Player
        }
        else
        {
            yield return StartCoroutine(AddCardCouroutine(image.transform, AI.transform)); // Déplacement vers AI
        }
        image.GetComponent<UICard>().Display();
    }

    private IEnumerator AddCardCouroutine(Transform initial, Transform destination)
    {
        while(Vector3.Distance(initial.position, destination.position) > 0)
        {
            initial.position = Vector3.MoveTowards(initial.position, destination.position, Time.deltaTime * speed * 500f);
            yield return new WaitForSeconds(0.02f);
        }
        initial.SetParent(destination);

    }

}
