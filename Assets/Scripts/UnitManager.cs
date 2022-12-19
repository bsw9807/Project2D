using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public float findTimer;

    public List<Transform> unitPool = new List<Transform>();
    public List<UnitBase> fUnitList = new List<UnitBase>();
    public List<UnitBase> eUnitList = new List<UnitBase>();
    
    void Awake()
    {
        SoonsoonData.Instance.unitManager = this;
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

    public UnitBase GetTarget(UnitBase unit)
    {
        UnitBase tUnit = null;

        List<UnitBase> tList = new List<UnitBase>();
        switch (unit.tag)
        {
            case "Friend": tList = eUnitList; break;
            case "Enemy": tList = fUnitList; break;
        }

        float tSDis = 999999;

        for(var i = 0; i < tList.Count; i++)
        {
            float tDis = ((Vector2)tList[0].transform.localPosition - (Vector2)unit.transform.localPosition).sqrMagnitude;
            if(tDis <= unit.unitAR * unit.unitAR)
            {
                if (tList[i].gameObject.activeInHierarchy)
                {
                    if (tList[i].unitState != UnitBase.UnitState.death)
                    {
                        if (tDis < tSDis)
                        {
                            tUnit = tList[i];
                            tSDis = tDis;
                        }
                    }
                }
            }
        }
        return tUnit;
    }
}
