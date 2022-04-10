using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Unity.VisualScripting;
using UnityEngine;

public class AI : Hand
{
    public Board board;
    public Player player;

    public bool canKoikoi = false;

    struct GameStateAI
    {
        public struct Action
        {
            public bool playerTarget;
            public ActionPart1 actPart1;
            public ActionPart2 actPart2;
        }

        public struct ActionPossible
        {
            public bool playerTarget;
            public ActionPart1 actPart1;
            public List<ActionPart2> actParts2;
        }

        public struct ActionPart1
        {
            public bool pairDone;
            public Card card1;
            public Card card2;
        }
        
        public struct ActionPart2
        {
            public bool pairDone;
            public Card card1;
            public Card card2;
            public int score;
        }

        //HAND
        public List<Card> aiCards;
        public List<Card> boardCards;
        public List<Card> playerAndDeckCards;
        public List<Card> aiYakusCards;
        public List<Card> playerYaskusCards;

        public GameStateAI(GameStateAI previous)
        {
            playerAndDeckCards = new List<Card>(previous.playerAndDeckCards);
            aiCards = new List<Card>(previous.aiCards);
            boardCards = new List<Card>(previous.boardCards);
            aiYakusCards = new List<Card>(previous.aiYakusCards);
            playerYaskusCards = new List<Card>(previous.playerYaskusCards);
        }

        public int Utility()
        {
            ScoreManager aism = new ScoreManager(aiYakusCards);
            ScoreManager playersm = new ScoreManager(playerYaskusCards);
            aism.UpdateYakus();
            playersm.UpdateYakus();
            return aism.EvaluateScore() - playersm.EvaluateScore();
        }
        
        public static bool IsUtilityBetter(bool player,int score1, int score2)
        {
            return player ? score1 < score2 : score1 > score2;
        }


        public void ApplyAction(Action act)
        {
            //Part 1
            if (!act.playerTarget)
            {
                aiCards.Remove(act.actPart1.card1);
            }
            else
            {
                playerAndDeckCards.Remove(act.actPart1.card1);
            }

            if (act.actPart1.pairDone)
            {
                if (act.playerTarget)
                {
                    playerYaskusCards.Add(act.actPart1.card1);
                    playerYaskusCards.Add(act.actPart1.card2);
                }
                else
                {
                    aiYakusCards.Add(act.actPart1.card1);
                    aiYakusCards.Add(act.actPart1.card2);
                }

                boardCards.Remove(act.actPart1.card2);
            }
            else
            {
                boardCards.Add(act.actPart1.card1);
            }

            //Part 2
            playerAndDeckCards.Remove(act.actPart2.card1);
            if (act.actPart2.pairDone)
            {
                if (act.playerTarget)
                {
                    playerYaskusCards.Add(act.actPart2.card1);
                    playerYaskusCards.Add(act.actPart2.card2);
                }
                else
                {
                    aiYakusCards.Add(act.actPart2.card1);
                    aiYakusCards.Add(act.actPart2.card2);
                }

                boardCards.Remove(act.actPart2.card2);
            }
            else
            {
                boardCards.Add(act.actPart2.card1);
            }
        }

        public List<ActionPossible> GetActions(bool player) //false : IA | true : player
        {
            List<Card> sourceCardPart1 = player ? playerAndDeckCards : aiCards;

            List<ActionPossible> actions = new List<ActionPossible>();
            List<ActionPart1> actionParts1 = new List<ActionPart1>();

            //Part 1
            for (int i = 0; i < sourceCardPart1.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < boardCards.Count; j++)
                {
                    if (sourceCardPart1[i].month == boardCards[j].month)
                    {
                        ActionPart1 actPart1 = new ActionPart1();
                        actPart1.pairDone = true;
                        actPart1.card1 = sourceCardPart1[i];
                        actPart1.card2 = boardCards[j];
                        actionParts1.Add(actPart1);
                        found = true;
                    }
                }

                if (!found)
                {
                    ActionPart1 actPart1 = new ActionPart1();
                    actPart1.pairDone = false;
                    actPart1.card1 = sourceCardPart1[i];
                    actionParts1.Add(actPart1);
                }
            }

            //Part 2
            for (int i = 0; i < actionParts1.Count; i++)
            {
                List<Card> sourceCardPart2 = new List<Card>(playerAndDeckCards);
                List<Card> yakusAfterPart1 = player ? playerYaskusCards : aiYakusCards;
                
                List<Card> board = new List<Card>(boardCards);
                if (player)
                {
                    sourceCardPart2.Remove(actionParts1[i].card1);
                }

                if (actionParts1[i].pairDone)
                {
                    board.Remove(actionParts1[i].card2);
                    yakusAfterPart1.Add(actionParts1[i].card1);
                    yakusAfterPart1.Add(actionParts1[i].card2);
                }
                else
                {
                    board.Add(actionParts1[i].card1);
                }

                ActionPossible actionPossible = new ActionPossible();
                actionPossible.actPart1 = actionParts1[i];
                actionPossible.actParts2 = new List<ActionPart2>();
                ScoreManager sm = new ScoreManager();
                sm.SetCards(yakusAfterPart1);
                int scoreAfterPart1 = sm.EvaluateScore();
                for (int j = 0; j < sourceCardPart2.Count; j++)
                {
                    bool found = false;
                    for (int k = 0; k < board.Count; k++)
                    {
                        
                        if (sourceCardPart2[j].month == board[k].month)
                        {
                            ActionPart2 actPart2 = new ActionPart2();
                            actPart2.pairDone = true;
                            actPart2.card1 = sourceCardPart2[j];
                            actPart2.card2 = board[k];
                            
                            List<Card> yakusAfterPart2 = new List<Card>(yakusAfterPart1);
                            yakusAfterPart2.Add(actPart2.card1);
                            yakusAfterPart2.Add(actPart2.card2);
                            sm.SetCards(yakusAfterPart2);
                            actPart2.score = sm.EvaluateScore();
                            
                            actionPossible.actParts2.Add(actPart2);
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        ActionPart2 actPart2 = new ActionPart2();
                        actPart2.pairDone = false;
                        actPart2.card1 = sourceCardPart2[j];
                        actPart2.score = scoreAfterPart1;
                        actionPossible.actParts2.Add(actPart2);
                    }
                }

                actions.Add(actionPossible);
            }

            return actions;
        }


