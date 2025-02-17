using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    [Header("Points")]
    [SerializeField] int points;
    [SerializeField] TextMeshProUGUI pointsToScreen;

    [Header("Waves")]
    [SerializeField] int waveNumber;
    [SerializeField] TextMeshProUGUI wavesToScreen;
    [SerializeField] int maxEnemies;
    [SerializeField] int currentEnemies;
    [SerializeField] GameObject enemyPref;
    [SerializeField] Transform[] spawns;

    private void Awake()
    {
        instance = this;
        StartCoroutine(WaveSpawn());
        waveNumber = 1;
        maxEnemies = 5;
    }
    public void PointsChange(int amount)
    {
        points += amount;
        pointsToScreen.text = points.ToString();
    }
    public void EnemyKilled() 
    {
        currentEnemies--;
        PointsChange(100);
        if (currentEnemies <= 0)
        {
            NextWave();
        }
    }
    void NextWave()
    {
        waveNumber++;
        wavesToScreen.text = waveNumber.ToString();
        StartCoroutine(WaveSpawn());
    }
    IEnumerator WaveSpawn()
    {
        maxEnemies += 5;
        for (int i = 0; i < maxEnemies; i++)
        {
            currentEnemies++;
            int RandomSpawn = UnityEngine.Random.Range(0, spawns.Length);
            Instantiate(enemyPref, spawns[RandomSpawn].position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
