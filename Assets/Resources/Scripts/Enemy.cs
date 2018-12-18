using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(EnemyAI))]
public class Enemy : MonoBehaviour {

    [System.Serializable]
    public class EnemyStats 
    {
        public int _currentHealth;

        [SerializeField] private int maxHealth;
        public float startPcHealh = 1f;
        public int damage = 40;
        public float repulsePlayerCoeff = 10f;

        public int CurrentHealth
        {
            get { return _currentHealth;}
            set { _currentHealth = Mathf.Clamp(value, 0, MaxHealth);}
        }

        public int MaxHealth
        {
            get{return maxHealth;}
            set{maxHealth = value;}
        }

        public void Init()
        {
            CurrentHealth = (int) (startPcHealh * MaxHealth);
        }
    }

    public string deathSoundName = "Explosion";
    public int moneyDrop = 10;

    public EnemyStats stats;

    //public Transform deathParticles;

    public float shakeAmountAmt = 0.1f;
    public float shakeLength = 0.1f;


    [Header("Optional : ")]
    [SerializeField]
    private StatusIndicator statusIndicator;

    private void Start()
    {
        stats = new EnemyStats();
        stats.Init();

        if (statusIndicator != null)
        {
           statusIndicator.SetHealth(stats.CurrentHealth, stats.MaxHealth);
        }

        /*if (deathParticles == null)
        {
            Debug.Log("No death particles found in enemy");
        }*/

        //GameMaster.gm.onToggleUpgrademenu += OnUpgradeMenuToggle;

    }

    public void DamageEnemy(int damageReceived)
    {
        stats.CurrentHealth -= damageReceived;
        if (stats.CurrentHealth <= 0)
        {
            GameMaster.KillEnemy(this);
        }

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.CurrentHealth, stats.MaxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D _colInfo)
    {
        Player _player = _colInfo.collider.GetComponent<Player>();
        Vector2 repulsiveVector;
        if (_player != null)
        {
            repulsiveVector = new Vector2(_colInfo.transform.position.x - this.transform.position.x, _colInfo.transform.position.y - this.transform.position.y);
            _player.DamagePlayer(stats.damage);
            _player.GetComponent<Rigidbody2D>().AddForce(repulsiveVector * stats.repulsePlayerCoeff, ForceMode2D.Impulse);
        }
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


}
