using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    GameObject player;
    public GameObject Body;
    public float timer = 2;
    public GameObject Explosion;
    public GameObject ExplosionArea;
    bool active = false;
    float triggerDist = 5;
    float damageDist = 15;
    float damage = 5;

    public AudioSource beeping;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Head");
        Body.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!active && Vector3.Distance(player.transform.position, transform.position) < triggerDist)
        {
            active = true;
            beeping.Play();
            Body.SetActive(true);
        }

        if (active)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                float playerDist = Vector3.Distance(player.transform.position, transform.position);
                if (playerDist < damageDist) {
                    player.GetComponent<Head>().takeDamage(damage*(1-playerDist/damageDist));
                }
                GameObject explosion = Instantiate(Explosion, transform.position, transform.rotation);
                explosion.GetComponent<Transform>().localScale = new Vector3(damageDist / 3.5f, damageDist / 3.5f, damageDist / 3.5f);

                Destroy(gameObject);
            }
        }
    }
}