using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameProgress : MonoBehaviour
{
    public TMP_Text VaseDeposits;

    public int depositedScore;
    public int gameStage = 0;

    public class BiomeData
    {
        public int Cost { get; set; }
        public GameObject Land { get; set; }

        public BiomeData(int cost, string landName)
        {
            Cost = cost;
            Land = GameObject.Find(landName);
        }
    }

    public List<BiomeData> biomeList;


    public void Deposit(int points)
    {
        FindObjectOfType<AudioManager>().Play("Hit");
        depositedScore += points;
        VaseDeposits.text = ""+depositedScore;
    }

    void Start()
    {
        Debug.Log("Game Start");

        biomeList = new List<BiomeData>();

        biomeList.Add(new BiomeData(0, "AppearBiome"));
        biomeList.Add(new BiomeData(10, "Crystal Biome"));
        biomeList.Add(new BiomeData(20, "NewLands"));

    }

    void Update()
    {
        if (depositedScore >= biomeList[gameStage + 1].Cost)
        {
            gameStage++;
        }

        BiomeData newBiome = biomeList[gameStage];
        if (Mathf.Abs(newBiome.Land.transform.position.y) > 3)
        {
            newBiome.Land.transform.Translate(Vector3.up * (newBiome.Land.transform.position.y * -0.3f * Time.deltaTime + 3));
            
            if (Mathf.Abs(newBiome.Land.transform.position.y) < 3)
            {
                newBiome.Land.transform.position = new Vector3(newBiome.Land.transform.position.x, 0, newBiome.Land.transform.position.z);
            }   
        }
    }
}