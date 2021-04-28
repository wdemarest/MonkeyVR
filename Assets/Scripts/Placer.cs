using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public GameObject Fruit;
    public GameObject Bubble;
    public GameObject FogCloud;
    public GameObject Chaser;
    public GameObject Turret;
    public GameObject Mine;

    public GameObject Rock;
    public GameObject WebStrand;

    public int placed = 0;

    float gasCloudDelay = 10f;
    float gasCloudDelayRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSimulation = false;
        for (int i = 0; i < 1000; i++)
        {
            Instantiate(Fruit, placePos(150, 175, 50), Quaternion.Euler(-90, 0, 0));
        }

        for (int i = 0; i < 100; i++)
        {
            Instantiate(Chaser, placePos(150, 175, 50), Quaternion.Euler(90, 0, 0));
        }

        for (int i = 0; i < 100; i++)
        {
            Instantiate(Mine, placePos(75, 350, 250), Quaternion.Euler(0, 0, 0));
        }



        /*for (int i = 0; i < 500; i++)
        {
            GameObject rock = Instantiate(Rock, placePos(75, 250, 200), Random.rotation);
            float size = 0.5f + Random.value * 2;
            rock.transform.localScale = new Vector3(size, size, size);
        }

        for (int i = 0; i < 100; i++)
        {
            Instantiate(WebStrand, placePos(50, 400, 325), Random.rotation);
        }*/
    }


    // Update is called once per frame
    void Update()
    {
        Physics.autoSimulation = true;

        gasCloudDelayRemaining -= Time.deltaTime;

        if (gasCloudDelayRemaining <= 0)
        {
            gasCloudDelayRemaining = gasCloudDelay;

            Instantiate(FogCloud, new Vector3(Random.Range(-100, 100), Random.Range(75, 175), -200), Quaternion.Euler(0, 0, 0));
        }
    }

    Vector3 placePos(float placeWidth, float placeTop, float placeBottom)
    {
        return new Vector3(Random.Range(-1 * placeWidth, placeWidth), Random.Range(placeTop, placeBottom), Random.Range(-1 * placeWidth, placeWidth));
    }

    public bool enoughPlaced()
    {
        return placed >= 500;
    }
}
