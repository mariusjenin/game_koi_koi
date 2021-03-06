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

    public SoundManager soundManager;

    // Values
    public int MaxTurn = 12;
    public bool koikoi = false;

    private float speed = 15;

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
    [SerializeField] private KoiKoiPopUp koikoiPopUp;

    public Deck deck;
    public Player player;
    public AI ai;
    public Board board;

    private bool reseting = false;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitGame());
        soundManager.music.Play();
    }

    private void ClearGame()
    {
        player.Reset();
        ai.Reset();
        board.Reset();
        deck.Reset();
        koikoi = false;

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

    private IEnumerator InitGame()
    {
        if (!reseting)
        {
            reseting = true;

            int CurrentTurn = Int32.Parse(Turn.text);

            if(CurrentTurn == MaxTurn)
            {
                // END Game
            } 
            else
            {
                ClearGame();

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

                IncreaseTurn();
                player.CanPlay(true);
            }

            reseting = false;
        }
    }

    public IEnumerator NextTurn(bool isPlayer)
    {
        koikoiPopUp.gameObject.SetActive(true);

        Hand hand = isPlayer ? player : ai;

        yield return StartCoroutine(FadeInGame());

        yield return StartCoroutine(koikoiPopUp.NextTurnCoroutine(hand));

        // R??initialisation de l'UI apr??s fade out
        yield return StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0f, 0.2f));
        koikoiPopUp.gameObject.SetActive(false);

        UpdateScore(isPlayer);
        // R??initialisation du jeu
        StartCoroutine(InitGame());
    }
    public IEnumerator Koikoi(bool isPlayer)
    {
        koikoiPopUp.gameObject.SetActive(true);

        // Set koikoi state to true
        koikoi = true;

        Hand hand = isPlayer ? player : ai;
        hand.lastYakusCount++;

        yield return StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0.5f, 0.2f));

        yield return StartCoroutine(koikoiPopUp.KoikoiCoroutine(hand));

        // R??initialisation de l'UI apr??s fade out
        yield return StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0f, 0.2f));
        koikoiPopUp.gameObject.SetActive(false);
        if (isPlayer) ai.CanPlay(true);
    }
    public IEnumerator Tie()
    {
        koikoiPopUp.gameObject.SetActive(true);

        yield return StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0.5f, 0.2f));

        yield return StartCoroutine(koikoiPopUp.TieCoroutine());

        // R??initialisation de l'UI apr??s fade out
        yield return StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0f, 0.2f));
        koikoiPopUp.gameObject.SetActive(false);

        // R??initialisation du jeu
        StartCoroutine(InitGame());
    }


    public void HandFinishTurn(Hand hand)
    {
        // On v??rifie si l'entit?? a un yaku
        if (hand.hasYakus())
        {
            
            hand.yakus.scoreManager.UpdateYakus();
            for (int i = 0; i < hand.yakus.scoreManager.yakus.Count; i++)
            {
                Debug.Log(hand.yakus.scoreManager.yakus[i]);
            }
            if (hand is Player)
            {
                StartCoroutine(PopUpKoiKoi(KoiKoiPopUp.Type.PLAYER));
                koikoiPopUp.isShowed = true;
            }
            else if (hand is AI) ((AI)hand).canKoikoi = true;
        }

        // On passe la main ?? l'autre entit??
        hand.CanPlay(false);
        if (hand is Player) ai.CanPlay(!koikoiPopUp.isShowed); // L'IA peut jouer si le popup du joueur n'est pas montr??
        else player.CanPlay(true);
    }


    // Fonction incr???mentant le tour et renvoyant la valeur du nouveau tour 
    private int IncreaseTurn()
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

    // Fonction mettant ??? jour le score du joueur ou de l'IA
    void UpdateScore(bool isPlayer)
    {
        if (isPlayer)
        {
            player.score += player.yakus.GetScore();
            PlayerScore.SetText("" + player.score);
        }
        else
        {
            ai.score += ai.yakus.GetScore();
            AIScore.SetText("" + ai.score);
        }
    }

    private IEnumerator NewCard(CardZone cz)
    {
        yield return new WaitForSeconds(0.02f);

        // Cr???ation d'un template image ??? la position du deck
        GameObject gObject = Instantiate(template, Deck.transform.position, Deck.transform.rotation, Deck.transform.parent.transform);

        Card card = deck.Draw();
        UICard uiCard = gObject.GetComponentInChildren<UICard>();

        card.SetUICard(uiCard);
        uiCard.Init(cz, card, gObject.GetComponent<Canvas>());
        

        // Animation
        if(cz is Player)
            yield return StartCoroutine(AddCardCouroutine(gObject.transform, PlayerGrid.transform)); // D???placement vers Player
        else if(cz is AI)
            yield return StartCoroutine(AddCardCouroutine(gObject.transform, AIGrid.transform)); // D???placement vers AI
        else if (cz is Board)
            yield return StartCoroutine(AddCardCouroutine(gObject.transform, BoardGrid.transform)); // D???placement vers Board

        if (cz is Player || cz is Board)
            uiCard.Display();
        cz.AddCard(card);
    }

    public IEnumerator AddCardCouroutine(Transform initial, Transform destination, Transform other=null, Transform otherDestination=null)
    {
        float startOtherDistance = 0f;

        float startInitialDistance = Vector3.Distance(initial.position, destination.position);
        if(other != null)
            startOtherDistance = Vector3.Distance(other.position, otherDestination.position);

        soundManager.PlayCardSound(other != null);

        while (Vector3.Distance(initial.position, destination.position) > 0.1f
            && (other==null || Vector3.Distance(other.position, otherDestination.position) > 0.1f))
        {
            initial.position = Vector3.MoveTowards(initial.position, destination.position, (startInitialDistance/100) * speed);
            if(other != null)
                other.position = Vector3.MoveTowards(other.position, otherDestination.position, (startOtherDistance / 100) * speed);
            yield return new WaitForSeconds(0.01f);
        }
        initial.SetParent(destination);
        if (other != null)
            other.SetParent(otherDestination);

    }

    public IEnumerator FadeInGame()
    {
        yield return StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0.5f, 0.2f));
    }

    public IEnumerator FadeOutGame()
    {
        yield return StartCoroutine(Fade(BlackOverlay, BlackOverlay.color.a, 0f, 0.2f));
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

    public IEnumerator PopUpKoiKoi(KoiKoiPopUp.Type type, Hand hand=null)
    {
        DesactivateButtons();

        yield return StartCoroutine(FadeInGame());
        yield return StartCoroutine(koikoiPopUp.Show(type, hand));
    }

    public bool CheckForTie()
    {
        return player.Cards.Count <= 0 && ai.Cards.Count <= 0;
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