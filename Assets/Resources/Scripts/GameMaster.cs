using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class GameMaster : MonoBehaviour
{

    public static GameMaster gm;

    private static int _remainingLives;

    [SerializeField]
    private int maxLives = 3;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 2f;
    public bool isRespawning = false;
    public Transform spawnPrefab;


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
        get
        {
            return _remainingLives;
        }

        set
        {
            _remainingLives = value;
        }
    }

    public void EndGame()
    {
        audioManager.PlaySound(gameOverSoundName);
        //gameOverUI.gameObject.SetActive(true);
    }

    private void Start()
    {
        _remainingLives = maxLives;
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

    public IEnumerator _RespawnPlayer()
    {
        isRespawning = true;
        
        //audioManager.PlaySound(respawnCountdownSoundName);

        yield return new WaitForSeconds(spawnDelay);

        //audioManager.PlaySound(spawnSoundName);
        Transform clone = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Transform spawnClone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);

        Camera2DFollow cameraFollow = Camera.main.GetComponentInParent<Camera2DFollow>();

        cameraFollow.target = clone;

        Destroy(spawnClone.gameObject, 3f);

        isRespawning = false;
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);

        _remainingLives -= 1;

        if (_remainingLives <= 0)
        {
            gm.EndGame();
        }
        else
        {
            gm.StartCoroutine(gm._RespawnPlayer());
        }
    }

    /*public static void KillEnemy(Enemy enemy)
    {

        gm._KillEnemy(enemy);
    }*/
    /*
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

    }*/

}
