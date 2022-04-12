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

    // public static int caseComputed;
    
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

        public abstract class ActionPart : IComparable<ActionPart>
        {
            public bool player;
            public bool pairDone;
            public bool wholeMonth;
            public Card card1;
            public Card card2;
            public Card card3;
            public Card card4;
            public int score;
            public int CompareTo(ActionPart other)
            {
                return score==other.score?0:(player ? score < other.score : score > other.score)?1:-1;
            }
        }
        
        public class ActionPart1: ActionPart
        {
            public List<Card> cardsAfterAction;
        }
        
        public class ActionPart2 : ActionPart
        {
            
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
                List<Card> yakusCard = act.playerTarget ? playerYaskusCards : aiYakusCards;
                yakusCard.Add(act.actPart1.card1);
                yakusCard.Add(act.actPart1.card2);
                if (act.actPart1.wholeMonth)
                {
                    yakusCard.Add(act.actPart1.card3);
                    yakusCard.Add(act.actPart1.card4); 
                    boardCards.Remove(act.actPart1.card3);
                    boardCards.Remove(act.actPart1.card4);
                }
                // if (act.playerTarget)
                // {
                //     playerYaskusCards.Add(act.actPart1.card1);
                //     playerYaskusCards.Add(act.actPart1.card2);
                // }
                // else
                // {
                //     aiYakusCards.Add(act.actPart1.card1);
                //     aiYakusCards.Add(act.actPart1.card2);
                // }

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
                List<Card> yakusCard = act.playerTarget ? playerYaskusCards : aiYakusCards;
                yakusCard.Add(act.actPart2.card1);
                yakusCard.Add(act.actPart2.card2);
                if (act.actPart2.wholeMonth)
                {
                    yakusCard.Add(act.actPart2.card3);
                    yakusCard.Add(act.actPart2.card4);
                    boardCards.Remove(act.actPart2.card3);
                    boardCards.Remove(act.actPart2.card4);
                }
                
                // if (act.playerTarget)
                // {
                //     playerYaskusCards.Add(act.actPart2.card1);
                //     playerYaskusCards.Add(act.actPart2.card2);
                // }
                // else
                // {
                //     aiYakusCards.Add(act.actPart2.card1);
                //     aiYakusCards.Add(act.actPart2.card2);
                // }

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

            
            List<Card> yakusBeforeActions = player ? playerYaskusCards : aiYakusCards;
            ScoreManager sm = new ScoreManager(yakusBeforeActions);
            int scoreBeforeActions = sm.EvaluateScore();
            
            //Part 1
            for (int i = 0; i < sourceCardPart1.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < boardCards.Count; j++)
                {
                    if (sourceCardPart1[i].month == boardCards[j].month)
                    {
                        List<int> cardOfMonth = new List<int>();
                        for (int k = 0; k < boardCards.Count; k++)
                        {
                            if (sourceCardPart1[i].month == boardCards[k].month) cardOfMonth.Add(k);
                        }
                        
                        ActionPart1 actPart1 = new ActionPart1();
                        actPart1.player = player;
                        actPart1.pairDone = true;
                        actPart1.card1 = sourceCardPart1[i];
                        // actPart1.wholeMonth = cardOfMonth.Count == 3;
                        // actPart1.card2 = boardCards[cardOfMonth[0]];
                        actPart1.wholeMonth = false; //TODO
                        actPart1.card2 = boardCards[j];
                        
                        List<Card> yakusAfterPart1 = new List<Card>(yakusBeforeActions);
                        yakusAfterPart1.Add(actPart1.card1);
                        yakusAfterPart1.Add(actPart1.card2);
                        if (actPart1.wholeMonth)
                        {
                            actPart1.card3 = boardCards[cardOfMonth[1]];
                            actPart1.card4 = boardCards[cardOfMonth[2]];
                            yakusAfterPart1.Add(actPart1.card3);
                            yakusAfterPart1.Add(actPart1.card4);
                        }
                        actPart1.cardsAfterAction = yakusAfterPart1;
                        sm.SetCards(actPart1.cardsAfterAction);
                        actPart1.score = sm.EvaluateScore();;
                        actionParts1.Add(actPart1);
                        found = true;
                    }
                }

                if (!found)
                {
                    ActionPart1 actPart1 = new ActionPart1();
                    actPart1.player = player;
                    actPart1.pairDone = false;
                    actPart1.card1 = sourceCardPart1[i];
                    actPart1.cardsAfterAction = new List<Card>(yakusBeforeActions);
                    actPart1.score = scoreBeforeActions;
                    actionParts1.Add(actPart1);
                }
            }
            actionParts1.Sort();

            //Part 2
            for (int i = 0; i < actionParts1.Count; i++)
            {
                List<Card> sourceCardPart2 = new List<Card>(playerAndDeckCards);
                
                List<Card> board = new List<Card>(boardCards);
                if (player)
                {
                    sourceCardPart2.Remove(actionParts1[i].card1);
                }

                if (actionParts1[i].pairDone)
                {
                    board.Remove(actionParts1[i].card2);
                    if (actionParts1[i].wholeMonth)
                    {
                        board.Remove(actionParts1[i].card3);
                        board.Remove(actionParts1[i].card4);
                    }
                }
                else
                {
                    board.Add(actionParts1[i].card1);
                }

                ActionPossible actionPossible = new ActionPossible();
                actionPossible.actPart1 = actionParts1[i];
                actionPossible.actParts2 = new List<ActionPart2>();
                for (int j = 0; j < sourceCardPart2.Count; j++)
                {
                    bool found = false;
                    for (int k = 0; k < board.Count; k++)
                    {
                        
                        if (sourceCardPart2[j].month == board[k].month)
                        {
                            List<int> cardOfMonth = new List<int>();
                            for (int l = 0; l < board.Count; l++)
                            {
                                if (sourceCardPart2[j].month == board[l].month) cardOfMonth.Add(l);
                            }
                            
                            ActionPart2 actPart2 = new ActionPart2();
                            actPart2.player = player;
                            actPart2.pairDone = true;
                            actPart2.card1 = sourceCardPart2[j];
                            actPart2.wholeMonth = false; //TODO
                            actPart2.card2 = board[k];
                            // actPart2.wholeMonth = cardOfMonth.Count == 3;
                            // actPart2.card2 = board[cardOfMonth[0]];
                            
                            List<Card> yakusAfterPart2 = new List<Card>(actionPossible.actPart1.cardsAfterAction);
                            yakusAfterPart2.Add(actPart2.card1);
                            yakusAfterPart2.Add(actPart2.card2);
                            if (actPart2.wholeMonth)
                            {
                                actPart2.card3 = boardCards[cardOfMonth[1]];
                                actPart2.card4 = boardCards[cardOfMonth[2]];
                                yakusAfterPart2.Add(actPart2.card3);
                                yakusAfterPart2.Add(actPart2.card4);
                            }
                            sm.SetCards(yakusAfterPart2);
                            actPart2.score = sm.EvaluateScore();
                            
                            actionPossible.actParts2.Add(actPart2);
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        ActionPart2 actPart2 = new ActionPart2();
                        actPart2.player = player;
                        actPart2.pairDone = false;
                        actPart2.card1 = sourceCardPart2[j];
                        actPart2.score = actionPossible.actPart1.score;
                        actionPossible.actParts2.Add(actPart2);
                    }
                }
                actionPossible.actParts2.Sort();
                
                actions.Add(actionPossible);
            }

            return actions;
        }


        public struct MinimaxResult
        {
            public int score;
            public ActionPossible act;
        }

        public MinimaxResult Minimax()
        {
            
            // caseComputed = 0;
            MinimaxResult mmr = MinimaxAux(false, 2, -2147483648, 2147483647);
            // Debug.Log(caseComputed);
            return mmr;
        }
        public MinimaxResult MinimaxAux(bool player, int depth,int alpha, int beta, ActionPossible prevActionPossible = new())
        {
            List<ActionPossible> acts = GetActions(player);

            if (depth <= 0 || acts.Count <= 0)
            {
                MinimaxResult mmrTerminal = new MinimaxResult();
                mmrTerminal.score = Utility();
                // caseComputed++;
                return mmrTerminal;
            }


            int score = player ? -2147483648 : 2147483647;
            MinimaxResult mmr = new MinimaxResult();
            mmr.score = score;
            bool stop = false;
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

                    MinimaxResult currMmr = gsai.MinimaxAux(!player, depth - 1,alpha, beta, currActionPossible);
                    currMmr.act.actPart1 = currActionPossible.actPart1;
                    if (player)
                    {
                        currMmr.act.actParts2 = prevActionPossible.actParts2;
                        if (currMmr.score > mmr.score) mmr = currMmr;
                        if (mmr.score > alpha) alpha = mmr.score;
                        if (mmr.score >= beta)
                        {
                            stop = true;
                            break;
                        }
                    }
                    else
                    {
                        currMmr.act.actParts2 = currActionPossible.actParts2;
                        if (currMmr.score < mmr.score) mmr = currMmr;
                        if (mmr.score < beta) beta = mmr.score;
                        if (mmr.score <= alpha)
                        {
                            stop = true;
                            break;
                        }
                    }
                }

                if (stop) break;
            }

            return mmr;
        }
    }

    public override void CanPlay(bool canPlay)
    {
        this.canPlay = canPlay;
        
        if (canPlay) StartCoroutine(Play());
        
        checkForTie();
    }

    public IEnumerator Play()
    {
        if (canPlay)
        {
            GameStateAI gsai = new GameStateAI();
            gsai.aiCards = new List<Card>(Cards);
            gsai.playerAndDeckCards = new List<Card>(GameManager.instance.player.Cards.Concat(deck.Cards).ToList());
            gsai.boardCards = new List<Card>(board.Cards);
            gsai.aiYakusCards = new List<Card>(yakus.Cards);
            gsai.playerYaskusCards = new List<Card>(GameManager.instance.player.yakus.Cards);
            AI.GameStateAI.ActionPossible actionPossible = gsai.Minimax().act;

            yield return ExecuteAction(actionPossible);

            // Au tour du joueur de jouer
            GameManager.instance.HandFinishTurn(this);


            if (canKoikoi) // Se met � jour tout seul dans GameManager.EndTurn
            {
                canKoikoi = false;
                // Affichage de la Décision du koikoi
                // Koikoi :
                // StartCoroutine(GameManager.instance.Koikoi(false));
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