using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ButtonManager : MonoBehaviour
{
    public GameObject gamePausePanel;
    public TextMeshProUGUI gameSpeedText;

    public void PauseBtn()
    {
        gamePausePanel.SetActive(true);
        Time.timeScale = 0;
        return;
    }

    public void ContinueBtn()
    {
        gamePausePanel.SetActive(false);
        Time.timeScale = 1;
        return;
    }

    public void GoToMainBtn()
    {
        Time.timeScale = 1;
        LoadingSceneManager.LoadScene("Scene_Lobby");
    }

    public void TryAgainBtn()
    {
        Time.timeScale = 1;
        LoadingSceneManager.LoadScene("Scene_Ready");
    }

    public void GoToNextBtn()
    {
        Time.timeScale = 1;
        LoadingSceneManager.LoadScene("Scene_Battle");
    }

    public void GoToBattleBtn()
    {
        Time.timeScale = 1;
        LoadingSceneManager.LoadScene("Scene_Battle");
    }

    public void TipBtn()
    {
    }

    public void GameSpeedBtn()
    {
        switch (Time.timeScale)
        {
            case 1:
                Time.timeScale = 2;
                gameSpeedText.text = "X2";
                break;
            case 2:
                Time.timeScale = 3;
                gameSpeedText.text = "X3";
                break;
            case 3:
                Time.timeScale = 1;
                gameSpeedText.text = "X1";
                break;
        }
    }
}
