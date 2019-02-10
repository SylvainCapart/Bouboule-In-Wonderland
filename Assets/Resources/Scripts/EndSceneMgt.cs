using UnityEngine;
using UnityStandardAssets._2D;

public class EndSceneMgt : MonoBehaviour
{
    private AudioManager m_AudioManager;
    private DialogueMgt m_DialogueMgt;
    private DialogueTrigger m_DialogueTrigger;
    [SerializeField] private Camera2DFollow m_CameraFollow;
    [SerializeField] private Transform m_CameraEndPoint;
    [SerializeField] private GameObject m_EndOverlay;

    

    // Start is called before the first frame update
    void Start()
    {
        m_AudioManager = AudioManager.instance;
        m_AudioManager.StopSoundSmooth(m_AudioManager.MainSound.name, 2f);
        m_DialogueMgt = DialogueMgt.instance;
       
    }

    private void OnEnable()
    {
        GameMaster.OnPlayerRespawn += PlayerStandStill;
        DialogueMgt.OnEmptyQueue += CameraUp;
    }

    private void OnDisable()
    {
        GameMaster.OnPlayerRespawn -= PlayerStandStill;
        DialogueMgt.OnEmptyQueue -= CameraUp;
    }


    void PlayerStandStill()
    {
        PlayerMovement playermov = FindObjectOfType<PlayerMovement>();
        if (playermov != null)
            playermov.IsMovementAllowed = false;

        PlayerSpit playerspit = FindObjectOfType<PlayerSpit>();
        if (playerspit != null)
            playerspit.IsSpittingAllowed = false;
    }

    void CameraUp()
    {
        m_CameraFollow.m_Damping = 2f;
        m_CameraFollow.target = m_CameraEndPoint;
        m_EndOverlay.SetActive(true);
    }

}
