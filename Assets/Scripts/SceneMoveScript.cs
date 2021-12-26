using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneMoveScript : MonoBehaviour
{
    public enum SceneMoveMode{
        Collider,
        Event,
        Start
    }
    public string MoveSceneName;
    public SceneMoveMode scene_move_mode;
    public bool SceneMoveGetFromGameController = false;
    GameControllerScript game_controller_script;
    // Start is called before the first frame update
    void Start()
    {
        if(scene_move_mode == SceneMoveMode.Start){
            SceneMove();
        }
        game_controller_script = GameObject.FindWithTag("GameController").GetComponent<GameControllerScript>();
        if(SceneMoveGetFromGameController){
            MoveSceneName = game_controller_script.BeforeSceneName;
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SceneMove(){
        SceneManager.LoadScene(MoveSceneName);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"){
            SceneMove();
        }
    }
}
