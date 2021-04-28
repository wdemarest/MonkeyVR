using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    public Transform Head;
    public bool followY;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Head.position.x, (followY ? Head.position.y : transform.position.y), Head.position.z);
    }
}
