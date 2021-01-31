using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public GameObject Fruit;

    public int placed = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            GameObject fruit = Instantiate(Fruit, new Vector3(0,0,0), Quaternion.Euler(-90, 0, 0));
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public bool enoughPlaced()
    {
        return placed >= 500;
    }
}
