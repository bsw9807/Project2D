using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    TextMeshProUGUI textMP;

    public float textSpeed;
    public float destroyTime;
    public float damage;

    void Start()
    {
        textMP = GetComponent<TextMeshProUGUI>();
        Destroy(gameObject, destroyTime);
        Debug.Log(damage);
        textMP.text = damage.ToString();
    }

    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * textSpeed);
    }

}