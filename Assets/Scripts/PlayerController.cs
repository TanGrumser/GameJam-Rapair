using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private const float SPEED = 0.1f;

    private const string PARTICLES_PATH = "Prefabs/Particle System - Coins";

    private Rigidbody2D rb;
    private SpriteRenderer m_SpriteRenderer;

    public GameObject machineP2;

    private bool gamePadOn = false;

    public int team;

    public Vector3 direction;

    public Transform leftItemHolder;
    public Transform rightItemHolder;
    private GameObject drink;
    private GameObject belt;
    private GameObject Television;
    private bool onTV = false;
    private bool removeKey = false;
    private AudioSource audio;

    private bool carryBelt = false;
    private bool carryDrink = false;

    [SerializeField] private GameObject fillBar;



    private string[] usableGadgets = new string[] { "Banana", "Drink", "Television", "Belt" };
    private int GadgetIterator = 0;

    private int credit;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        //Fetch the SpriteRenderer from the GameObject
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(RemoveTelevision());
        carryBelt = false;
        carryDrink = false;
        audio = GetComponent<AudioSource>();
    }

    void Update() {
        if (team == 0) {
            if (gamePadOn) {
                HandleInput("joystick 2 button 5", "joystick 2 button 4", "Horizontal2", "Vertical2", "joystick 2 button 0", "removeC2", 0);
            } else {
                HandleInput("e", "q", "Horizontalwasd", "Verticalwasd", "space", "removeR", 0);
            }
        } else {
            if (gamePadOn) {
                HandleInput("joystick 1 button 5", "joystick 1 button 4", "Horizontal1", "Vertical1", "joystick 1 button 0", "removeC1", 1);
            } else {
                HandleInput("k", "i", "Horizontal", "Vertical", "right ctrl", "removeP", 1);
            }
        }
        transform.up = direction;
    }

    private void FixedUpdate() {
    }


    private void HandleInput(string iteratorDown, string iteratorUp, string horizontal, string vertical, string fire, string remove, int pl) {
        Vector3 movementInput = new Vector3(Input.GetAxis(horizontal), Input.GetAxis(vertical), 0f);
        Vector3 delta = Mathf.Clamp(movementInput.magnitude, 0f, 1f) * movementInput * SPEED;
        rb.MovePosition(transform.position + delta);
        if (Input.GetKeyDown(fire)) {
            HandleFire(GadgetIterator);
        }
        if (Input.GetButton(remove)) {
            removeKey = true;
        } else {
            removeKey = false;
        }

        if (Input.GetButtonDown(remove)) {
            StartCoroutine(RemoveTelevision());
        }



        HandleIteratorDown(iteratorDown, pl);
        HandleIteratorUp(iteratorUp, pl);

        if (delta.magnitude > float.Epsilon) {
            direction = delta;
        }
    }

    private void HandleIteratorDown(string key, int pl) {
        if (Input.GetKeyDown(key)) {
            if (GadgetIterator == 3) {
                GadgetIterator = 0;
            } else {
                GadgetIterator++;
            }
            GameManager.instance.uiManager.ChangeIterator(pl, GadgetIterator);
        }
    }
    private void HandleIteratorUp(string key, int pl) {
        if (Input.GetKeyDown(key)) {
            if (GadgetIterator == 0) {
                GadgetIterator = 3;
            } else {
                GadgetIterator--;
            }
            GameManager.instance.uiManager.ChangeIterator(pl, GadgetIterator);
        }
    }

    private void HandleFire(int interator) {
        int price = GetPriceOfGadget(interator);

        if (price <= credit) {
            if (interator == 0 || interator == 2) {
                Instantiate(Resources.Load<GameObject>("Prefabs/" + usableGadgets[interator]), transform.position, transform.rotation);
                credit -= price;
            } else {
                if (interator == 1 && !carryDrink) {
                    HoldDrink();
                    credit -= price;
                } else if (interator == 3 && !carryBelt) {
                    carryBelt = true;
                    HoldBelt();
                    credit -= price;
                }
            }
            GameManager.instance.uiManager.UpdateCoinTexts(team, credit);
        }
    }

    private void HoldBelt() {
        belt = Instantiate(Resources.Load<GameObject>("Prefabs/" + usableGadgets[3]), rightItemHolder.position, Quaternion.identity);
        belt.transform.parent = transform;
        carryBelt = true;
    }

    private void HoldDrink() {
        drink = Instantiate(Resources.Load<GameObject>("Prefabs/" + usableGadgets[1]), leftItemHolder.position, Quaternion.identity);
        drink.transform.parent = transform;
        carryDrink = true;
    }

    private int GetPriceOfGadget(int gadget) {
        if (gadget == 0) {
            return 25;
        } else if (gadget == 1) {
            return 30;
        } else if (gadget == 2) {
            return 75;
        } else if (gadget == 3) {
            return 85;
        }
        return 10000;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Coin") {
            credit += (int)Random.Range(5, 10);
            //Debug.Log("your credit is: " + credit + " §");
            GameManager.instance.uiManager.UpdateCoinTexts(team, credit);
            Destroy(col.gameObject);
            GameObject particleSystem = Instantiate(Resources.Load<GameObject>(PARTICLES_PATH), col.gameObject.transform.position, Quaternion.identity);
            Destroy(particleSystem, .5f);
            audio.Play();
        }

        if (col.gameObject.tag == "Worker") {
            if (carryDrink) {
                carryDrink = false;
                Destroy(drink);
                col.GetComponent<WorkerController>().ActivateDrink();
            }

            if (carryBelt) {
                carryBelt = false;
                Destroy(belt);
                col.GetComponent<WorkerController>().ActivateBelt();
            }
        }
        if (col.gameObject.tag == "TVrm") {
            Television = col.gameObject.transform.parent.gameObject;
            onTV = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Worker") {
            OnTriggerEnter2D(collision);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "TVrm") {
            onTV = false;
            Television = null;
        }
    }

    private IEnumerator RemoveTelevision() {
        float startDuration = GameManager.instance.gameSettings.removalTimeTV;
        float duration = startDuration;

        LoadBar bar = null;

        if (onTV && removeKey) {
            fillBar.SetActive(true);
            bar = gameObject.GetComponentInChildren<LoadBar>(true);
        }

        while (duration >= 0f && onTV && removeKey) {
            bar.SetFillBar((startDuration - duration) / startDuration);

            duration -= Time.fixedDeltaTime;

            Debug.Log("value of on TV: " + onTV + "  value of removekey: " + removeKey);
            yield return new WaitForFixedUpdate();

        }

        if (onTV && removeKey) {

            Destroy(Television);
        }

        fillBar.SetActive(false);
    }
}
