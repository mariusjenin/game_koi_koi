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

        public int getScore()
        {
            this.score.SetCards(Cards);
            int s = score.EvaluateScore();
            Debug.Log(s);
            return s;
        }
    }
}