using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadBar : MonoBehaviour {

    [SerializeField] private Image fillBar;
    [SerializeField] private Image backgroudBar;

    public int team;

    void Start()
    {
       if(team == 0)
        {
            //team blue
            //backgroudBar.color = "0e3568";
            //  fillBar.color = "689add";
        }
    }

    private void Update() {
        gameObject.transform.up = Vector3.up;
    }

    public void SetFillBar(float value) {
        fillBar.fillAmount = value;
    }

}
