using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        Camera cam = GetComponent<Camera>();
        Rect rt = cam.rect;
        float scale_Height = ((float)Screen.width / Screen.height) / ((float)16 / 9);
        float scale_width = 1f / scale_Height;

        if (scale_Height < 1)
        {
            rt.height = scale_Height;
            rt.y = (1f - scale_width) / 2f;
        }
        else
        {
            rt.width = scale_width;
            rt.x = (1f - scale_width) / 2f;
        }
        cam.rect = rt;
    }
}
