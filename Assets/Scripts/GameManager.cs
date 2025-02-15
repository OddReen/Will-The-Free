using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    [Header("Waves")]
    [SerializeField] int waveNumber;
    [SerializeField] int maxEnemies;
    [SerializeField] int currentEnemies;
    [SerializeField] GameObject enemyPref;
    [SerializeField] Transform[] spawns;
    public Action OnEnemyKilled;

    private void Awake()
    {
        instance = this;
        StartCoroutine(Wave());
    }

    IEnumerator Wave()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            int RandomSpawn = UnityEngine.Random.Range(0, spawns.Length);
            Instantiate(enemyPref, spawns[RandomSpawn].position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
