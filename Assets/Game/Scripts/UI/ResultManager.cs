using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ResultManager : MonoBehaviour
{
    public GameObject resultPanel;
    public Text resultText;

    public faghtingController[] fightingController;
    public OpponentAI[] opponentAI;

    void Update()
    {
        foreach(faghtingController fightingController in fightingController)
        {
            if(fightingController.gameObject.activeSelf && fightingController.currentHealth <= 0)
            {
                SetResult("You Lose!");
                return;
            }
        }

        foreach (OpponentAI opponentAI in opponentAI)
        {
            if (opponentAI.gameObject.activeSelf && opponentAI.currentHealth <= 0)
            {
                SetResult("You Win!");
                return;
            }
        }
    }

    void SetResult(string result)
    {
        resultText.text = result;
        resultPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


}
