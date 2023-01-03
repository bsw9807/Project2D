using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ButtonManager : MonoBehaviour
{
    public GameObject gamePausePanel;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

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
        LoadingSceneManager.LoadScene("Scene_Lobby");
        Debug.Log("로비로 이동");
    }

    public void TryAgainBtn()
    {
        Debug.Log("준비로 이동");
        LoadingSceneManager.LoadScene("Scene_Ready");
    }

    public void GoToNextBtn()
    {
        Debug.Log("배틀2로 이동");
        LoadingSceneManager.LoadScene("Scene_Battle2");
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
                break;
            case 2:
                Time.timeScale = 3;
                break;
            case 3:
                Time.timeScale = 1;
                break;
        }
    }
}
