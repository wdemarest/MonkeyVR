using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject FireBurst;
    public Vector3 target;

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
        RB.MovePosition(RB.position + (GetTargetAngle().normalized * Time.deltaTime * bulletSpeed));
    }

    Vector3 GetTargetAngle()
    {
        Vector3 targetAngle = target - GetComponent<Transform>().position;
        return targetAngle;
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(FireBurst, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
