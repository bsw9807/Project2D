using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitBase : MonoBehaviour
{
    public List<GameObject> FoundObjects;
    public GameObject target;
    public float tDis;
    public Vector2 dirVec;

    public Animator animator;
    public GameObject prefHpBar;
    public GameObject prefMpBar;
    public GameObject canvas;
    public float hpBar_h;
    public float mpBar_h;
    RectTransform hpBar;
    RectTransform mpBar;
    Image currentHPbar;
    Image currentMPbar;

    public float atkDelay;  // 공격 딜레이

    public string unitName; // 유닛 이름
    public float unitHP;    // 체력
    public float currentHP; // 현재 체력
    public float unitMP;    // 마나
    public float currentMP; // 현재 마나
    public float unitMS;    // 이동 속도
    public float unitAD;    // 공격력
    public float unitAS;    // 공격 속도
    public float unitDis;   // 공격 범위
    public string unitType;  // 유닛 타입

    public enum UnitState
    {
        idle,
        run,
        attack,
        skill,
        stun,
        dead
    }

    public enum UnitType
    {
        Warrior,
        Archer,
        Magician,
        Priest,
        Assassin
    }           // 미적용

    void UnitHpbar()
    {
        hpBar = Instantiate(prefHpBar, canvas.transform).GetComponent<RectTransform>();
        currentHPbar = hpBar.transform.GetChild(0).GetComponent<Image>();
    }

    void UnitHpbarUpdate()
    {
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + hpBar_h, 0));
        hpBar.position = hpBarPos;
        currentHPbar.fillAmount = currentHP / unitHP;

        if (currentHP <= 0)
        {
            unitState = UnitState.dead;
        }
    }

    void UnitMpbar()
    {
        mpBar = Instantiate(prefMpBar, canvas.transform).GetComponent<RectTransform>();
        currentMPbar = mpBar.transform.GetChild(0).GetComponent<Image>();
    }

    void UnitMpbarUpdate()
    {
        Vector3 mpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + mpBar_h, 0));
        mpBar.position = mpBarPos;
        currentMPbar.fillAmount = currentMP / unitMP;

        if (currentMP <= 0)
        {
            unitState = UnitState.dead;
        }
    }

    private void SetUnitStatus(string unitName, float unitHP, float unitMP, float unitMS, float unitAD, float unitAS, float unitDis, string unitType)
    {
        unitName = this.unitName;
        unitHP = this.unitHP;
        currentHP = this.unitHP;
        unitMP = this.unitMP;
        currentMP = this.unitMP;
        unitMS = this.unitMS;
        unitAD = this.unitAD;
        unitAS = this.unitAS;
        unitDis = this.unitDis;
        unitType = this.unitType;
    }

    void SetAS(float AS)
    {
        animator.SetFloat("AttackSpeed", AS);
        unitAS = AS;
    }

    void Start()
    {
        UnitHpbar();
        UnitMpbar();
        SetAS(unitAS);
        FindTarget();
    }

    void Update()
    {
        UnitHpbarUpdate();
        UnitMpbarUpdate();
        UnitAI();
        CheckState();
    }

    public UnitState unitState = UnitState.idle;

    void SetState(UnitState state)
    {
        unitState = state;
        switch (unitState)
        {
            case UnitState.idle:
                #region Anim
                animator.SetFloat("Runstate", 0f);
                #endregion
                break;

            case UnitState.run:
                #region Anim
                animator.SetFloat("Runstate", 0.5f);
                #endregion
                break;

            case UnitState.attack:
                #region Anim
                animator.SetTrigger("Attack");
                animator.SetFloat("AttackState", 0.0f);
                animator.SetFloat("NormalState", 0.0f); // 0.0: Sword // 0.5: Bow // 1.0: Magic
                #endregion
                break;

            case UnitState.skill:
                #region Anim
                animator.SetTrigger("Attack");
                animator.SetFloat("AttackState", 1.0f);
                animator.SetFloat("NormalState", 0.0f);
                #endregion
                break;

            case UnitState.stun:
                #region Anim
                animator.SetFloat("Runstate", 1.0f);
                #endregion
                break;

            case UnitState.dead:
                #region Anim
                animator.SetTrigger("Die");
                #endregion
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

    void UnitAI()
    {
        atkDelay += Time.deltaTime;
        if (atkDelay >= unitAS) atkDelay = 0;
        
        float dis = Vector3.Distance(transform.position, target.transform.position);


        if (CheckTarget())      // 타겟이 있으면
        {
            FaceTarget();
            if (dis <= unitDis && atkDelay == 0)
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

        SetState(unitState);
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
    }

    void MoveToTarget() //타겟으로 이동
    {
        Vector2 dir = (Vector2)(target.transform.localPosition - transform.position);
        dirVec = dir.normalized;
        transform.position += (Vector3)dirVec * unitMS * Time.deltaTime;
    }

    void onDie() // 유닛 사망 후 실행
    {
        Destroy(gameObject, 1);
        Destroy(hpBar.gameObject, 1);
        Destroy(mpBar.gameObject, 1);
        unitState = UnitState.dead;
    }

    bool CheckTarget()
    {
        if (target == null)
            return false;
        if (target.GetComponent<UnitBase>().unitState == UnitState.dead)
            return false;
        if (!target.activeInHierarchy) 
            return false;
        
        return true;
    }
}
