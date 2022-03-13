using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    private bool visible;
    private Card card;
    private Image image;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        image = GetComponent<Image>();
    }
    public void Init(bool visible, Card card)
    {
        this.visible = visible;
        this.card = card;
    }

    public void Display()
    {
        if (visible) image.sprite = card.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick()
    {
        Debug.Log(card.id);
    }
}