        public struct MinimaxResult
        {
            public int score;
            public ActionPossible act;
        }

        public MinimaxResult Minimax(bool player, int depth, ActionPossible prevActionPossible = new())
        {
            List<ActionPossible> acts = GetActions(player);

            if (depth <= 0 || acts.Count <= 0)
            {
                MinimaxResult mmrTerminal = new MinimaxResult();
                mmrTerminal.score = Utility();
                // mmrTerminal.act = prevActionPossible;
                return mmrTerminal;
            }


            int score = player ? -2147483648 : 2147483647;
            MinimaxResult mmr = new MinimaxResult();
            mmr.score = score;
            for (int i = 0; i < acts.Count; i++)
            {
                ActionPossible currActionPossible = acts[i];
                for (int j = 0; j < currActionPossible.actParts2.Count; j++)
                {
                    GameStateAI gsai = new GameStateAI(this);
                    Action act = new Action();
                    act.actPart1 = currActionPossible.actPart1;
                    act.actPart2 = currActionPossible.actParts2[j];
                    gsai.ApplyAction(act);

                    MinimaxResult currMmr = gsai.Minimax(!player, depth - 1, currActionPossible);
                    currMmr.act.actPart1 = currActionPossible.actPart1;
                    if (player)
                    {
                        currMmr.act.actParts2 = prevActionPossible.actParts2;
                        if (currMmr.score > mmr.score) mmr = currMmr;
                    }
                    else
                    {
                        currMmr.act.actParts2 = currActionPossible.actParts2;
                        if (currMmr.score < mmr.score) mmr = currMmr;
                    }
                }
            }

            return mmr;
        }
    }

    public override void CanPlay(bool canPlay)
    {
        base.CanPlay(canPlay);
        if (canPlay) StartCoroutine(Play());
    }

    public IEnumerator Play()
    {
        if (canPlay)
        {
            GameStateAI gsai = new GameStateAI();
            gsai.aiCards = new List<Card>(this.Cards);
            gsai.playerAndDeckCards = new List<Card>(GameManager.instance.player.Cards.Concat(deck.Cards).ToList());
            gsai.aiYakusCards = new List<Card>(this.yakus.Cards);
            gsai.playerYaskusCards = new List<Card>(GameManager.instance.player.yakus.Cards);
            gsai.boardCards = new List<Card>(board.Cards);
            AI.GameStateAI.ActionPossible actionPossible = gsai.Minimax(false, 2).act;

            yield return ExecuteAction(actionPossible);

            // Au tour du joueur de jouer
            GameManager.instance.HandFinishTurn(this);


            if (canKoikoi) // Se met � jour tout seul dans GameManager.EndTurn
            {
                canKoikoi = false;
                // Affichage de la Décision du koikoi
                // Koikoi :
                // StartCoroutine(GameManager.instance.Koikoi(false))
                //
                // Fin de tour :
                StartCoroutine(GameManager.instance.NextTurn(false));
            }
        }
    }

    private IEnumerator ExecuteAction(AI.GameStateAI.ActionPossible act)
    {
        //PART 1
        act.actPart1.card1.GetUI().Display();
        if (act.actPart1.pairDone)
        {
            // Anime les deux cartes vers la bonne zone Yakus
            yield return StartCoroutine(AddCardToYakus(act.actPart1.card1, act.actPart1.card2));
            // Supprime les cartes de la main et du board
            board.RemoveCard(act.actPart1.card2);
        }
        else
        {
            AddCardToBoard(act.actPart1.card1);
        }
        RemoveCard(act.actPart1.card1);

        //PART 2
        Card deckCard = GameManager.instance.deck.Draw();
        // Création d'un template image à la position du deck
        Transform deckTransform = GameManager.instance.deck.transform;
        GameObject gObject = Instantiate(GameManager.instance.template, deckTransform.position, deckTransform.rotation,
            deckTransform.parent.transform);
        UICard uiCard = gObject.GetComponentInChildren<UICard>();
        deckCard.SetUICard(uiCard);
        uiCard.Init(this, deckCard, gObject.GetComponent<Canvas>());
        deckCard.GetUI().Display();

        GameStateAI.ActionPart2 actPart2 = new GameStateAI.ActionPart2();
        bool found = false;
        for (int i = 0; i < act.actParts2.Count; i++)
        {
            if (act.actParts2[i].card1.isSame(deckCard))
            {
                if (!found || GameStateAI.IsUtilityBetter(false, act.actParts2[i].score, actPart2.score))
                {
                    actPart2 = act.actParts2[i];
                    found = true;
                }
            }
        }
        // actPart2.card1 est la carte tirée du deck, mais n'a pas d'UI d'initialisé, il faut préférer
        // deckCard, qui elle a été instanciée par le template.
        if (actPart2.pairDone)
        {
            // Anime les deux cartes vers la bonne zone Yakus
            yield return StartCoroutine(AddCardToYakus(actPart2.card2, deckCard));
            // Supprime les cartes de la main et du board
            board.RemoveCard(actPart2.card2);
        }
        else
        {
            AddCardToBoard(deckCard);
        }
    }
}