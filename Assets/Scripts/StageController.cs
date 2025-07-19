using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class StageController : MonoBehaviour
{
    [SerializeField]
    private GameObject GoalCanvasObject;

    [SerializeField]
    private TextMeshProUGUI goalHaveCoinTextUI;
    [SerializeField]
    private TextMeshProUGUI StageCountTextUI;
    [SerializeField]
    private Button NextSceneButtonUI;
    [SerializeField]
    private TextMeshProUGUI KillEnemiesCountTextUI;
    private GameControllerScript gameControllerScript;
    private PlayerStatus playerStatus;
    private PlayerMoveScript playerMoveScript;
    private SceneMoveScript sceneMoveScript;
    [SerializeField]
    private AudioSource GoalAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneMoveScript = this.gameObject.GetComponent<SceneMoveScript>();
        playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
        playerMoveScript = playerStatus.gameObject.GetComponent<PlayerMoveScript>();
        gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameControllerScript>();
        NextSceneButtonUI.onClick.AddListener(() =>
        {
            //ゴールした後次のステージに進むボタンをクリックしたら　次のステージに進む
            DOVirtual.DelayedCall(1, () =>
            {
                Time.timeScale = 1;
                sceneMoveScript.SceneMove();
            });
        });
        GameObject Player = GameObject.FindWithTag("Player");
        if (gameControllerScript.BeforeSceneName.Contains("Stage") && gameControllerScript.NowSceneName.Contains("Stage"))
        {
            Debug.Log("Contains----------------");
            if (Player != null)
            {
                //HPとかCoinとかロード
                Debug.Log("HPとかCoinとかロード");
                // PlayerStatus player_status = Player.GetComponent<PlayerStatus>();
                // player_status.HP = gameControllerScript.PlayerHP;
                // player_status.MaxHP = gameControllerScript.PlayerMaxHP;
            }    
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Goal()
    {
        GoalCanvasObject.gameObject.SetActive(true);
        goalHaveCoinTextUI.SetText($"{playerStatus.HaveCoins}");
        StageCountTextUI.SetText($"{gameControllerScript.StageCountHistory[0]}");
        playerMoveScript.PermitMove = false;
        GoalAudioSource.Play();
        KillEnemiesCountTextUI.SetText(playerStatus.KillEnemyCount.ToString());
        Time.timeScale = 0;
    }
}
