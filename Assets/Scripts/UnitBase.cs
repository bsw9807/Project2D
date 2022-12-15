using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;


public enum ClassType
{
    CT_SwordMan,
    CT_Archer,
    CT_Guarder,
    CT_Wizard,
}

public enum AI_State
{
    AI_Idle,
    AI_Move,
    AI_Attack,

}

public class UnitBase : MonoBehaviour, IPoolObject
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private ClassType classType;
    [SerializeField]
    private string poolName;
    public string PoolName
    {
        get { return poolName; }
    }

    [SerializeField]
    private int maxHP;
    private int currentHP;
    private bool isAlive;
    private Vector3 move;

    [SerializeField]
    private float attackRate;
    private float attackDelay = 0f;
    [SerializeField]
    private float attackRange;

    private Animator animator;

    private AI_State state = AI_State.AI_Idle;
    private Vector3 rayDir = Vector3.zero;
    private int mask;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void InitData()
    {
        currentHP = maxHP;
        isAlive = true;
        state = AI_State.AI_Move;


        if (moveSpeed < 0f)//레드팀
        {
            rayDir.x = -1f;
            mask = 0 << LayerMask.NameToLayer("Blue");
        }
        else
        {
            rayDir.x = 1f;
            mask = 0 << LayerMask.NameToLayer("Red");
        }
    }

    private void Update()
    {
        switch (state)
        {
            case AI_State.AI_Move:
                Move_State();
                break;
            case AI_State.AI_Attack:
                Attack_State();
                break;
            case AI_State.AI_Idle:
                break;
        }
    }
    private void Move_State()
    {
        move.x = moveSpeed * Time.deltaTime;
        move.y = 0f;
        move.z = 0f;
        transform.Translate(move);
        CheckEnemy();
    }


    private void Attack_State()
    {
        attackDelay -= Time.deltaTime;
        if (attackDelay <= 0f)
        {
            attackDelay = attackRate;

            animator.SetTrigger("doAttack");

            // 공격 
        }
    }

    private void CheckEnemy()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, attackRange, mask);
        if (hit)
        {
            state = AI_State.AI_Attack;
            Debug.DrawRay(transform.position, rayDir, Color.green);
        }
        else
            Debug.DrawRay(transform.position, rayDir, Color.red);
    }



    #region POOLS
    public void OnCreatedInPool() // 오브젝트를 생성할때 
    {
    }

    public void OnGettingFromPool()
    {
        if (moveSpeed < 0f) // 레드팀
            transform.position = new Vector3(4.8f, 0f, 0f);
        else
            transform.position = new Vector3(-4.8f, 0f, 0f);

        InitData();
    }
    #endregion
}
