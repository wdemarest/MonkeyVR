using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Vase : MonoBehaviour
{
    public TMP_Text Deposits;
    public Transform headT;

    int deposits = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Deposits.text = ""+deposits;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            deposits += GameObject.Find("ScoreTracker").GetComponent<ScoreTracker>().score;
            GameObject.Find("ScoreTracker").GetComponent<ScoreTracker>().score = 0;
        }
    }
}
