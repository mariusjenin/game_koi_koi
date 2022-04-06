using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KoiKoiPopUp : MonoBehaviour
{
    public GameObject ImagesGrid;
    public GameObject ButtonsGrid;

    private void Awake()
    {

        gameObject.SetActive(false);
        SetPopUpOpacity(0);
    }

    public void Display()
    {
        gameObject.SetActive(true);
        StartCoroutine(Fade(0f, 1f, 0.2f));
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
    }

    private void SetPopUpOpacity(float a)
    {
        foreach (Transform child in ImagesGrid.transform)
        {
            SetImageOpacity(child.GetComponent<Image>(), a);
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
