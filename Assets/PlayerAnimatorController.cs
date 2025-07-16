using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private PlayerMoveScript playerMoveScript;
    [SerializeField]private GroundCheckScript groundCheckScript;
    private PlayerStatus playerStatus;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMoveScript = this.gameObject.GetComponent<PlayerMoveScript>();
        anim = this.gameObject.GetComponent<Animator>();
        playerStatus = this.gameObject.GetComponent<PlayerStatus>();
        playerStatus.AttackedEvent.AddListener(() =>
        {
            anim.SetTrigger("Hurt");
        });
        playerStatus.DeathEvent.AddListener(() =>
        {
            anim.SetBool("Death", true);
        });

    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(playerMoveScript.x) > 0f)
        {
            anim.SetBool("Move", true);
        }
        else
        {
            anim.SetBool("Move", false);
        }
        if (playerMoveScript.JumpKeyPush && playerMoveScript.y > 0)
        {
            anim.SetTrigger("Jump");
        }
        anim.SetBool("Grounded", groundCheckScript.IsGround);
    }
}
