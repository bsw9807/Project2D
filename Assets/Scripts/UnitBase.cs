using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UnitBase : MonoBehaviour
{
    public List<GameObject> FoundObjects;
    public GameObject target;
    public float tDis;
    public Vector2 dirVec;
    public Vector2 tempDis;

    public Animator animator;
    public GameObject prefHpBar;
    public GameObject prefMpBar;
    public GameObject prefDmgText;
    public GameObject canvas;
    public float hpBar_h;
    public float mpBar_h;
    public float dmgText_h;
    RectTransform hpBar;
    RectTransform mpBar;
    RectTransform dmgText;
    Image currentHPbar;
    Image currentMPbar;

    public UnitState unitState = UnitState.idle;

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
    public UnitType unitType;  // 유닛 타입

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
            OnDie();
        }
    }

    public void DamageText(float dmg)
    {
        dmgText = Instantiate(prefDmgText, canvas.transform).GetComponent<RectTransform>();
        Vector3 dmgTextPos = Camera.main.WorldToScreenPoint(new Vector3(target.transform.localPosition.x, target.transform.localPosition.y + dmgText_h, 0));
        dmgText.position = dmgTextPos;
        dmgText.GetComponent<DamageText>().damage = target.GetComponent<UnitBase>().unitAD;
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
        SetInitState();
        SetAS(unitAS);
    }

    void Update()
    {
        UnitHpbarUpdate();
        UnitMpbarUpdate();
        CheckState();
    }

    void SetInitState()
    {
        unitState = UnitState.idle;
    }

    void SetState(UnitState state)
    {
        unitState = state;
        switch (unitState)
        {
            case UnitState.idle:
                #region Anim
                animator.SetFloat("RunState", 0f);
                #endregion
                break;

            case UnitState.run:
                #region Anim
                animator.SetFloat("RunState", 0.5f);
                #endregion
                break;

            case UnitState.attack:
                #region Anim
                animator.SetFloat("RunState", 0f);
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
                animator.SetFloat("RunState", 1.0f);
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
                FindTarget();
                DoMove();
                break;

            case UnitState.attack:
                CheckAttack();
                break;

            case UnitState.skill:
                break;

            case UnitState.stun:
                break;

            case UnitState.dead:
                OnDie();
                break;
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

        if (target != null) SetState(UnitState.run);
        else SetState(UnitState.idle);
    }

    void FaceTarget()
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

    void CheckAttack()
    {
        if (!CheckTarget()) return;
        if (!CheckDist()) return;

        atkDelay += Time.deltaTime;
        if(atkDelay > unitAS)
        {
            DoAttack();
            atkDelay = 0;
        }
    }

    bool CheckDist()
    {
        tempDis = (Vector2)(target.transform.localPosition - transform.position);
        float tDis = tempDis.sqrMagnitude;
        if (tDis <= unitDis * unitDis)
        {
            SetState(UnitState.attack);
            return true;
        }
        else
        {
            if (!CheckTarget()) SetState(UnitState.idle);
            else SetState(UnitState.run);
            return false;
        }
    }

    void DoAttack()
    {
        animator.SetTrigger("Attack");
        animator.SetFloat("AttackState", 0.0f); // 0.0: Sword // 0.5: Bow // 1.0: Magic
        animator.SetFloat("NormalState", 0.0f); // 0.0: Sword // 0.5: Bow // 1.0: Magic
        target.GetComponent<UnitBase>().currentHP -= unitAD;
        DamageText(unitAD);
    }

    void DoMove() //타겟으로 이동
    {
        if (!CheckTarget()) return;
        CheckDist();
        dirVec = tempDis.normalized;
        FaceTarget();
        transform.position += (Vector3)dirVec * unitMS * Time.deltaTime;
    }

    void OnDie() // 유닛 사망 후 실행
    {
        SetState(UnitState.dead);
        Destroy(gameObject, 3);
        Destroy(hpBar.gameObject, 3);
        Destroy(mpBar.gameObject, 3);
    }

    bool CheckTarget()
    {
        bool value = true;
        if (target == null)
            return false;
        if (target.GetComponent<UnitBase>().unitState == UnitState.dead)
            return false;
        if (!target.activeInHierarchy) 
            return false;

        if (!value)
        {
            SetState(UnitState.idle);
        }
        
        return value;
    }
}
