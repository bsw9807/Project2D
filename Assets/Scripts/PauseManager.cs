using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject gamePausePanel;
    bool isPause;

    // Start is called before the first frame update
    void Start()
    {
        gamePausePanel.SetActive(false);
        isPause = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PauseBtn()
    {
        if (! isPause)
        {
            gamePausePanel.SetActive(true);
            Time.timeScale = 1;
            isPause = true;
            return;
        }
        if (isPause)
        {
            gamePausePanel.SetActive(false);
            Time.timeScale = 0;
            isPause = false;
            return;
        }
    }

    public void ContinueBtn()
    {
        if (! isPause)
        {
            gamePausePanel.SetActive(true);
            Time.timeScale = 1;
            isPause = true;
            return;
        }
        if (isPause)
        {
            gamePausePanel.SetActive(false);
            Time.timeScale = 0;
            isPause = false;
            return;
        }
    }

    public void GoToMainBtn()
    {

    }

    public void TryAgainBtn()
    {

    }
}
