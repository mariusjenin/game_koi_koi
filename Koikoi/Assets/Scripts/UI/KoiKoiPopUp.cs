using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KoiKoiPopUp : MonoBehaviour
{
    public GameObject ImagesGrid;
    public GameObject ButtonsGrid;
    public TextMeshProUGUI Message;

    private GameObject EndImage;
    private GameObject KoiKoiImage;

    private GameObject EndButton;
    private GameObject KoiKoiButton;

    public enum Type { PLAYER, KOIKOI, END};

    private void Awake()
    {
        EndImage = ImagesGrid.transform.GetChild(0).gameObject;
        KoiKoiImage = ImagesGrid.transform.GetChild(1).gameObject;
        EndButton = ButtonsGrid.transform.GetChild(0).gameObject;
        KoiKoiButton = ButtonsGrid.transform.GetChild(1).gameObject;

        gameObject.SetActive(false);
        SetPopUpOpacity(0);
    }

    public void Show(Type type, Hand hand)
    {
        string player;
        GameManager.instance.FadeInGame();
        gameObject.SetActive(true);
        EndButton.GetComponent<Button>().enabled = true;
        KoiKoiButton.GetComponent<Button>().enabled = true;

        switch (type)
        {
            case Type.PLAYER:
                EndImage.SetActive(true);
                EndButton.SetActive(true);
                KoiKoiImage.SetActive(true);
                KoiKoiButton.SetActive(true);
                Message.gameObject.SetActive(false);
                break;

            case Type.KOIKOI:
                Message.gameObject.SetActive(true);
                KoiKoiImage.SetActive(true);
                EndImage.SetActive(false);
                EndButton.SetActive(false);
                KoiKoiButton.SetActive(false);

                player = (hand is Player) ? "you" : "the AI";
                Message.SetText("Koikoi declared by " + player + ".");

                StartCoroutine(HideAfterSeconds(1.5f));
                break;

            case Type.END:
                Message.gameObject.SetActive(true);
                EndImage.SetActive(true);
                EndButton.SetActive(false);
                KoiKoiImage.SetActive(false);
                KoiKoiButton.SetActive(false);

                player = (hand is Player) ? "You" : "The AI";
                Message.SetText(player + " ended the turn.");

                StartCoroutine(HideAfterSeconds(1.5f));
                break;
        }
        StartCoroutine(Fade(0f, 1f, 0.2f));
    }

    public void Hide()
    {
        GameManager.instance.FadeOutGame();
        EndButton.GetComponent<Button>().enabled = false;
        KoiKoiButton.GetComponent<Button>().enabled = false;
        StartCoroutine(Fade(1f, 0f, 0.2f));

    }

    private IEnumerator HideAfterSeconds(float duration)
    {
        float timer = 0f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Hide();
        GameManager.instance.ActivateButtons();
    }

    public void onClickEndTurn()
    {
        Hide();
        GameManager.instance.NextTurn();
    }

    public void onClickKoiKoi()
    {
        Debug.Log("test");
    }

    private IEnumerator Fade(float start, float end, float duration)
    {
        float timer = 0f;
        AnimationCurve smoothCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });

        while (timer <= duration)
        {
            timer += Time.deltaTime;
            SetPopUpOpacity(Mathf.Lerp(start, end, smoothCurve.Evaluate(timer / duration)));
            yield return new WaitForSeconds(0.01f);
        }

        // Si on cherche � cacher l'UI, � la fin on le d�sactive
        if(end == 0f) gameObject.SetActive(false);
    }

    private void SetPopUpOpacity(float a)
    {
        Message.color = new Color(Message.color.r, Message.color.g, Message.color.b, a);

        foreach (Transform child in ImagesGrid.transform)
        {
            SetImageOpacity(EndImage.GetComponent<Image>(), a);
            SetImageOpacity(KoiKoiImage.GetComponent<Image>(), a);
        }

        foreach (Transform child in ButtonsGrid.transform)
        {
            SetImageOpacity(child.GetComponent<Image>(), a);
            SetTextButtonOpacity(child.GetComponent<Button>(), a);
        }
    }

    private void SetImageOpacity(Image image, float a)
    {
        Color c = image.color;
        image.color = new Color(c.r, c.g, c.b, a);
    }

    private void SetTextButtonOpacity(Button button, float a)
    {
        TextMeshProUGUI tmpro = button.GetComponentInChildren<TextMeshProUGUI>();
        Color c = tmpro.color;
        tmpro.color = new Color(c.r, c.g, c.b, a);
    }
}
