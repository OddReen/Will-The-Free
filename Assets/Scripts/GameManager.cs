using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    enum GameState
    {
        MainMenu,
        DeathScreen,
        Wave,
        Rest
    }

    [SerializeField] GameState gameState = GameState.MainMenu;

    public GameObject gameplayHUD;

    [Header("InGameMenu")]
    [SerializeField] GameObject inGameMenu;

    [Header("Player Spawn")]
    [SerializeField] Transform playerSpawn;

    [Header("HealthBar")]
    [SerializeField] TextMeshProUGUI textHealthBar;
    [SerializeField] Image imageHealthBar;

    [Header("Ammo")]
    [SerializeField] TextMeshProUGUI textAmmo;

    [Header("Points")]
    [SerializeField] int points;
    [SerializeField] TextMeshProUGUI pointsToScreen;

    [Header("Player Death")]
    public GameObject DeathScreen;
    public Button restart;
    public Button quit;

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
        InputHandler.instance.OnInGameMenu += InGameMenu;
        restart.onClick.AddListener(OnRestart);
        quit.onClick.AddListener(OnQuit);
        player = Instantiate(player, playerSpawn.position, Quaternion.identity);
        StartWave();
    }

    public void InGameMenu()
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);
        Time.timeScale = inGameMenu.activeSelf ? 0.0f : 1.0f;
    }

    public void HealthBarUpdate(float InCurrentHealth, float InMaxHealth)
    {
        textHealthBar.text = InCurrentHealth + "/" + InMaxHealth;
        imageHealthBar.fillAmount = InCurrentHealth/InMaxHealth;
    }

    public void AmmoUpdate(float InCurrentAmmo, float InMagazineSize)
    {
        textAmmo.text = InCurrentAmmo + "/" + InMagazineSize;
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

    public void OnDeath()
    {
        gameplayHUD.SetActive(false);
        DeathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
