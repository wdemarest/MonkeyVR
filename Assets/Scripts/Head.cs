using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public Rigidbody PlayerRB;
    public bool gravGrace = false;
    public bool levitation = false;
    public Vector3 lastStablePos;
    SphereCollider SC;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
        SC = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerRB.useGravity = !(gravGrace || levitation);

        if(transform.position.y < 40)
        {
            JumpSave();
        }
    }

    public void Reset()
    {
        lastStablePos = new Vector3(0, 125, -3);
        JumpSave();
    }

    public void JumpSave()
    {
        PlayerRB.velocity = new Vector3(0, 0, 0);
        PlayerRB.position = lastStablePos;
        gravGrace = true;
    }
}
