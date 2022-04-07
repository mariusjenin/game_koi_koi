using System.Collections.Generic;

namespace Game
{
    public class ScoreManager
    {

        public List<Card> cards;
        public List<Yaku> yakus;
        public int nbLight;
        public int nbTane;
        public int nbTan;
        public int nbKasu;
        public enum Yaku
        {
            //Lights
            Goko,
            Shiko,
            AmeShiko,
            Sanko,
            //Sake
            HanamiSake,
            TsukimiSake,
            //Tane
            Inoshikacho,
            Tane,
            //Tan
            AotanAkatanNoChofuku,
            Aotan,
            Akatan,
            Tanzaku,
            //Kasu
            Kasu,
            
        }

        public ScoreManager(List<Card> c)
        {
            yakus = new List<Yaku>();
            cards = c;
        }
          public void UpdateYakus()
        {
            yakus.Clear();
            
            //NB TYPE
            nbLight = 0;
            nbTane = 0;
            nbTan = 0;
            nbKasu = 0;
            
            //SPECIFIC CARDS
            bool hasFlowerLight = false;
            bool hasRainyLight = false;
            bool hasMoonLight = false;
            bool hasBoarTane = false;
            bool hasButterflyTane = false;
            bool hasDeerTane = false;
            bool hasSakeTane = false;
            
            //TAN SPECIFIC TYPE
            int nbRedTan = 0;
            int nbBlueTan = 0;
            int nbWrittenTan = 0;

            int sizeCards = cards.Count;
            for (int i = 0; i < sizeCards; i++)
            {
                Card currCard = cards[i];
                if (!hasFlowerLight && currCard.specificity == Card.Specificity.FlowerLight) hasFlowerLight = true;
                if (!hasRainyLight && currCard.specificity == Card.Specificity.RainyLight) hasRainyLight = true;
                if (!hasMoonLight && currCard.specificity == Card.Specificity.MoonLight) hasMoonLight = true;
                if (!hasBoarTane && currCard.specificity == Card.Specificity.BoarTane) hasBoarTane = true;
                if (!hasButterflyTane && currCard.specificity == Card.Specificity.ButterflyTane) hasButterflyTane = true;
                if (!hasDeerTane && currCard.specificity == Card.Specificity.DeerTane) hasDeerTane = true;
                if (!hasSakeTane && currCard.specificity == Card.Specificity.SakeTane) hasSakeTane = true;
                if (currCard.type == Card.Type.Hikari) nbLight++;
                if (currCard.type == Card.Type.Tane) nbTane++;
                if (currCard.specificity == Card.Specificity.RedTan) nbRedTan++;
                if (currCard.specificity == Card.Specificity.BlueTan) nbBlueTan++;
                if (currCard.specificity == Card.Specificity.WrittenTan) nbWrittenTan++;
                if (currCard.type == Card.Type.Kasu) nbKasu++;
            }
            nbTan = nbRedTan + nbBlueTan + nbWrittenTan;

            //HIKARI YAKUS
            if (nbLight == 5) yakus.Add(Yaku.Goko);
            else if (nbLight == 4)
            {
                if (hasRainyLight) yakus.Add(Yaku.AmeShiko);
                else yakus.Add(Yaku.Shiko);
            }
            else if (nbLight == 3 && !hasRainyLight)yakus.Add(Yaku.Sanko);
            
            //ZAKE YAKUS
            if (hasSakeTane && hasFlowerLight)yakus.Add(Yaku.HanamiSake);
            if (hasSakeTane && hasMoonLight) yakus.Add(Yaku.TsukimiSake);
            
            //TANE YAKUS
            if (hasBoarTane && hasButterflyTane && hasDeerTane) yakus.Add(Yaku.Inoshikacho);
            if (nbTane >=5) yakus.Add(Yaku.Tane);
            
            //TAN YAKUS
            if (nbBlueTan == 3 && nbWrittenTan == 3)yakus.Add(Yaku.AotanAkatanNoChofuku);
            else if (nbBlueTan == 3)yakus.Add(Yaku.Aotan);
            else if (nbWrittenTan == 3)yakus.Add(Yaku.Akatan);
            if (nbTan >= 5) yakus.Add(Yaku.Tanzaku);
            
            //KASU YAKU
            if (nbKasu >= 10) yakus.Add(Yaku.Kasu);
        }
        
        public int EvaluateScore()
        {
            UpdateYakus();
            int score = 0;

            for (int i = 0; i <  yakus.Count; i++)
            {
                switch (yakus[i])
                {
                    case Yaku.Goko:
                        score += 10;
                        break;
                    case Yaku.Shiko:
                        score += 8;
                        break;
                    case Yaku.AmeShiko:
                        score += 7;
                        break;
                    case Yaku.Sanko:
                        score += 5;
                        break;
                    case Yaku.HanamiSake:
                        score += 5;
                        break;
                    case Yaku.TsukimiSake:
                        score += 5;
                        break;
                    case Yaku.Inoshikacho:
                        score += 5 - (nbTane - 3);
                        break;
                    case Yaku.Tane:
                        score += 5 - (nbTane - 5);
                        break;
                    case Yaku.AotanAkatanNoChofuku:
                        score += 10;
                        break;
                    case Yaku.Aotan:
                    case Yaku.Akatan:
                        score += 5 + (nbTan-3);
                        break;
                    case Yaku.Tanzaku:
                        score += 1 + (nbTan - 5);
                        break;
                    case Yaku.Kasu:
                        score += 1 + (nbKasu - 1);
                        break;
                }
            }
            return GameManager.instance.koikoi ? score * 2 : score;
        }
        
        
    }
}