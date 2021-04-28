using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    GameObject player;
    public GameObject Body;
    public float timer = 5;
    public GameObject Explosion;
    bool active = false;
    float triggerDist = 5;
    float damageDist = 25;
    float damage = 5;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        if(!active || Vector3.Distance(player.transform.position, transform.position) < triggerDist)
        {
            active = true;
            FindObjectOfType<AudioManager>().Play("MineBeeping");
            Body.SetActive(true);
        }

        if (active)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < damageDist) {
                    player.GetComponent<Head>().takeDamage(damage);
                }
                Instantiate(Explosion, transform.position, transform.rotation);
                
                Destroy(gameObject);
            }
        }
    }
}
