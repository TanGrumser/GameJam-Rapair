using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WorkerController : MonoBehaviour {

    public Transform target;

    public int teamIndex;
    public int index;

    private const string BANANA_PARTICLES_PATH = "Prefabs/Particle System - Bananas";
    private const string ENERGY_PARTICLES_PATH = "Prefabs/Particle System - Energy";

    private string[] TOOLS_PATHS = { "sprites/hammer", "sprites/pliers", "sprites/screwdriver" };


    public float speed = 200f;
    public float nextWaypointDistacnce = 3f;

    public Transform itemHolder;
    private GameObject item;

    public Transform waitingSpot;

    Path path;
    int currentWayponint = 0;
    bool reachedEndOfPath;
    private float turningTime = -1f;
    private float drinkTime = -1f;
    private int beltRepairs = 0;
    private bool speedDecreased = false;
    private Transform television;

    Seeker seeker;
    Rigidbody2D rb;
    private Machine machine;
    

    public GameObject ConveyorSlot;
    public GameObject ToolStation;

    [SerializeField] private GameObject fillBar;
    [SerializeField] private SpriteRenderer tool;


    private bool carryTool;
    private bool carryPiece;

    private void Awake() {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Machine machine) {
        this.machine = machine;

        //Goto(machine.GetSlotPosition(index));
        Goto(ToolStation);
        //Debug.Log("Worker: " + this + " Target " + (machine.GetSlotPosition(index)));
    }

    private void Update() {
        if (television) {
            transform.up = television.position - transform.position;
        }
    }

    public void Goto(Vector2 position)
    {
        seeker.StartPath(rb.position, position, OnPathComplete);
    }

    public void Goto(GameObject GoToObject)
    {
        Goto(GoToObject.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == ToolStation && carryTool == false) {
            carryTool = true;
            if (GameManager.instance.conveyor.IsSlotFree()) {
                Goto(GameManager.instance.conveyor.GetFreeSlotPosition(this));
            } else {
                StartCoroutine(WaitForFreeSlot());
            }
        }
    }

    private IEnumerator WaitForFreeSlot() {
        Goto(waitingSpot.position);

        do {
            yield return new WaitForEndOfFrame();
        }
        while (!GameManager.instance.conveyor.IsSlotFree());

        Goto(GameManager.instance.conveyor.GetFreeSlotPosition(this));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        string slotName = "Slot " + (teamIndex == 0 ? "Blue" : "Red") + " " + (index);
       // Debug.Log(collision.gameObject.tag + ", " + slotName);

        //Debug.Log(collision.gameObject.name + ", "  + slotName); 

        if (collision.gameObject.name == slotName) {
            StartCoroutine(RepairMachine());
            path = null;
        }

        if (collision.gameObject == ToolStation && carryTool == false)
        {
            carryTool = true;
            tool.gameObject.SetActive(true);
            tool.sprite = Resources.Load<Sprite>(TOOLS_PATHS[Random.Range(0, 3)]);
            if (GameManager.instance.conveyor.IsSlotFree())
            {
                Goto(GameManager.instance.conveyor.GetFreeSlotPosition(this));
            }
            else
            {
                StartCoroutine(WaitForFreeSlot());
            }
        }

        if (collision.gameObject.tag == "BucketSlot") {
           // Debug.Log("Geh zur Maschine");
            carryPiece = true;
            item = collision.GetComponent<BucketSlot>().currentItem;
            item.transform.parent = transform;
            item.transform.position = itemHolder.position;
            collision.GetComponent<BucketSlot>().currentItem = null;
            Goto(machine.GetSlotPosition(index));
        }


        if (collision.gameObject.tag == "Banana") {
            turningTime = GameManager.instance.gameSettings.durationTimeBanana;
            GameObject particleSystem = Instantiate(Resources.Load<GameObject>(BANANA_PARTICLES_PATH), collision.gameObject.transform.position, Quaternion.identity);
            Destroy(particleSystem, .5f);
            Destroy(collision.gameObject);
            Instantiate(Resources.Load("Prefabs/Sounds/Cartoon"));

        }

        if (collision.gameObject.tag == "Television") {
            speedDecreased = true;
            television = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Television") {
            speedDecreased = false;
            television = null;
        }
    }

    private void GoalReached() {
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWayponint = 0;
        }
    }

    public void ActivateDrink() {
        drinkTime = GameManager.instance.gameSettings.durationTimeEnergy;
        GameObject particleSystem = Instantiate(Resources.Load<GameObject>(ENERGY_PARTICLES_PATH), gameObject.transform.position, Quaternion.identity);
        Destroy(particleSystem, .5f);
    }

    public void ActivateBelt() {
        beltRepairs = GameManager.instance.gameSettings.runsToolBelt;

    }

    void FixedUpdate() {
        if (path == null) {
            return;
        }

        if (drinkTime != -1f) {
            drinkTime -= Time.fixedDeltaTime;

            if (drinkTime <= 0f) {
                drinkTime = -1f;
                transform.rotation = Quaternion.identity;
            }
        }

        if (turningTime != -1f) {
            transform.rotation *= Quaternion.Euler(Vector3.forward * 20f);
            turningTime -= Time.fixedDeltaTime;

            if (turningTime <= 0f) {
                turningTime = -1f;
                transform.rotation = Quaternion.identity;
            }

            return;
        }

        if (currentWayponint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            //GoalReached();
            return;
        } else {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayponint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        Vector3 fututePosition = path.vectorPath[(int)Mathf.Clamp(currentWayponint + 10, 0, path.vectorPath.Count - 1)];
        transform.up = fututePosition - transform.position;


        //rb.AddForce(force);
        rb.MovePosition(transform.position + (Vector3)direction * speed * Time.fixedDeltaTime * (speedDecreased ? .5f : 1f) * (drinkTime != -1f ? 3f: 1f));

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayponint]);

        if (distance < nextWaypointDistacnce) {
            currentWayponint++;
        }
    }

    private IEnumerator RepairMachine() {
        float startDuration = 1f / (drinkTime != -1f ? 3f : 1f);
        float duration = startDuration;

        fillBar.SetActive(true);
        machine.ToggleParticles(index, true);
        LoadBar bar = gameObject.GetComponentInChildren<LoadBar>(true);

        while (duration >= 0f) {
            bar.SetFillBar((startDuration - duration) / startDuration);
            duration -= Time.deltaTime;
            transform.up = machine.transform.position - transform.position;
            yield return new WaitForEndOfFrame();
        }

        fillBar.SetActive(false);
        machine.ToggleParticles(index, false);
        tool.gameObject.SetActive(false);


        if (useToolOrBelt() && carryPiece) {
            machine.workDone(teamIndex);
            carryPiece = false;
        }

        Destroy(item);

        if (beltRepairs > 0)
        {
            if (GameManager.instance.conveyor.IsSlotFree())
            {
                Goto(GameManager.instance.conveyor.GetFreeSlotPosition(this));
            }
            else
            {
                StartCoroutine(WaitForFreeSlot());
            }
        }
        else
        {
            Goto(ToolStation);
        }
    }

    private bool useToolOrBelt()
    {
        if(carryTool){
            carryTool = false;
            return true;
        } 
        else if (beltRepairs > 0)
        {
            beltRepairs = beltRepairs - 1;
            return true;
        }
        return false;
       
    }
}
