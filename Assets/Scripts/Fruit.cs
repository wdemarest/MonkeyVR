using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : Item
{
    public bool positioning = true;
    public GameObject CollectionParticles;

    float placeWidth = 150;
    float placeTop = 175;
    float placeBottom = 50;


    // Start is called before the first frame update
    void Start()
    {
        points = 5;
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

    public override void OnHandCollide()
    {
        Debug.Log("handcol");

        //handCollided.GetComponent<Hand>.;
        FindObjectOfType<AudioManager>().Play("FruitPickup");
        Instantiate(CollectionParticles, transform.position, Quaternion.Euler(0, 0, 0));
        Debug.Log("touch");
        Destroy(gameObject);
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
}
