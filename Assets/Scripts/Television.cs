using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Television : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Invoke("Kill", GameManager.instance.gameSettings.durationTimeTV);
    }

    void Kill() {
        Destroy(gameObject);
    }
}
