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
            public ActionPart actPart1;
            public ActionPart actPart2;
        }
        
        public struct ActionPart
        {
            public bool pairDone;
            public Card card1;
            public Card card2;
        }
        
        //HAND
        public List<Card> aiCards;
        public List<Card> boardCards;
        public List<Card> playerAndDeckCards;
        public List<Card> deckCards;
        public List<Card> aiYakusCards;
        public List<Card> playerYaskusCards;

        public GameStateAI(GameStateAI previous)
        {
            playerAndDeckCards = previous.playerAndDeckCards;
            deckCards = previous.deckCards;
            aiCards = previous.aiCards;
            boardCards = previous.boardCards;
            aiYakusCards = previous.aiYakusCards;
            playerYaskusCards = previous.playerYaskusCards;
        }
        
        public int Utility()
        {
            ScoreManager aism = new ScoreManager(aiYakusCards);
            ScoreManager playersm = new ScoreManager(playerYaskusCards);
            aism.UpdateYakus();
            playersm.UpdateYakus();
            return aism.EvaluateScore() - playersm.EvaluateScore();
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
        
        public  List<Action> GetActions(bool player) //false : IA | true : player
        {
            List<Card> handPart1 = player?playerAndDeckCards:aiCards;
            List<Card> handPart2 = player?playerAndDeckCards:deckCards;

            List<Action> actions = new List<Action>();
            List<ActionPart> actionParts1 = new List<ActionPart>();
            List<ActionPart> actionParts2 = new List<ActionPart>();
            
            //Part 1
            for (int i = 0; i < handPart1.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < boardCards.Count; j++)
                {
                    if (handPart1[i].month == boardCards[j].month)
                    {
                        
                        ActionPart actPart1 = new ActionPart();
                        actPart1.pairDone = true;
                        actPart1.card1 = handPart1[i];
                        actPart1.card2 = boardCards[j];
                        actionParts1.Add(actPart1);
                        found = true;
                    }
                }

                if (!found)
                {
                    ActionPart actPart1 = new ActionPart();
                    actPart1.pairDone = false;
                    actPart1.card1 = handPart1[i];
                    actionParts1.Add(actPart1);
                }
            }
            
            //Part 2
            for (int i = 0; i < handPart2.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < boardCards.Count; j++)
                {
                    if (handPart2[i].month == boardCards[j].month)
                    {
                        
                        ActionPart actPart2 = new ActionPart();
                        actPart2.pairDone = true;
                        actPart2.card1 = handPart2[i];
                        actPart2.card2 = boardCards[j];
                        actionParts2.Add(actPart2);
                        found = true;
                    }
                }

                if (!found)
                {
                    ActionPart actPart2 = new ActionPart();
                    actPart2.pairDone = false;
                    actPart2.card1 = handPart2[i];
                    actionParts2.Add(actPart2);
                }
            }

            //Combining the both part TODO à revoir car cette fâçon n'explore pas toutes les possibilités
            for (int i = 0; i < actionParts1.Count; i++)
            {
                for (int j = 0; j < actionParts2.Count; j++)
                {
                    if (actionParts1[i].card1 != actionParts2[j].card1 && actionParts1[i].card2 != actionParts2[j].card2)
                    {
                        Action act = new Action();
                        act.actPart1 = actionParts1[i];
                        act.actPart2 = actionParts2[j];
                        actions.Add(act);
                    }
                }
            }
            return actions;
        }


        public struct MinimaxResult
        {
            public int score;
            public Action act;
        }
        public MinimaxResult Minimax(bool player, int depth)
        {
            List<Action> acts = GetActions(player);
            
            if (depth <= 0 || acts.Count <= 0)
            {
                MinimaxResult mmrTerminal = new MinimaxResult();
                mmrTerminal.score = Utility();
                return mmrTerminal;
            }


            int score = player ? -2147483648 : 2147483647;
            MinimaxResult mmr = new MinimaxResult();
            mmr.score = score;
            for (int i = 0; i < acts.Count; i ++)
            {
                GameStateAI gsai = new GameStateAI(this);
                gsai.ApplyAction(acts[i]);

                MinimaxResult currMmr = gsai.Minimax(!player,depth-1);
                currMmr.act = acts[i];
                if (player)
                {
                    if (currMmr.score > mmr.score) mmr = currMmr;
                }
                else
                {
                    if (currMmr.score < mmr.score) mmr = currMmr;
                }
            }
            return mmr;
        }
        
    }
    
    public override void CanPlay(bool canPlay)
    {
        base.CanPlay(canPlay);
        if(canPlay) Play();
    }
    public void Play()
    {
        if(canPlay)
        {
            if(canKoikoi) // Se met � jour tout seul dans GameManager.EndTurn
            {
                // Affichage de la D�cision du koikoi
                // Koikoi :
                // StartCoroutine(GameManager.instance.Koikoi(false))
                //
                // Fin de tour :
                // StartCoroutine(GameManager.instance.NextTurn(false));
            }

            GameStateAI gsai = new GameStateAI();
            gsai.aiCards = new List<Card>(this.Cards);
            gsai.playerAndDeckCards = new List<Card>(GameManager.instance.player.Cards.Concat(deck.Cards).ToList());
            gsai.deckCards = new List<Card>(deck.Cards);
            gsai.aiYakusCards = new List<Card>(this.yakus.Cards);
            gsai.playerYaskusCards = new List<Card>(GameManager.instance.player.yakus.Cards);
            gsai.boardCards = new List<Card>(board.Cards);
            AI.GameStateAI.Action act = gsai.Minimax(false, 1).act;

            ExecuteAction(act);

            // Au tour du joueur de jouer
            GameManager.instance.HandFinishTurn(this);
        }
    }

    private void ExecuteAction(AI.GameStateAI.Action act)
    {
        
        //PART 1
        act.actPart1.card1.GetUI().Display();
        if (act.actPart1.pairDone)
        {
            // Anime les deux cartes vers la bonne zone Yakus
            AddCardToYakus(act.actPart1.card2);
            AddCardToYakus(act.actPart1.card1);
            // Supprime les cartes de la main et du board
            board.RemoveCard(act.actPart1.card2);
        }
        else
        {
            AddCardToBoard(act.actPart1.card1);
        }
        RemoveCard(act.actPart1.card1);
        
        //PART 2
        // Création d'un template image à la position du deck
        GameObject gObject = Instantiate(GameManager.instance.template, GameManager.instance.deck.transform.position, GameManager.instance.deck.transform.rotation, GameManager.instance.deck.transform.parent.transform);
        UICard uiCard = gObject.GetComponentInChildren<UICard>();
        act.actPart2.card1.SetUICard(uiCard);
        uiCard.Init(this, act.actPart2.card1, gObject.GetComponent<Canvas>());
        act.actPart2.card1.GetUI().Display();
        if (act.actPart2.pairDone)
        {
            // Anime les deux cartes vers la bonne zone Yakus
            AddCardToYakus(act.actPart2.card2);
            AddCardToYakus(act.actPart2.card1);
            // Supprime les cartes de la main et du board
            board.RemoveCard(act.actPart2.card2);
        }
        else
        {
            AddCardToBoard(act.actPart2.card1);
        }
        GameManager.instance.deck.RemoveCard(act.actPart2.card1);


    }
}
