using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

//[RequireComponent(typeof(EnemyAI))]
public class Enemy : MonoBehaviour
{
    public enum Mode { IDLE, PATROL, TARGET };

    [SerializeField] private int m_CurrentHealth;

    [SerializeField] private int m_MaxHealth;
    private readonly float m_StartPcHealh = 1f;
    [SerializeField] private int m_Damage = 40;
    //[SerializeField] private float m_RepulsePlayerCoeff = 10f;

    public float m_ShakeAmountAmt = 0.1f;
    public float m_ShakeLength = 0.1f;

    public string m_DeathSoundName = "Explosion";

    [SerializeField] private bool m_Burnable;

    public int CurrentHealth
    {
        get { return m_CurrentHealth; }
        set { m_CurrentHealth = Mathf.Clamp(value, 0, MaxHealth); }
    }

    public int MaxHealth
    {
        get { return m_MaxHealth; }
        set { m_MaxHealth = value; }
    }

    //public Transform deathParticles;



    public void Init()
    {
        CurrentHealth = (int)(m_StartPcHealh * MaxHealth);
    }

    [Header("Optional : ")]
    [SerializeField]
    private ParticleSystem m_BurnEffect;

    [Header("Optional : ")]
    [SerializeField]
    private StatusIndicator statusIndicator;

    private void Start()
    {
        Init();

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(CurrentHealth, MaxHealth);
        }

        if (m_Burnable && m_BurnEffect == null)
            m_BurnEffect = GameObject.FindGameObjectWithTag("BurnEffect").GetComponent<ParticleSystem>();
        /*if (deathParticles == null)
        {
            Debug.Log("No death particles found in enemy");
        }*/

        //GameMaster.gm.onToggleUpgrademenu += OnUpgradeMenuToggle;

    }

    /*private void OnEnable()
    {
        if (m_Burnable)
            SourceFire.OnFireHit += Burn;
    }

    private void OnDisable()
    {
        if (m_Burnable)
            SourceFire.OnFireHit -= Burn;
    }*/

    public void DamageEnemy(int damageReceived)
    {
        CurrentHealth -= damageReceived;
        if (CurrentHealth <= 0)
        {
            GameMaster.KillEnemy(this);
        }

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(CurrentHealth, MaxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D _colInfo)
    {
        Player _player = _colInfo.collider.GetComponent<Player>();
        //Vector2 repulsiveVector;
        if (_player != null)
        {
            //repulsiveVector = new Vector2((_colInfo.transform.position.x - this.transform.position.x), _colInfo.transform.position.y - this.transform.position.y);
            _player.DamagePlayer(m_Damage);
            //_player.GetComponent<Rigidbody2D>().AddForce(repulsiveVector * m_RepulsePlayerCoeff, ForceMode2D.Impulse);
        }
    }


    private void FixedUpdate()
    {
        
    }


    void OnUpgradeMenuToggle(bool active)
    {
        //handle what happens when the upgrade menu is toggled
        /*Rigidbody2D _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero;
        }*/

        /*EnemyAI _AI = GetComponent<EnemyAI>();
        if (_AI != null)
            _AI.enabled = !active;

        Rigidbody2D _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
        {
            if (active)
            {
                _rb.bodyType = RigidbodyType2D.Static;

            }
            else
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
            }
                
        }*/
        //   _rb.gameObject.SetActive(!active);
        //GetComponent<Seeker>().enabled = !active;

    }

    private void OnParticleCollision(GameObject other)
    {
        if (m_Burnable && other.tag == "FireSource" && other.transform.parent.tag == "Player")
            Burn();
    }

    private void Burn()
    {

        m_BurnEffect.Play();
        DamageEnemy(1);
        //StartCoroutine(BurnCoroutine());

    }

    private IEnumerator BurnCoroutine()
    {
        m_BurnEffect.Play();

        yield return (new WaitForSeconds(m_BurnEffect.main.duration));

    }


}
