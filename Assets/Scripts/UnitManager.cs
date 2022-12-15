using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class UnitManager : MonoBehaviour
{
    private static UnitManager inst;
    public static UnitManager Inst
    {
        get { return inst; }
    }

    private PoolManager poolManager;
    private void Awake()
    {
        if (inst)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            inst = this;
            poolManager = GetComponent<PoolManager>();
        }
    }

    public void SpawnUnit(int unitIndex)
    {
        UnitBase newUnit = poolManager.GetFromPool<UnitBase>(unitIndex);
    }
    public void ReturnPool(UnitBase unit)
    {
        poolManager.TakeToPool<UnitBase>(unit.PoolName, unit);
    }
}
