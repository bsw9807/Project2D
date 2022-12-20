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

    public Transform targetTF;

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
        hpBar = Instantiate(prefHpBar, canvas.transform).GetComponent<RectTransform>();
        #region 01_Warrior_Status
        if (name.Equals("01_Warrior"))
        {
            SetUnitStatus("01_Warrior", 1000, 50, 100, 1, 10);
        }
        #endregion
        currentHPbar = hpBar.transform.GetChild(0).GetComponent<Image>();

        SetAS(unitAS);
        FindTarget();
    }

    void Update()
    {
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + hpBar_h, 0));
        hpBar.position = hpBarPos;
        currentHPbar.fillAmount = currentHP / unitHP;

        unitAS -= Time.deltaTime;
        float tDis = Vector3.Distance(transform.position, targetTF.position);

        FaceTarget();
        if(tDis <= unitDis)
        {
            AttackTarget();
        }
        else
        {
            MoveToTarget();
        }
    }

    public List<GameObject> FoundTargets;
    public GameObject target;
    public string TagName;
    public float shortDis;

    void FindTarget()
    {
        switch (gameObject.tag)
        {
            case "Friend": TagName = "Enemy"; break;
            case "Enemy": TagName = "Friend"; break;
        }
        FoundTargets = new List<GameObject>(GameObject.FindGameObjectsWithTag(TagName));
        shortDis = Vector3.Distance(gameObject.transform.position, FoundTargets[0].transform.position);
        target = FoundTargets[0];

        foreach(GameObject found in FoundTargets)
        {
            float Dis = Vector3.Distance(gameObject.transform.position, found.transform.position);
            
            if (Dis <= shortDis)
            {
                target = found;
                shortDis = Dis;
            }
        }
    }

    void FaceTarget()   //타겟 바라보기
    {

    }

    void AttackTarget() //타겟 공격
    {

    }

    void MoveToTarget() //타겟으로 이동
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            currentHP -= unitAD;
            if(currentHP <= 0)
            {
                onDie();
            }
        }
    }

    void onDie()
    {
        unitState = UnitState.dead;
        Destroy(gameObject, 1);
        Destroy(hpBar.gameObject, 1);
    }

    void SetAS(float AS)
    {
        animator.SetFloat("AttackSpeed", AS);
    }

    #region SetState
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
    #endregion
}
