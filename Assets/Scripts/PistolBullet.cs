using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    public GameObject Target;
    public GameObject Explosion;
    public float speed = 50f;
    public float turnSpeed = 5f;
    public float hitRange = 1;
    public float ageMax = 3;
    float age = 0;

    public AudioSource land;
    
    // Update is called once per frame
    void Update()
    {
        Target = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Chaser"));

        age += Time.deltaTime;
        if (age > ageMax)
        {
            Destroy(gameObject);
        }

        Vector3 deltaToTarget = Target.GetComponent<Transform>().position - transform.position;

        if (deltaToTarget.magnitude < hitRange) {
            Debug.Log("hit");

            Target.GetComponent<Chaser>().TakeDamage();

            explode();
        }

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    GameObject GetClosestEnemy(GameObject[] enemies)
    {
        GameObject gMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject g in enemies)
        {
            Transform t = g.GetComponent<Transform>();
            
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                gMin = g;
                minDist = dist;
            }
        }
        return gMin;
    }

    void explode()
    {
        land.Play();
        Instantiate(Explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void CollisionDetected(Collision collision)
    {
        if (collision.gameObject.tag == "Chaser") {
            Target.GetComponent<Chaser>().TakeDamage();
        }

        explode();
    }
}
