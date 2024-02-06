using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public WorkerController[] workers1;
    public WorkerController[] workers2;
    public GameObject redMachine;
    public GameObject blueMachine;
    public GameObject canvas;
    public ConveyorController conveyor;

    private string PATH = "Prefabs/GameOverScreen";

    public UIManager uiManager;
    private bool gameFinished = false;

    public ConfigMaster gameSettings;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        
        //DontDestroyOnLoad(gameObject);

        //load default settings
        gameSettings = new ConfigMaster();
        uiManager.SetInventoryCosts();
        uiManager.UpdateScore();
    }

    public void HandleWin(int team){
        if (gameFinished) {
            return;
        }
        gameFinished = true;
        if(team == 0){

            // team blue wins!
            TotalGameScore.scoreBlue = TotalGameScore.scoreBlue + 1;
            GameObject screen = Instantiate(Resources.Load<GameObject>(PATH + 1.ToString()), canvas.transform);
            
        } else {

            // team red wins!
            TotalGameScore.scoreRed = TotalGameScore.scoreRed + 1;
            GameObject screen = Instantiate(Resources.Load<GameObject>(PATH + 0.ToString()), canvas.transform);

      
        }
    }

    private void Start() {
        InitGame();
    }

    private void InitGame() {
        foreach(WorkerController worker in workers1) {
            worker.Initialize(blueMachine.GetComponent<Machine>());
        }

        foreach (WorkerController worker in workers2) {
            worker.Initialize(redMachine.GetComponent<Machine>());
        }
        StartCoroutine(SpawnCoins());



    }

    private IEnumerator SpawnCoins(){
        float duration;
        float xPos;
        float yPos;
        while(true){
            duration = Random.Range(gameSettings.coinSpawnRateMin, gameSettings.coinSpawnRateMax);
            yield return new WaitForSeconds(duration);
            xPos = Random.Range(-7.3f, 7.3f);
            yPos = Random.Range(-1.5f, 3.0f);
            if(!(yPos < 1.5 && (xPos < 2 && xPos > -2))){
                Instantiate(Resources.Load<GameObject>("Prefabs/Coin"), new Vector3(xPos, yPos, 0), Quaternion.identity);
            } else {
                Instantiate(Resources.Load<GameObject>("Prefabs/Coin"), new Vector3(xPos, Random.Range(1.6f, 3.6f), 0), Quaternion.identity);
            }
        }
    }

}
