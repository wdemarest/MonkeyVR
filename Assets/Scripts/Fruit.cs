using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : Item
{
    public GameObject CollectionParticles;

    public AudioSource collected;

    // Start is called before the first frame update
    void Start()
    {
        points = 1;
        //Debug.Log("FruitInstantiated");
        GetComponent<LODGroup>().ForceLOD(3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnHandCollide()
    {

        //handCollided.GetComponent<Hand>.;
        collected.Play();
        Instantiate(CollectionParticles, transform.position, Quaternion.Euler(0, 0, 0));
        //Debug.Log("touch");
        Destroy(gameObject);
    }

    /*void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (positioning == true)
            {
                transform.localScale = new Vector3(3, 3, 3);
                positioning = false;
                GetComponent<LODGroup>().ForceLOD(-1);
                //Debug.Log("Landed");
            }
        }
    }*/
}
