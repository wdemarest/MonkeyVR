using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float vibDur = 0.5f;
    public int points = 0;
    
    public virtual void OnHandCollide()
    {
        Debug.Log("itemHandCol");
    }
}
