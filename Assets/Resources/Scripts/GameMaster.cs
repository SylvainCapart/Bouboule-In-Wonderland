using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class GameMaster : MonoBehaviour
{

    public static GameMaster gm;


    public Transform playerPrefab;
    private GameObject m_SpawnPoint;
    [SerializeField] private GameObject[] m_SpawnArray;

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

    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgrademenu;

    private AudioManager audioManager;

    public delegate void OnResetDelegate();
    public static event OnResetDelegate ResetDelegate;



    /*[SerializeField]
    private WaveSpawner waveSpawner;*/


    public GameObject SpawnPoint
    {
        get { return gm.m_SpawnPoint; }
        set { gm.m_SpawnPoint = value; }
    }

    /*public RespawnFlagMgt LastRespawnMgt
    {
        get{return gm.m_LastRespawnMgt;}
        set{ gm.m_LastRespawnMgt = value;}
    }*/

    public void EndGame()
    {
        //audioManager.PlaySound(gameOverSoundName);
        //gameOverUI.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (cameraShake == null)
        {
            Debug.LogError("No camera shake found in Gamemaster");
        }
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager missing in GameMaster");
        }

        m_SpawnArray = GameObject.FindGameObjectsWithTag("Flag");
        if (m_SpawnArray.Length >= 1)
        {
            m_SpawnPoint = m_SpawnArray[0];
            m_SpawnPoint.GetComponent<RespawnFlagMgt>().State = RespawnFlagMgt.FlagState.GREEN;
        }


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
        clone.transform.position = m_SpawnPoint.transform.position + new Vector3(0f, 0.5f, 0f);
        clone.name = "Player";

        //Transform spawnClone = Instantiate(spawnPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation);

        Camera2DFollow cameraFollow = Camera.main.GetComponentInParent<Camera2DFollow>();

        cameraFollow.target = clone.transform;
        gm.StartCoroutine(cameraFollow.DampingShutOff(0.1f));

        //Destroy(spawnClone.gameObject, 3f);
        ResetDelegate(); // called a second time to relink the statusindicator in the new player instance
        isRespawning = false;

    }

    public static void KillPlayer(Player player)
    {
        if (player.gameObject != null)
        {
            Destroy(player.gameObject);
            //ResetDelegate(); // called a first time to deactivate the audiosource linked to the player's audiolistener that is to be destroyed
        }
        else return;

        gm.StartCoroutine(gm.RespawnPlayer());
    }

    public static void KillEnemy(Enemy enemy)
    {

        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        //sound
        if (_enemy.m_DeathSoundName != "")
            audioManager.PlaySound(_enemy.m_DeathSoundName);

        //particles
        //Transform clone = Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity);
        //Destroy(clone.gameObject, 5f);

        //camerashake
        cameraShake.Shake(_enemy.m_ShakeAmountAmt, _enemy.m_ShakeLength);

        Destroy(_enemy.gameObject);

    }
    public static void InitializePlayerRespawn(Player player)
    {
        gm.m_SpawnPoint = player.gameObject;
    }

    public void ResetFlagsExcept(GameObject gameObj)
    {
        for (int i = 0; i < m_SpawnArray.Length; i++)
        {
            if (m_SpawnArray[i].GetComponent<RespawnFlagMgt>().State == RespawnFlagMgt.FlagState.GREEN
            && m_SpawnArray[i] != gameObj)
                m_SpawnArray[i].GetComponent<RespawnFlagMgt>().State = RespawnFlagMgt.FlagState.RED;
        }
    }


}
