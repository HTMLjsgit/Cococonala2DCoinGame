using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyStatus enemy_status;
    public GameObject BulletPrefab;
    public Transform[] BulletPrefabCreatePosition;
    [HideInInspector]
    public bool BulletShot; //こいつをTrueにすれば、銃が撃たれる。

    GameObject Player;
    [HideInInspector]
    public float EnemyToPlayer;
    [HideInInspector]
    public int EnemyToPlayerDirection;
    int MyDirection; //自分が向いている方向
    Vector3 DefaultLocalScale;

    [Header("弾を撃つ敵かどうか")]
    public bool BulletShotMode; //こいつをTrueにすれば常に弾を撃つ。

    [Header("弾の設定(BulletShotModeがTrueのときのみ有効)")]
    public float NextBulletShotWaitTime; //つぎ弾が発射されるまで待つ時間
    public float BulletSpeed; //弾のスピード速度 
    public float BulletDistanceDestroy; //弾が飛んでから破壊される距離
    public bool BulletShotDirectionByMine; //自分の方向から弾を発射する(プレイヤーがどこにいようが発射する)
    public bool BulletShotDirectionForPlayer; //プレイヤーの方向に向かって弾を発射する
    public float BulletShotMineToPlayerDistance; //自分の座標とプレイヤーとの距離がどれほど近づいたら弾を発射するか

    public AudioSource BulletSound;

    //計測
    private float NextBulletShowWaitTimeNow = 0;
    private float EnemyToPlayerDistanceMeasure;

    private bool BulletShotGO;

    private bool once_shot_go;
    // Start is called before the first frame update
    void Start()
    {
        enemy_status = this.gameObject.GetComponent<EnemyStatus>();
        Player = GameObject.FindWithTag("Player");
        DefaultLocalScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //敵からPlayerまでの距離(左側にいるのか 右側にいるのか)
        EnemyToPlayer = Player.transform.position.x - this.transform.position.x;

        //自分が向いている方向を-1か1で取得
        MyDirection = (int)Mathf.Floor(Mathf.Clamp(this.transform.localScale.x, -1, 1));

        //自分からプレイヤーまでの距離
        EnemyToPlayerDistanceMeasure = Vector2.Distance(this.transform.position, Player.transform.position);
        
        if(EnemyToPlayer > 0){
            EnemyToPlayerDirection = 1;
        }else if(EnemyToPlayer < 0){
            EnemyToPlayerDirection = -1;
        }
        if(BulletShotMode && BulletShotGO){
            if(!enemy_status.Death){
                NextBulletShowWaitTimeNow += Time.deltaTime;
                if(NextBulletShowWaitTimeNow > NextBulletShotWaitTime){
                    if(BulletShotDirectionForPlayer){
                        if(MyDirection == EnemyToPlayerDirection){
                            //自分の方向とプレイヤーと自分との方向が一致したら
                            BulletShot = true;
                        }
                    }else{
                        BulletShot = true;
                    }
                    NextBulletShowWaitTimeNow = 0;
                }
            }
        }
        if(EnemyToPlayerDistanceMeasure <= BulletShotMineToPlayerDistance){
            // プレイヤーと一定の距離近づいたら
            BulletShotGO = true;
            once_shot_go = false;
        }else{
            if(!once_shot_go){
                //一回だけ実行
                BulletShotGO = false;
                once_shot_go = true;
            }
        }
        if(BulletShot){
            BulletSound.PlayOneShot(BulletSound.clip);
            foreach(Transform BulletPrefabCreate in BulletPrefabCreatePosition){
                GameObject BulletShotObject = Instantiate(BulletPrefab, BulletPrefabCreate.position, Quaternion.identity);
                BulletScript bullet_script = BulletShotObject.GetComponent<BulletScript>();
                bullet_script.direction = MyDirection;
                bullet_script.bullet_attack_col.AttackPower = enemy_status.AttackPowerBullet;
                bullet_script.speed = BulletSpeed;
                bullet_script.DistanceDestroy = BulletDistanceDestroy;
                BulletShot = false;
            }

        }
    }
}
