using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGamePopUp : MonoBehaviour
{
    public TextMeshProUGUI PlayerScore;
    public TextMeshProUGUI AIScore;
    public TextMeshProUGUI Result;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(int playerScore, int aiScore)
    {
        PlayerScore.SetText(""+playerScore);
        AIScore.SetText("" + aiScore);

        string winner = playerScore > aiScore ? "Player" : "AI";
        Result.SetText(winner + " won the game");

        gameObject.SetActive(true);

        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        yield return null;
    }
}
