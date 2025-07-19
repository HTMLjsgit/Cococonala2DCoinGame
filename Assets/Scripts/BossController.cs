using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float attackSpeed = 8f;
    public float circleRadius = 3f;

    [Header("体当たり設定")]
    public float attackDuration = 5f;  // 体当たり継続時間

    [Header("休憩設定")]
    public float restDuration = 3f;    // 休憩時間（到着後に滞在する時間）
    public bool RestNow;
    [Header("休憩位置設定")]
    public Transform restPosition;     // 休憩位置
    public Vector3 defaultRestPos = new Vector3(0, 5, 0); // デフォルト休憩位置

    private Rigidbody2D rigid;
    private GameObject player;
    private Vector3 defaultLocalScale;

    private enum BossState
    {
        Attacking,
        Resting
    }
    private BossState currentState;

    // 攻撃用タイマー
    private float attackTimer;
    // 円運動用角度
    private float circleAngle;

    // 休憩用
    private bool isMovingToRest;
    private bool arrivedAtRest;
    private float restTimer;           // 到着後の待機タイマー
    private float arriveThreshold = 0.1f;
    private EnemyController enemyController;
    [SerializeField] private GameObject Ase;
    [SerializeField] private List<AttackColliderScript> attackColliderScripts;
    private Animator animator;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        enemyController = this.gameObject.GetComponent<EnemyController>();
        player = GameObject.FindWithTag("Player");
        defaultLocalScale = transform.localScale;

        if (restPosition == null)
        {
            var go = new GameObject("BossRestPosition");
            go.transform.position = defaultRestPos;
            restPosition = go.transform;
        }

        // 初期状態を攻撃に
        currentState = BossState.Attacking;
        attackTimer = attackDuration;
    }

    void Update()
    {
        if (player == null) return;
        animator.SetBool("Move", true);
        switch (currentState)
        {
            case BossState.Attacking:
                HandleAttackState();
                enemyController.BulletShotMode = true;
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                    SwitchToRest();
                break;

            case BossState.Resting:
                HandleRestState();
                enemyController.BulletShotMode = false;
                break;
        }
    }

    void HandleAttackState()
    {
        Vector3 p = player.transform.position;

        // 円運動
        circleAngle += Time.deltaTime * attackSpeed;
        Vector3 circlePos = p + new Vector3(
            Mathf.Cos(circleAngle) * circleRadius,
            Mathf.Sin(circleAngle) * circleRadius,
            0f
        );

        float dist = Vector2.Distance(p, transform.position);
        Vector3 target = Vector3.Lerp(circlePos, p, 0.7f);
        Vector2 dir = (target - transform.position).normalized;

        if (dist >= 1f)
            rigid.linearVelocity = dir * attackSpeed;
        else
            rigid.linearVelocity = Vector2.zero;

        UpdateFacing(dir.x);
    }

    void HandleRestState()
    {
        Vector3 restPos = restPosition.position;
        // まだ到着していない → 移動
        if (!arrivedAtRest)
        {
            if (!isMovingToRest)
            {
                isMovingToRest = true;
                rigid.linearVelocity = Vector2.zero;
            }

            Vector2 dir = (restPos - transform.position).normalized;
            rigid.linearVelocity = dir * moveSpeed;
            UpdateFacing(dir.x);

            if (Vector2.Distance(transform.position, restPos) <= arriveThreshold)
            {
                // 到着
                arrivedAtRest = true;
                rigid.linearVelocity = Vector2.zero;
                restTimer = restDuration;
                Debug.Log("ボス: 休憩ポイント到着");
            }
        }
        else
        {
            // 到着後の待機
            restTimer -= Time.deltaTime;
            if (restTimer <= 0f)
            {
                SwitchToAttack();
            }
        }
    }

    void SwitchToRest()
    {
        currentState = BossState.Resting;
        isMovingToRest = false;
        arrivedAtRest = false;
        AttackEnable(false);
        Debug.Log("ボス: 休憩開始");
        Ase.gameObject.SetActive(true);
    }

    void SwitchToAttack()
    {
        currentState = BossState.Attacking;
        attackTimer = attackDuration;
        rigid.linearVelocity = Vector2.zero;
        AttackEnable(true);
        Ase.gameObject.SetActive(false);
        Debug.Log("ボス: 体当たり再開");
    }

    void UpdateFacing(float h)
    {
        if (h > 0.1f)
            transform.localScale = new Vector3(defaultLocalScale.x, defaultLocalScale.y, defaultLocalScale.z);
        else if (h < -0.1f)
            transform.localScale = new Vector3(-defaultLocalScale.x, defaultLocalScale.y, defaultLocalScale.z);
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.transform.position, circleRadius);
        }
        Vector3 rp = restPosition != null ? restPosition.position : defaultRestPos;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rp, 0.5f);
    }
    void AttackEnable(bool enabled)
    {
        enemyController.BulletShotMode = enabled;
        if (enabled)
        {
            foreach (AttackColliderScript attackColliderScript in attackColliderScripts)
            {
                attackColliderScript.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (AttackColliderScript attackColliderScript in attackColliderScripts)
            {
                attackColliderScript.gameObject.SetActive(false);
            }
        }
    }
}
