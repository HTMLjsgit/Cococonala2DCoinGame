using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EnemyStatus : MonoBehaviour
{
    //ここは敵のステータスを管理する場所
    public float HP = 100; //HP
    public float AttackPower; //普通攻撃の攻撃力
    public float AttackPowerBullet; //弾の攻撃力

    public AttackColliderScript[] AttackColliders;
    [Header("イベント系")]
    public UnityEvent DeathEvent; 
    public UnityEvent AttackedEvent;
    public UnityEvent DeathAnimationEndEvent;
    public bool Death;
    Animator anim;
    bool death_once;
    // Start is called before the first frame update
    void Start()
    {
        foreach(AttackColliderScript attack_col_script in AttackColliders){
            //AttackColliderScriptのAttackPowerに自分のステータスの攻撃力を代入してあげる。
            attack_col_script.AttackPower = this.AttackPower;

        }
        anim = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //もしHPが0以下になれば死ぬ(破壊する)
        if(this.HP <= 0){
            Death = true;
            if(!death_once){
                DeathEvent.Invoke();
            }
            if(!death_once){
                death_once = true;
            }

        }
    }
    //ダメージを受ける関数
    public void Attacked(float _attcked){
        if(!Death){
            AttackedEvent.Invoke();
            this.HP -= _attcked;
        }

        
    }


    public void DeathAnimationEnd(){
        //Deathアニメーションが終了したら(AnimationからこのFunctionを指定しています)
        DeathAnimationEndEvent.Invoke();
    }
    
}
