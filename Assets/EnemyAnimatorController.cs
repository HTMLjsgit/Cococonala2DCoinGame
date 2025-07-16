using UnityEngine;
using DG.Tweening;
public class EnemyAnimatorController : MonoBehaviour
{
    private EnemyStatus enemyStatus;
    private EnemyMove enemyMove;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        enemyStatus = this.gameObject.GetComponent<EnemyStatus>();
        enemyMove = this.gameObject.GetComponent<EnemyMove>();
        enemyStatus.AttackedEvent.AddListener(() =>
        {
            enemyMove.PermitMove = false;
            anim.SetTrigger("Hurt");
            DOVirtual.DelayedCall(0.3f, () =>
            {
                //攻撃を受けたらしばらくの間動けないようにする
                enemyMove.PermitMove = true;
            });
        });
        enemyStatus.DeathEvent.AddListener(() =>
        {
            anim.SetBool("Death", true);
        });

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Move", enemyMove.Move);
    }
}
