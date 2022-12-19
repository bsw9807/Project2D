using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitBase : MonoBehaviour
{
    public SPUM_Prefabs spumPref;

    public enum UnitState
    {
        idle,
        run,
        attack,
        skill,
        stun,
        death
    }

    public UnitState unitState = UnitState.idle;

    public UnitBase target;

    public float unitHP; //체력
    public float unitMS; //이동속도
    public float unitFR; //프레임
    public float unitAT; //공격력
    public float unitAS; //공격속도
    public float unitAR; //공격범위

    // Move
    public Vector2 dirVec;

    public float findTimer;

    void Start()
    {
        
    }

    void Update()
    {
        findTimer += Time.deltaTime;
        if (findTimer > SoonsoonData.Instance.unitManager.findTimer)
        {
            CheckState();
            findTimer = 0;
        }
    }

    void SetInitState()
    {
        unitState = UnitState.idle;
    }

    void CheckState()
    {
        switch (unitState)
        {
            case UnitState.idle:
                FindTarget();
                break;

            case UnitState.run:
                FindTarget();
                DoMove();
                break;

            case UnitState.attack:
                break;

            case UnitState.skill:
                break;

            case UnitState.stun:
                break;

            case UnitState.death:
                break;
        }
    }

    void SetState(UnitState state)
    {
        unitState = state;
        switch (unitState)
        {
            case UnitState.idle:
                spumPref.PlayAnimation(state.ToString("0"));
                break;

            case UnitState.run:
                spumPref.PlayAnimation(state.ToString("1"));
                break;

            case UnitState.attack:
                spumPref.PlayAnimation(state.ToString("4"));//4,7 Sword 5,8 Bow 6,9 Magic
                break;

            case UnitState.skill:
                spumPref.PlayAnimation(state.ToString("7"));
                break;

            case UnitState.stun:
                spumPref.PlayAnimation(state.ToString("3"));
                break;

            case UnitState.death:
                spumPref.PlayAnimation(state.ToString("2"));
                break;
        }
    }

    void FindTarget()
    {
        target = SoonsoonData.Instance.unitManager.GetTarget(this);
    }

    void DoMove()
    {
        if (CheckTarget()) return;

        Vector2 tVec = (Vector2)(target.transform.localPosition - transform.position);
        dirVec = tVec.normalized;

        transform.position += (Vector3)dirVec * unitMS * Time.deltaTime;
    }

    bool CheckTarget()
    {
        if (target == null) return false;
        if (target.unitState == UnitState.death) return false;
        if (target.gameObject.activeInHierarchy) return false;

        return true;
    }
}
