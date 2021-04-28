using UnityEngine;

public class PistolBulletBody : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        GetComponent<Transform>().parent.GetComponent<PistolBullet>().CollisionDetected(collision);
    }
}