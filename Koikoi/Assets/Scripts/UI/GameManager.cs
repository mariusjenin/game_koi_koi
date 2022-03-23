using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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
    public Image BlackOverlay;

    public Deck deck;
    public Player player;
    public AI ai;
    public Board board;

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
        player.CanPlay(false);
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
        player.CanPlay(true);
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
        GameObject gObject = Instantiate(template, Deck.transform.position, Deck.transform.rotation, Deck.transform.parent.transform);

        Card card = deck.Draw();
        UICard uiCard = gObject.GetComponentInChildren<UICard>();

        card.SetUICard(uiCard);
        uiCard.Init(cz, card, gObject.GetComponent<Canvas>());
        

        // Animation
        if(cz is Player)
            yield return StartCoroutine(AddCardCouroutine(gObject.transform, PlayerGrid.transform)); // D�placement vers Player
        else if(cz is AI)
            yield return StartCoroutine(AddCardCouroutine(gObject.transform, AIGrid.transform)); // D�placement vers AI
        else if (cz is Board)
            yield return StartCoroutine(AddCardCouroutine(gObject.transform, BoardGrid.transform)); // D�placement vers Board
        uiCard.Display();
        cz.AddCard(card);
    }

    public IEnumerator AddCardCouroutine(Transform initial, Transform destination)
    {
        while (Vector3.Distance(initial.position, destination.position) > 0.1f)
        {
            initial.position = Vector3.MoveTowards(initial.position, destination.position, speed / 0.4f);
            yield return new WaitForSeconds(0.01f);
        }
        initial.SetParent(destination);

    }

    public void FadeInGame()
    {
        StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0.5f, 0.2f));
    }

    public void FadeOutGame()
    {
        StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0f, 0.2f));
    }

    private IEnumerator Fade(Image img, float start, float end, float duration)
    {
        float timer = 0f;
        AnimationCurve smoothCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, Mathf.Lerp(start, end, smoothCurve.Evaluate(timer / duration)));
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void DesactivateButtons()
    {
        player.DesactivateButtons();
        board.DesactivateButtons();
    }

    public void ActivateButtons()
    {
        player.ActivateButtons();
        board.ActivateButtons();
    }

}