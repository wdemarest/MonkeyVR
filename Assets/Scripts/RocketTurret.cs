using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTurret : MonoBehaviour
{
    GameObject Head;
    public GameObject Rocket;
    public GameObject FireBurst;
    Vector3 playerPos;
    Vector3 playerPosAngle;
    Vector3? AimTarget;
    float fireDelay = 3f;
    public float fireDelayRemaining = 0;
    float range = 25f;
    public bool active = true;
    bool LockedOn;

    public AudioSource awaken;
    public AudioSource lockOn;
    public AudioSource fire;

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

        playerPos = Head.GetComponent<Transform>().position;
        playerPosAngle = TargetAngle(playerPos);

        bool playerInSight = !Physics.Raycast(transform.position, playerPosAngle, playerPosAngle.magnitude, LayerMask.GetMask("GrabbableTerrain"));

        if (playerPosAngle.magnitude < range)
        {
            if (!active)
            {
                Debug.Log(active);
                awaken.Play();
                active = true;
            }
        }
        else
        {
            active = false;
        }


        if (playerPosAngle.magnitude < range && playerInSight)
        {
            LockedOn = fireDelayRemaining <= 1;
            if (!LockedOn)
            {
                AimTarget = playerPos;
            }

            fireDelayRemaining -= Time.deltaTime;
            GetComponent<Transform>().LookAt(new Vector3(AimTarget.Value.x, AimTarget.Value.y, AimTarget.Value.z));

            if(fireDelayRemaining <= 1 && !LockedOn)
            {
                lockOn.Play();
                LockedOn = true;
            }

            if (fireDelayRemaining <= 0)
            {
                GameObject rocket = Instantiate(Rocket, GetComponent<Transform>().position, Quaternion.Euler(0, 0, 0));
                fire.Play();

                rocket.GetComponent<Rocket>().target = (Vector3)AimTarget;

                fireDelayRemaining = fireDelay;
                AimTarget = null;
            }
        }
        else
        {
            fireDelayRemaining = fireDelay;
            AimTarget = null;
        }
    }

    Vector3 TargetAngle(Vector3 targetPos)
    {
        Vector3 myPos = GetComponent<Transform>().position;
        
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