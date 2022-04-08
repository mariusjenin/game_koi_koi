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

        public ScoreManager score;

        private int mScore;
        public void UpdateScore()
        {
            this.score.SetCards(Cards);
            int newScore = score.EvaluateScore();
            mScore += newScore;
        }
        public int GetScore(bool shouldUpdate)
        {
            if (shouldUpdate) UpdateScore();
            return mScore;
        }
    }
}