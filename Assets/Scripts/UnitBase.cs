using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitBase : MonoBehaviour
{
    public Animator animator;
    public GameObject prefHpBar;
    public GameObject canvas;
    public float hpBar_h;
    RectTransform hpBar;
    Image currentHPbar;

    public List<GameObject> FoundObjects;
    public GameObject target;
    float tDis;

    float atkDelay;

    public Vector2 dirVec;

    #region UnitStatus
    public string unitName; // 유닛 이름
    public float unitHP;    // 체력
    public float currentHP; // 현재 체력
    public float unitMS;    // 이동 속도
    public float unitAD;    // 공격력
    public float unitAS;    // 공격 속도
    public float unitDis;   // 공격 범위

    private void SetUnitStatus(string unitName, float unitHP, float unitMS, float unitAD, float unitAS, float unitDis)
    {
        unitName = this.unitName;
        unitHP = this.unitHP;
        currentHP = this.unitHP;
        unitMS = this.unitMS;
        unitAD = this.unitAD;
        unitAS = this.unitAS;
        unitDis = this.unitDis;
    }
    #endregion

    public enum UnitState
    {
        idle,
        run,
        attack,
        skill,
        stun,
        dead
    }

    void Start()
    {
        #region 01_Warrior_Status
        if (name.Equals("01_Warrior"))
        {
            SetUnitStatus("01_Warrior", 1000, 50, 100, 1, 10);
        }
        #endregion

        UnitHP();
        SetAS(unitAS);
        FindTarget();
    }

    void Update()
    {
        UnitHPUpdate();
        CheckState();
        UnitAI();
    }

    public UnitState unitState = UnitState.idle;

    void SetState(UnitState state)
    {
        unitState = state;
        switch (unitState)
        {
            case UnitState.idle:
                animator.SetFloat("Runstate", 0f);
                break;

            case UnitState.run:
                animator.SetFloat("Runstate", 0.5f);
                break;

            case UnitState.attack:
                animator.SetTrigger("Attack");
                animator.SetFloat("AttackState", 0.0f);
                animator.SetFloat("NormalState", 0.0f); // 0.0: Sword // 0.5: Bow // 1.0: Magic
                break;

            case UnitState.skill:
                animator.SetTrigger("Attack");
                animator.SetFloat("AttackState", 1.0f);
                animator.SetFloat("NormalState", 0.0f);
                break;

            case UnitState.stun:
                animator.SetFloat("Runstate", 1.0f);
                break;

            case UnitState.dead:
                animator.SetTrigger("Die");
                break;
        }
    }

    void CheckState()
    {
        switch (unitState)
        {
            case UnitState.idle:
                FindTarget();
                break;

            case UnitState.run:
                MoveToTarget();
                break;

            case UnitState.attack:
                AttackTarget();
                break;

            case UnitState.skill:
                break;

            case UnitState.stun:
                break;

            case UnitState.dead:
                onDie();
                break;
        }
    }

    void UnitHP()
    {
        hpBar = Instantiate(prefHpBar, canvas.transform).GetComponent<RectTransform>();
        currentHPbar = hpBar.transform.GetChild(0).GetComponent<Image>();
    }

    void UnitHPUpdate()
    {
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + hpBar_h, 0));
        hpBar.position = hpBarPos;
        currentHPbar.fillAmount = currentHP / unitHP;

        if(currentHP <= 0)
        {
            onDie();
        }
    }

    void SetAS(float AS)
    {
        animator.SetFloat("AttackSpeed", AS);
        unitAS = AS;
    }

    void UnitAI()
    {
        atkDelay -= Time.deltaTime;
        if(atkDelay < 0) atkDelay = 0;

        float dis = Vector3.Distance(transform.position, target.transform.position);

        if (atkDelay == 0 && CheckTarget())
        {
            FaceTarget();
            if (dis <= unitAD)
            {
                unitState = UnitState.attack;
            }
            else
            {
                unitState = UnitState.run;
            }
        }
        else
        {
            unitState = UnitState.idle;
        }
    }

    void FindTarget()
    {
        switch(gameObject.tag)
        {
            case "Friend":
                FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy")); break;
            case "Enemy":
                FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Friend")); break;
        }
        tDis = Vector2.Distance(gameObject.transform.position, FoundObjects[0].transform.position);

        target = FoundObjects[0];

        foreach (GameObject found in FoundObjects)
        {
            float Dis = Vector2.Distance(gameObject.transform.position, found.transform.position);

            if(Dis < tDis)
            {
                target = found;
                tDis = Dis;
            }
        }
    }

    void FaceTarget()   //타겟 바라보기
    {
        if (target.transform.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void AttackTarget() //타겟 공격
    {
        target.GetComponent<UnitBase>().currentHP -= unitAD;
        unitState = UnitState.attack;
    }

    void MoveToTarget() //타겟으로 이동
    {
        Vector2 dir = (Vector2)(target.transform.localPosition - transform.position);
        dirVec = dir.normalized;
        transform.position += (Vector3)dirVec * unitMS * Time.deltaTime;
        unitState = UnitState.run;
    }

    void onDie() // 유닛 사망 후 실행
    {
        unitState = UnitState.dead;
        Destroy(gameObject, 3);
        Destroy(hpBar.gameObject, 3);
    }

    bool CheckTarget() //타겟 존재 여부 체크
    {
        if (target == null) return false;
        if(target.GetComponent<UnitBase>().unitState == UnitState.dead) return false;
        if (!target.activeInHierarchy) return false;

        return true;
    }
}
