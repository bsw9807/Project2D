using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public TextMeshProUGUI[] timeText;

    private float time = 90;
    int min, sec;

    void Start()
    {
    }

    void Update()
    {
        BattleTimer();
    }

    private void BattleTimer()
    {
        time -= Time.deltaTime;

        min = (int)time / 60;
        sec = ((int)time - min * 60) % 60;

        if (min <= 0 && sec <= 0)
        {
            timeText[0].text = "00";
            timeText[1].text = "00";
        }
        else
        {
            if (sec >= 60)
            {
                min += 1;
                sec -= 60;
            }
            else
            {
                timeText[0].text = "0" + min.ToString();
                if (sec <= 9) timeText[1].text = "0" + sec.ToString();
                else timeText[1].text = sec.ToString();
            }
        }
    }
}
