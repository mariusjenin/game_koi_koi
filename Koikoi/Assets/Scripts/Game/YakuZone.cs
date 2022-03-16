namespace Game
{
    public class ZoneYakus : CardZone
    {
        public int EvaluateScore()
        {
            int score = 0;
            
            //NB TYPE
            int nbLight = 0;
            int nbTane = 0;
            int nbTan = 0;
            int nbKasu = 0;
            
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

            int sizeCards = Cards.Count;
            for (int i = 0; i < sizeCards; i++)
            {
                Card currCard = Cards[i];
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
            if (nbLight == 5) score += 10;
            else if (nbLight == 4) score += hasRainyLight ? 7 : 8;
            else if (nbLight == 3 && !hasRainyLight) score += 5;
            
            //ZAKE YAKUS
            if (hasSakeTane && hasFlowerLight) score += 5;
            if (hasSakeTane && hasMoonLight) score += 5;
            
            //TANE YAKUS
            if (hasBoarTane && hasButterflyTane && hasDeerTane) score += 5 - (nbTane - 3);
            if (nbTane >=5) score += 5 - (nbTane - 5);
            
            //TAN YAKUS
            if (nbBlueTan == 3 && nbWrittenTan == 3) score += 10;
            else if (nbBlueTan == 3) score += 5 + (nbTan-3);
            else if (nbWrittenTan == 3)score += 5 + (nbTan-3);
            if (nbTan >= 5) score += 1 + (nbTan - 5);
            
            //KASU YAKU
            if (nbKasu >= 10) score += 1 + (nbKasu - 1);
            
            return score;
        }
    }
}