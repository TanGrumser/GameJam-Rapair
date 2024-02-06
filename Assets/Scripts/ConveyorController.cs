using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{

    private List<GameObject> allUnassignedBuckets;
    private List<GameObject> allAssignedBuckets;

    public GameObject piece;
    public Transform leftStartPosition;
    public Transform rightStartPosition;


    // Start is called before the first frame update
    void Start()
    {

        allUnassignedBuckets = new List<GameObject>();
        allAssignedBuckets = new List<GameObject>();


        //goes throw all children
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {


            if (gameObject.transform.GetChild(i).tag == "BucketSlot")
            {
                allAssignedBuckets.Add(gameObject.transform.GetChild(i).gameObject);
            }

        }

        StartCoroutine(spawnPieces());
    }

    private IEnumerator spawnPieces()
    {
        while (true)
        {
            Instantiate(piece, (Vector3)leftStartPosition.position, Quaternion.identity);
            Instantiate(piece, (Vector3)rightStartPosition.position, Quaternion.identity);

            yield return new WaitForSeconds(5f);
        }
    }

    public bool IsSlotFree()
    {
        return allUnassignedBuckets.Count > 0;
    }

    public void AddBucketToUnassignedBuckets(GameObject bucket)
    {
        allUnassignedBuckets.Add(bucket);
    }

    public Vector2 GetFreeSlotPosition(WorkerController worker)
    {

        GameObject nearestBucket = null;
        float minDistance = 9999999999f;


        foreach (GameObject bucket in allUnassignedBuckets)
        {

            Vector2 distanceVector = bucket.transform.position - worker.transform.position;

            float distance = distanceVector.magnitude;

            if (minDistance > distance)
            {

                nearestBucket = bucket;
                minDistance = distance;
            }

        }
        allUnassignedBuckets.Remove(nearestBucket);
        return nearestBucket.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }



}
