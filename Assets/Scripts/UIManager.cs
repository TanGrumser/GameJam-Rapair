using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour {


    public TextMeshProUGUI[] coinTexts;
    public GameObject[] inventory;
    public GameObject[] costTexts;
    public TextMeshProUGUI[] playerScores;
    public TextMeshProUGUI fillBlue;
    public TextMeshProUGUI fillRed;


    public void ChangeIterator(int pl, int gadgetIterator){
        Transform[] highlights = inventory[pl].GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < 4; i++) {
            highlights[i + 1].gameObject.SetActive(i == gadgetIterator);
        }

    }

    public void UpdateFillBlue(float progress) {
        int repairs = (int) (progress / GameManager.instance.gameSettings.workprogressForWorker);
        int neededRepairs = (int)(1f / GameManager.instance.gameSettings.workprogressForWorker) + 1;
        fillBlue.text = repairs + "/" + neededRepairs;
    }

    public void UpdateFillRed(float progress) {
        int repairs = (int)(progress / GameManager.instance.gameSettings.workprogressForWorker);
        int neededRepairs = (int)(1f / GameManager.instance.gameSettings.workprogressForWorker) + 1;
        fillRed.text = repairs + "/" + neededRepairs;
    }

    public void UpdateCoinTexts(int index, int amountCoins) {
        coinTexts[index].text = amountCoins.ToString();
    }

    public void UpdateScore()
    {

        string scoreBlue = "Score: " + TotalGameScore.scoreBlue;
        string scoreRed = "Score: " + TotalGameScore.scoreRed;

        playerScores[0].text = scoreBlue;
        playerScores[1].text = scoreRed;



    }


    public void GameFinishedButton() {
        SceneManager.LoadScene(0);
    }


    public void SetInventoryCosts() {
        //Debug.Log("nreoieoi");
        foreach(GameObject holder in costTexts) {
            TextMeshProUGUI[] texts = holder.GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++) {
                texts[i].text = GameManager.instance.gameSettings.itemPrices[i].ToString();
            }

        }
    }

    


}
