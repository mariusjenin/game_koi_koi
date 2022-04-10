using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public enum Month
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }
    
    public enum Type
    {
        Hikari,
        Tane,
        Tan,
        Kasu
    }
    
    public enum Specificity
    {
        ClassicLight,
        RainyLight,
        FlowerLight,
        MoonLight,
        ClassicTane,
        SakeTane,
        BoarTane,
        DeerTane,
        ButterflyTane,
        RedTan,
        BlueTan,
        WrittenTan,
        ClassicKasu
    }

    public bool isSame(object obj)
    {
        //Check for null and compare run-time types.
        if (obj == null || ! GetType().Equals(obj.GetType()))
        {
            return false;
        } else {
            Card c = (Card) obj;
            return sprite != c.sprite && month == c.month && type == c.type && specificity == c.specificity;
        }
    }

    public Month month;
    public Type type;
    public Sprite sprite;
    public Specificity specificity;
    private UICard ui;

    public void SetUICard(UICard ui)
    {
        this.ui = ui;
    }
    public UICard GetUI()
    {
        return ui;
    }

}