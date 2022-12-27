using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private float destroyTime;
    TextMeshPro text;
    public float damage;

    void Start()
    {
        destroyTime = 0.5f;
        text = GetComponent<TextMeshPro>();
        text.text = damage.ToString();
        Invoke("DestroyObj", destroyTime);
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}
