using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Game
{
    public class ZoneYakus : CardZone
    {
        // Grids
        public GameObject HikariGrid;
        public GameObject TaneGrid;
        public GameObject TanGrid;
        public GameObject KasuGrid;

        public ScoreManager scoreManager;

        public int GetScore()
        {
            scoreManager.SetCards(Cards);
            int score = scoreManager.EvaluateScore();
            return score;
        }
    }
}