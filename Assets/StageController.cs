using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    private GameControllerScript gameControllerScript;
    private PlayerStatus playerStatus;
    private SceneMoveScript sceneMoveScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneMoveScript = this.gameObject.GetComponent<SceneMoveScript>();
        playerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
        gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameControllerScript>();
        NextSceneButtonUI.onClick.AddListener(() =>
        {
            sceneMoveScript.SceneMove();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Goal()
    {
        goalHaveCoinTextUI.SetText($"{playerStatus.HaveCoins}");
        StageCountTextUI.SetText($"{gameControllerScript.StageCountHistory[0]}");
    }
}
