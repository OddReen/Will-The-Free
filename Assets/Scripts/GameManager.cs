using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject[] players;

    enum GameState
    {
        MainMenu,
        Wave,
        Rest
    }

    [SerializeField] GameState gameState = GameState.MainMenu;

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

    private void Start()
    {
        instance = this;
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        Debug.Log("eyo");
        StartWave();
    }
    public void EnemyKilled()
    {
        PointsChange(100);
        if (NetworkManager.Singleton.IsClient)
        {
            return;
        }
        currentEnemies--;
        if (currentEnemies <= 0)
        {
            StartRest();
        }
    }
    public void PointsChange(int amount)
    {
        points += amount;
        pointsToScreen.text = points.ToString();
    }

    private void StartRest()
    {
        StartCoroutine(Rest());
    }
    IEnumerator Rest()
    {
        yield return new WaitForSeconds(1f);
        StartWave();
    }
    void StartWave()
    {
        waveNumber++;
        wavesToScreen.text = waveNumber.ToString();
        Debug.Log("eyo");
        StartCoroutine(Wave());
    }
    IEnumerator Wave()
    {
        maxEnemies += 5;
        currentEnemies = maxEnemies;
        for (int i = 0; i < maxEnemies; i++)
        {
            Debug.Log(i);
            int RandomSpawn = UnityEngine.Random.Range(0, spawns.Length);
            GameObject NewEnemy = Instantiate(enemyPref, spawns[RandomSpawn].position, Quaternion.identity);
            NewEnemy.GetComponent<NetworkObject>().Spawn();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
