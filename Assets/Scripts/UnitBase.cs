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

    public float atkDelay;  // ���� ������

    public string unitName; // ���� �̸�
    public float unitHP;    // ü��
    public float currentHP; // ���� ü��
    public float unitMP;    // ����
    public float currentMP; // ���� ����
    public float unitMS;    // �̵� �ӵ�
    public float unitAD;    // ���ݷ�
    public float unitAS;    // ���� �ӵ�
    public float unitDis;   // ���� ����
    public string unitType;  // ���� Ÿ��

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
    }           // ������

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

    public UnitState unitState = UnitState.idle;

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
                animator.SetTrigger("Run");
                animator.SetFloat("Runstate", 0f);
                #endregion
                break;

            case UnitState.run:
                #region Anim
                animator.SetTrigger("Run");
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
                animator.SetTrigger("Run");
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

    void FaceTarget()   //Ÿ�� �ٶ󺸱�
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

    void CheckAttack() //Ÿ�� ����
    {
        atkDelay += Time.deltaTime;
        if(atkDelay > unitAS)
        {
            DoAttack();
            atkDelay = 0;
        }
    }

    void DoAttack()
    {
        animator.SetTrigger("Attack");
        animator.SetFloat("AttackState", 0.0f);
        animator.SetFloat("NormalState", 0.0f); // 0.0: Sword // 0.5: Bow // 1.0: Magic
    }

    void DoMove() //Ÿ������ �̵�
    {
        if (CheckTarget()) return;

        Vector2 tVec = (Vector2)(target.transform.localPosition - transform.position);

        float tDis = tVec.sqrMagnitude;

        if(tDis <= unitDis * unitDis)
        {
            SetState(UnitState.attack);
            return;
        }

        dirVec = tVec.normalized;
        transform.position += (Vector3)dirVec * unitMS * Time.deltaTime;
    }

    void OnDie() // ���� ��� �� ����
    {
        Destroy(gameObject, 3);
        Destroy(hpBar.gameObject, 3);
        Destroy(mpBar.gameObject, 3);
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
