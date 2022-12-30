using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    public string SceneName;

    public void goScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
