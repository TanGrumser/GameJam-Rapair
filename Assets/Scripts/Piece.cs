using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    private const int CONVEYOR_LAYER = 10;
    public float speed = .5f;
    private bool hasSlot = false;

    private string[] PATHS = {"glass", "metal", "wood" };
    
    void Start() {
        ChangeSprite();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!hasSlot && collision.gameObject.layer == CONVEYOR_LAYER) {
            transform.position += collision.transform.right * -1f * Time.fixedDeltaTime * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
       // Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.tag == "BucketSlot" && !hasSlot) {


            BucketSlot slot = collision.gameObject.GetComponent<BucketSlot>();

            if (slot.currentItem == null) {
                hasSlot = true;
                slot.currentItem = this.gameObject;
                transform.position = collision.transform.position;
                GameManager.instance.conveyor.AddBucketToUnassignedBuckets(slot.gameObject);
            }
        }

        else if (collision.gameObject.tag == "Destroyer") {
            Destroy(this.gameObject);
        }
    }

    private void ChangeSprite() {
        int index = Random.RandomRange(0, 3);
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("mats/" + PATHS[index]);
    }
}
