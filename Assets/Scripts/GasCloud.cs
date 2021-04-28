using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : MonoBehaviour
{
    float speed = 5;
    float width = 50;
    public GameObject Head;

    float damageTimer = 0;
    float damageInterval = 1;

    // Start is called before the first frame update
    void Start()
    {
        Head = GameObject.Find("Head");
        GetComponent<Transform>().localScale = new Vector3(width, width, width);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        if (transform.position.z > 150)
        {
            Destroy(gameObject);
        }

        bool touchingPlayer = Vector3.Distance(Head.GetComponent<Transform>().position, GetComponent<Transform>().position) < width/2;
        if (touchingPlayer)
        {
            if (damageTimer >= damageInterval)
            {
                Head.GetComponent<Head>().takeDamage(1);
                damageTimer -= damageInterval;
            }
            damageTimer += Time.deltaTime;
        }
        else
        {
            damageTimer = 0;
        }
    }
}
