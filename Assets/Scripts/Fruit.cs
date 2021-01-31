using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public bool positioning = true;

    float placeWidth = 150;
    float placeTop = 175;
    float placeBottom = 40;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("FruitInstantiated");
        GetComponent<LODGroup>().ForceLOD(3);
        transform.localScale = new Vector3(10, 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (positioning)
        {
            transform.position = placePos();
            if (GameObject.Find("Placer").GetComponent<Placer>().enoughPlaced())
            {
                Destroy(gameObject);
            }
        }
    }

    Vector3 placePos()
    {
        return new Vector3(Random.Range(-1 * placeWidth, placeWidth), Random.Range(placeTop, placeBottom), Random.Range(-1 * placeWidth, placeWidth));
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (positioning == true)
            {
                transform.localScale = new Vector3(3, 3, 3);
                GameObject.Find("Placer").GetComponent<Placer>().placed++;
                positioning = false;
                GetComponent<LODGroup>().ForceLOD(-1);
                Debug.Log("Landed");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player") && GameObject.Find("ScoreTracker").GetComponent<ScoreTracker>().score < 30) {
            GameObject.Find("ScoreTracker").GetComponent<ScoreTracker>().GetPoint();
            Destroy(gameObject);
        }
        
    }
}
