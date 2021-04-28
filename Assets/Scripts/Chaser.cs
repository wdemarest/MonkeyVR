using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    public GameObject Target;
    public GameObject Fins;
    public GameObject Explosion;
    public float minSpeed = 2.5f;
    public float maxSpeed = 3.5f;
    float speed;
    public float turnSpeed = 1;
    public int health = 10;
    public float activateRange = 50;
    float triggerDist = 5;
    bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.Find("Head");
        speed = minSpeed + (Random.value * (maxSpeed - minSpeed));
    }

    // Update is called once per frame
    void Update()
    {
        Fins.SetActive(activated);

        Vector3 deltaToTarget = Target.GetComponent<Transform>().position - transform.position;

        if (deltaToTarget.magnitude < activateRange && !activated)
        {
            activated = true;
            FindObjectOfType<AudioManager>().Play("ChaserActivate");
        }

        if (activated)
        {
            Vector3 targetDirection = Vector3.Normalize(deltaToTarget);
            float angle = Vector3.Angle(targetDirection, transform.forward);

            float turnAmount = turnSpeed * ((angle - 3) / 180) * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turnAmount, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            Fins.GetComponent<Transform>().Rotate(0.0f, 0.0f, -720.0f * Time.deltaTime, Space.Self);

            if (Vector3.Distance(Target.transform.position, transform.position) < triggerDist)
            {
                Target.GetComponent<Head>().takeDamage(5);
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage()
    {
        if (activated)
        {
            FindObjectOfType<AudioManager>().Play("Hit");
            health -= 1;

            if (health <= 0)
            {
                FindObjectOfType<AudioManager>().Play("ChaserDie");
                Instantiate(Explosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}