using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float direction; //進む方向
    public float speed; //スピード

    Rigidbody2D rigid;

    EnemyStatus enemy_status;
    public AttackColliderScript bullet_attack_col;

    public float DistanceDestroy; //破壊される距離
    float init_position_x;
    // Start is called before the first frame update
    void Start()
    {
        rigid = this.gameObject.GetComponent<Rigidbody2D>();
        init_position_x = this.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        rigid.velocity = new Vector2(direction * speed, 0);
        //発射位置から現在の位置を取得して、DistanceDestroyよりも超えていたらObjectを削除
        if(Mathf.Abs(this.transform.position.x - init_position_x) > DistanceDestroy){
            Destroy(this.gameObject);
        }
    }

}
