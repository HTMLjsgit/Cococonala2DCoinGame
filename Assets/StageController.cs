using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class StageController : MonoBehaviour
{
    [SerializeField]
    private GameObject GoalCanvasObject;

    [SerializeField]
    private TextMeshProUGUI goalHaveCoinTextUI;
    [SerializeField]
    private TextMeshProUGUI StageCountTextUI;
    private GameControllerScript gameControllerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Goal()
    {
        goalHaveCoinTextUI.SetText("");
        StageCountTextUI.SetText("");
    }
}
