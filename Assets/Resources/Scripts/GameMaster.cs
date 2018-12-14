using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class GameMaster : MonoBehaviour
{

    public static GameMaster gm;

    private static int m_RemainingLives;

    [SerializeField]
    private int maxLives = 3;

    
    public Transform playerPrefab;
    private static Vector3 m_SpawnPoint;
    [SerializeField] private Vector3 m_InitSpawnPoint;
    public float spawnDelay = 0.5f;
    public bool isRespawning = false;
    //public Transform spawnPrefab;


    public string respawnCountdownSoundName = "RespawnCountdown";
    public string spawnSoundName = "Spawn";
    public string gameOverSoundName = "GameOver";

    public CameraShake cameraShake;

    //[SerializeField]
    //private Transform gameOverUI;

    //[SerializeField]
    //private GameObject upgradeMenu;

    [SerializeField]
    private int startingMoney;
    public static int Money;

    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgrademenu;

    private AudioManager audioManager;

    /*[SerializeField]
    private WaveSpawner waveSpawner;*/

    public static int RemainingLives
    {
        get {return m_RemainingLives;}
        set {m_RemainingLives = value;}
    }

    public static Vector3 SpawnPoint
    {
        get {return m_SpawnPoint; }
        set {m_SpawnPoint = value;}
    }

    public void EndGame()
    {
        audioManager.PlaySound(gameOverSoundName);
        //gameOverUI.gameObject.SetActive(true);
    }

    private void Start()
    {
        m_RemainingLives = maxLives;
        if (cameraShake == null)
        {
            Debug.LogError("No camera shake found in Gamemaster");
        }
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager missing in GameMaster");
        }

        Money = startingMoney;
        m_SpawnPoint = m_InitSpawnPoint;

    }

    void Awake()
    {
        if (gm == null)
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
    }


    private void Update()
    {
        if (!isRespawning && Input.GetKeyDown(KeyCode.U))
        {
            //ToggleUpgradeMenu();
        }
    }

    private void ToggleUpgradeMenu()
    {
        //upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        //waveSpawner.enabled = !upgradeMenu.activeSelf;
        //onToggleUpgrademenu.Invoke(upgradeMenu.activeSelf);
    }

    public IEnumerator RespawnPlayer()
    {
        isRespawning = true;
        
        //audioManager.PlaySound(respawnCountdownSoundName);

        yield return new WaitForSeconds(spawnDelay);

        //audioManager.PlaySound(spawnSoundName);
        //Transform clone = Instantiate(playerPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation);
        GameObject clone = (GameObject)Instantiate(Resources.Load("Prefabs\\Player"));
        clone.transform.position = m_SpawnPoint;
        //Transform spawnClone = Instantiate(spawnPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation);

        Camera2DFollow cameraFollow = Camera.main.GetComponentInParent<Camera2DFollow>();

        cameraFollow.target = clone.transform;
        gm.StartCoroutine(cameraFollow.DampingShutOff(0.1f));

        //Destroy(spawnClone.gameObject, 3f);

        isRespawning = false;
    }

    public static void KillPlayer(Player player)
    {
        if (player.gameObject != null)
            Destroy(player.gameObject);
        else return;

        m_RemainingLives -= 1;

        if (m_RemainingLives <= 0)
        {
            //gm.EndGame();
        }
        else
        {
           // gm.StartCoroutine(gm.RespawnPlayer());
        }
        gm.StartCoroutine(gm.RespawnPlayer());
    }

    public static void KillEnemy(Enemy enemy)
    {

        gm._KillEnemy(enemy);
    }
    
    public void _KillEnemy(Enemy _enemy)
    {
        //sound
        audioManager.PlaySound(_enemy.deathSoundName);

        // gain money from enemy
        Money += _enemy.moneyDrop;
        audioManager.PlaySound("Money");

        //particles
        Transform clone = Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity);
        Destroy(clone.gameObject, 5f);

        //camerashake
        cameraShake.Shake(_enemy.shakeAmountAmt, _enemy.shakeLength);

        Destroy(_enemy.gameObject);

    }
    public static void InitializePlayerRespawn(Player player)
    {
        m_SpawnPoint = player.gameObject.transform.position;
    }

}
