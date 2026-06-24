using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

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
        if (instance == null)
        {
            instance = this;
        }
        player = Instantiate(player);
        StartWave();
    }
    public void OnEnemyDeath()
    {
        AddPoints(100);
        currentEnemies--;
        if (currentEnemies <= 0)
        {
            StartCoroutine(WaveInterval());
        }
    }

    public void AddPoints(int amount)
    {
        points += amount;
        pointsToScreen.text = points.ToString();
    }

    IEnumerator WaveInterval()
    {
        yield return new WaitForSeconds(1f);
        StartWave();
    }

    void StartWave()
    {
        waveNumber++;
        wavesToScreen.text = waveNumber.ToString();
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        maxEnemies += 5;
        currentEnemies = maxEnemies;
        for (int i = 0; i < maxEnemies; i++)
        {
            int RandomSpawn = Random.Range(0, spawns.Length);
            GameObject NewEnemy = Instantiate(enemyPref, spawns[RandomSpawn].position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
