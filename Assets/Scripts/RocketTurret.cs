using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTurret : MonoBehaviour
{
    GameObject Head;
    public GameObject FireBurst;
    Vector3 target;
    float fireDelay = 3f;
    float fireDelayRemaining = 0;
    float range = 25f;

    float placeWidth = 150;
    float placeTop = 175;
    float placeBottom = 50;

    // Start is called before the first frame update
    void Start()
    {
        Head = GameObject.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetAngle = TargetAngle();
        target = Head.GetComponent<Transform>().position;

        bool playerInSight = !Physics.Raycast(transform.position, targetAngle, targetAngle.magnitude, LayerMask.GetMask("GrabbableTerrain"));

        if (targetAngle.magnitude < range && playerInSight)
        {


            fireDelayRemaining -= Time.deltaTime;
            if (fireDelayRemaining <= fireDelay - 0.1f)
            {
            }

            if (fireDelayRemaining <= 0)
            {
                Head.GetComponent<Head>().takeDamage(5);

                fireDelayRemaining = fireDelay;
            }
        }
        else
        {
            fireDelayRemaining = fireDelay;
        }
    }

    Vector3 TargetAngle()
    {
        Vector3 myPos = GetComponent<Transform>().position;
        Vector3 targetPos = target;
        return targetPos - myPos;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameObject explosion = Instantiate(FireBurst, transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(3, 3, 3);

            Destroy(gameObject);
        }
    }
}
