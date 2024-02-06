using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Machine : MonoBehaviour {

    public GameObject[] slots;

    [SerializeField] private Image fillBar;

    private float totalProgress = 0f;
    private float progressForWorker;

    public Vector3 GetSlotPosition(int index) {
        return slots[index].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void workDone(int team) {
        totalProgress = totalProgress + progressForWorker;
        fillBar.fillAmount = totalProgress;

        if (team == 0) {
            GameManager.instance.uiManager.UpdateFillBlue(totalProgress);
        } else {
            GameManager.instance.uiManager.UpdateFillRed(totalProgress);
        }

        if(totalProgress >= 1) {
            GameManager.instance.HandleWin(team);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        progressForWorker = GameManager.instance.gameSettings.workprogressForWorker;
    }

    public void ToggleParticles(int index, bool state) {
        slots[index].GetComponentInChildren<ParticleSystem>(true).gameObject.SetActive(state);
    }

}
