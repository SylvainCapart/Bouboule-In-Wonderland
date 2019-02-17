using System.Collections;
using UnityEngine;
using UnityStandardAssets._2D;

public class GameMaster : MonoBehaviour
{

    public static GameMaster gm;


    //public Transform playerPrefab;
    private GameObject m_SpawnPoint;
    private GameObject[] m_SpawnArray;

    [SerializeField] private Transform m_InitSpawnPoint;

    [SerializeField] private float m_SpawnDelay = 0.5f;
    [SerializeField] private float m_AppearDelay = 1f;

    public bool isRespawning = false;

    public string gameOverSoundName = "GameOver";

    public CameraShake m_CameraShake;

    private AudioManager m_AudioManager;
    [SerializeField] private string m_LastMainSoundStr = "Music";

    public delegate void PlayerRespawnDelegate();
    public static event PlayerRespawnDelegate OnPlayerRespawn;
    public delegate void PlayerKillDelegate();
    public static event PlayerKillDelegate OnPlayerKill;

    public bool m_DebugMode;
    public bool m_IntroSceneEnded;
    private bool m_EndReached;
    public float m_FinalTime;

    [SerializeField] private GameObject m_IntroScene;
    [SerializeField] private GameObject m_EndScene;
    [SerializeField] private Transform m_EndPlayerPos;

    void OnGUI()
    {
        if (m_DebugMode)
            GUI.Label(new Rect(0, 0, 100, 100), "" + (int)(1.0f / Time.smoothDeltaTime));
    }


    public GameObject SpawnPoint
    {
        get { return gm.m_SpawnPoint; }
        set { gm.m_SpawnPoint = value; }
    }

    public bool EndReached
    {
        get
        {
            return m_EndReached;
        }

        set
        {
            m_EndReached = value;
            if (m_EndReached == true)
            {
                m_IntroScene.SetActive(false);
                m_EndScene.SetActive(true);
                SpawnPoint = m_EndPlayerPos.gameObject;
                Player player = FindObjectOfType<Player>();
                if (player != null)
                    KillPlayer(player);
                DeathCounter.instance.DeathCount -= 1;
                m_FinalTime = Time.time;
            }
        }
    }

    /*public RespawnFlagMgt LastRespawnMgt
    {
        get{return gm.m_LastRespawnMgt;}
        set{ gm.m_LastRespawnMgt = value;}
    }*/

    private void Start()
    {
        m_CameraShake = CameraShake.instance;

        m_AudioManager = AudioManager.instance;

        m_SpawnArray = GameObject.FindGameObjectsWithTag("Flag");
        if (m_SpawnArray.Length >= 1)
        {
            m_SpawnPoint = m_SpawnArray[0];
            m_SpawnPoint.GetComponent<RespawnFlagMgt>().State = RespawnFlagMgt.FlagState.GREEN;
        }

        PlayerSpit playerspit = FindObjectOfType<PlayerSpit>();
        if (playerspit != null && gm.m_IntroSceneEnded == false)
            playerspit.IsSpittingAllowed = false;

        GameObject coincounter = GameObject.FindGameObjectWithTag("CoinCounter");
        if (coincounter != null)
            coincounter.SetActive(false);
        if (m_DebugMode)
        {
            if (playerspit != null)
                playerspit.IsSpittingAllowed = true;
            if (coincounter != null)
                coincounter.SetActive(true);
            gm.m_IntroSceneEnded = true;
        }

        if (OnPlayerRespawn != null)
            OnPlayerRespawn(); // called to relink the statusindicator in the new player instance

    }

    void Awake()
    {
        //if (gm == null)
        //  gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();

        if (gm != null)
        {
            if (gm != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            gm = this;
        }

        GameObject clone = (GameObject)Instantiate(Resources.Load("Prefabs\\Player"));
        clone.transform.position = m_InitSpawnPoint.position + new Vector3(0f, 0.5f, 0f);
        clone.name = "Player";

        //if (gm.m_IntroSceneEnded == false)
          //  clone.GetComponentInChildren<PlayerSpit>().IsSpittingAllowed = false;

        Camera2DFollow cameraFollow = Camera.main.GetComponentInParent<Camera2DFollow>();
        if (cameraFollow != null)
            cameraFollow.target = clone.transform;


    }

    public IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        yield return new WaitForSeconds(m_SpawnDelay);

        GameObject cloneappear = (GameObject)Instantiate(Resources.Load("Prefabs\\AppearPlayer"));
        cloneappear.transform.position = m_SpawnPoint.transform.position + new Vector3(0f, 0.5f, 0f);
        cloneappear.name = "AppearPlayer";

        Camera2DFollow cameraFollow = Camera.main.GetComponentInParent<Camera2DFollow>();
        if (cameraFollow != null)
            cameraFollow.target = cloneappear.transform;
        gm.StartCoroutine(cameraFollow.DampingShutOff(0.1f));

        yield return new WaitForSeconds(m_AppearDelay);

        Destroy(cloneappear);

        GameObject clone = (GameObject)Instantiate(Resources.Load("Prefabs\\Player"));
        clone.transform.position = m_SpawnPoint.transform.position + new Vector3(0f, 0.5f, 0f);
        clone.name = "Player";

        if (!gm.m_IntroSceneEnded)
            clone.GetComponentInChildren<PlayerSpit>().IsSpittingAllowed = false;

        cameraFollow.target = clone.transform;

        if (OnPlayerRespawn != null)
            OnPlayerRespawn(); // called to relink the statusindicator in the new player instance
        isRespawning = false;

    }

    public static void KillPlayer(Player player)
    {


        if (player.gameObject != null)
        {
            if (OnPlayerKill != null)
                OnPlayerKill(); // called to deactivate the audiosource linked to the player's audiolistener that is to be destroyed

            AudioManager.instance.CrossFade(AudioManager.instance.MainSound.name, gm.m_LastMainSoundStr, 2f, 2f, AudioManager.instance.GetSound(gm.m_LastMainSoundStr).initVol);
            DeathCounter.instance.DeathCount += 1;
            Destroy(player.gameObject);
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
            m_AudioManager.PlaySound(_enemy.m_DeathSoundName);

        //camerashake
        m_CameraShake.Shake(_enemy.m_ShakeAmountAmt, _enemy.m_ShakeLength);

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

    public string LastMainSoundStr
    {
        get
        {
            return m_LastMainSoundStr;
        }

        set
        {
            m_LastMainSoundStr = value;
        }
    }


}
