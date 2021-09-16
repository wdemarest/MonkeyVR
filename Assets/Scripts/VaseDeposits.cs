using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VaseDeposits : MonoBehaviour
{
    [SerializeField] GameProgress gameProgress;
    
    void Start()
    {
        gameProgress = GameObject.Find("GameProgress").GetComponent<GameProgress>();
    }

    void Update()
    {
        GetComponent<TMP_Text>().text = ""+gameProgress.depositedScore;
    }
}
