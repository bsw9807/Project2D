using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<Transform> unitPool = new List<Transform>();
    public List<UnitBase> fUnitList = new List<UnitBase>();
    public List<UnitBase> eUnitList = new List<UnitBase>();

    void Awake()
    {
    }

    void Start()
    {
        SetUnitList();
    }

    void Update()
    {

    }

    void SetUnitList()
    {
        fUnitList.Clear();
        eUnitList.Clear();

        for (var i = 0; i < unitPool.Count; i++)
        {
            for (var j = 0; j < unitPool[i].childCount; j++)
            {
                switch (i)
                {
                    case 0:
                        fUnitList.Add(unitPool[i].GetChild(j).GetComponent<UnitBase>());
                        unitPool[i].GetChild(j).gameObject.tag = "Friend";
                        break;
                    case 1:
                        eUnitList.Add(unitPool[i].GetChild(j).GetComponent<UnitBase>());
                        unitPool[i].GetChild(j).gameObject.tag = "Enemy";
                        break;
                }
            }
        }
    }

}
