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

    struct GameStateAI
    {
        public struct Action
        {
            public bool playerTarget;
            public bool pairDone;
            public Card handCard;
            public Card boardCard;
        }
        
        //HAND
        public List<Card> aiCards;
        public List<Card> boardCards;
        public List<Card> deckCards;
        public List<Card> playerCards;
        public List<Card> aiYakusCards;
        public List<Card> playerYaskusCards;

        public GameStateAI(GameStateAI previous)
        {
            playerCards = previous.playerCards;
            aiCards = previous.aiCards;
            boardCards = previous.boardCards;
            deckCards = previous.deckCards;
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
            
            if (!act.playerTarget)
            {
                aiCards.Remove(act.handCard);
            }
            if (act.pairDone)
            {
                if (act.playerTarget)
                {
                    playerYaskusCards.Add(act.handCard);
                    playerYaskusCards.Add(act.boardCard);
                }
                else
                {
                    aiYakusCards.Add(act.handCard);
                    aiYakusCards.Add(act.boardCard);
                }
                boardCards.Remove(act.boardCard);
            }
            else
            { 
                boardCards.Add(act.handCard);
            }
        }
        
        public  List<Action> GetActions(bool player) //false : IA | true : player
        {
            List<Card> hand;
            if (player)
            {
                hand = playerCards.Concat(deckCards).ToList();
            }
            else
            {
                hand = aiCards;
            }

            List<Action> actions = new List<Action>();
            for (int i = 0; i < hand.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < boardCards.Count; j++)
                {
                    if (hand[i].month == boardCards[j].month)
                    {
                        
                        Action act = new Action();
                        act.pairDone = true;
                        act.handCard = hand[i];
                        act.boardCard = boardCards[j];
                        act.playerTarget = player;
                        actions.Add(act);
                        found = true;
                    }
                }

                if (!found)
                {
                    Action act = new Action();
                    act.pairDone = false;
                    act.handCard = hand[i];
                    act.playerTarget = player;
                    actions.Add(act);
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
            if (depth <= 0)
            {
                MinimaxResult mmrTerminal = new MinimaxResult();
                mmrTerminal.score = Utility();
                return mmrTerminal;
            }

            List<Action> acts = GetActions(player);

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
            Debug.Log("L'IA joue !");

            GameStateAI gsai = new GameStateAI();
            gsai.aiCards = new List<Card>(this.Cards);
            gsai.playerCards = new List<Card>(GameManager.instance.player.Cards);
            gsai.aiYakusCards = new List<Card>(this.yakus.Cards);
            gsai.playerYaskusCards = new List<Card>(GameManager.instance.player.yakus.Cards);
            gsai.deckCards = new List<Card>(deck.Cards);
            gsai.boardCards = new List<Card>(board.Cards);
            AI.GameStateAI.Action act = gsai.Minimax(false, 2).act;

            ExecuteAction(act);
            
            // Au tour du joueur de jouer
            CanPlay(false);
            GameManager.instance.player.CanPlay(true);
        }
    }

    private void ExecuteAction(AI.GameStateAI.Action act)
    {
        
        act.handCard.GetUI().Display();
        
        if (act.pairDone)
        {
            // Anime les deux cartes vers la bonne zone Yakus
            AddCardToYakus(act.boardCard);
            AddCardToYakus(act.handCard);
            // Supprime les cartes de la main et du board
            board.RemoveCard(act.boardCard);
        }
        else
        { 
            AddCardToBoard(act.handCard);
        }
        RemoveCard(act.handCard);


    }
}
