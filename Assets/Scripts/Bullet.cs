using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject FireBurst;
    public Vector3 targetAngle;
    Rigidbody RB;
    float bulletSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RB.MovePosition(RB.position + (targetAngle * Time.deltaTime * bulletSpeed));
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(FireBurst, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
