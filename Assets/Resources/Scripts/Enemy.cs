using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(EnemyAI))]
public class Enemy : MonoBehaviour {

    [System.Serializable]
    public class EnemyStats 
    {
        private int _currentHealth;

        public int maxHealth = 100;
        public float startPcHealh = 1f;
        public int damage = 40;

        public int CurrentHealth
        {
            get { return _currentHealth;}
            set { _currentHealth = Mathf.Clamp(value, 0, maxHealth);}
        }

        public void Init()
        {
            CurrentHealth = (int) (startPcHealh * maxHealth);
        }
    }

    public string deathSoundName = "Explosion";
    public int moneyDrop = 10;

    public EnemyStats stats;

    public Transform deathParticles;

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
            statusIndicator.SetHealth(stats.CurrentHealth, stats.maxHealth);
        }

        if (deathParticles == null)
        {
            Debug.Log("No death particles found in enemy");
        }

        GameMaster.gm.onToggleUpgrademenu += OnUpgradeMenuToggle;

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
            statusIndicator.SetHealth(stats.CurrentHealth, stats.maxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D _colInfo)
    {
        Player _player = _colInfo.collider.GetComponent<Player>();
        if(_player != null)
        {
            _player.DamagePlayer(stats.damage);
            DamageEnemy(9999999);
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
