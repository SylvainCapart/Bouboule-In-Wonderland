using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_rollSpeed = .76f;          // Amount of maxSpeed applied to rolling movement. 1 = 100%
    [Range(0, 1)] [SerializeField] private float m_climbSpeed = .5f;          // Amount of maxSpeed applied to climbing movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_movementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_airControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_whatIsGround;							// A mask determining what is ground to the character                                                                   //[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private Transform[] m_GroundCheckTable;                           // A positions table marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
    [SerializeField] private Collider2D[] m_RollDisableColliderTable;                // A collider that will be disabled when rolling
    [SerializeField] private Collider2D m_RollCollider;                // A collider that will be disabled when rolling
    [SerializeField] private Collider2D m_climbingTrigger;                // A collider that detects objects where climbing is possible


    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private float[] k_GroundedRadiusTable = { .2f, .05f, .05f }; // Radius of the overlap circle to determine if grounded
    [SerializeField] private bool m_Grounded;            // Whether or not the player is grounded.
    public bool m_Rolling;             // Wether player is currently rolling or not
    public bool m_Climbing;           // Wether player is currently climbing or not

    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_velocity = Vector3.zero;
    private Animator m_Anim;
    private float m_MaxAnimSpeed = 3f;
    private float m_IncrementAnimSpeed = 1.5f;

    private bool inAir = false;       // to prevent the foot steps sound to be played twice
    private float m_normalGravity = 3.5f;
    [SerializeField] private float m_JumpDisableClimbingTime = 0.2f;
    [SerializeField] private float m_JumpReductionWhenClimbing = 0.5f;

    [SerializeField] private ParticleSystem m_SparkEffect;
    private float m_MaxSparkParticles = 60f;
    private float m_MaxSparkSimulationSpeed = 3f;


    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D == null)
            Debug.LogError(this.name + " : RB not found");

        m_Anim = this.GetComponentInParent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");

    }


	private void FixedUpdate()
	{
		m_Grounded = false;
        Collider2D[] colliders;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        //if (m_GroundCheckTable[0].gameObject.activeSelf)
        //{
            for (int i = 0; i < m_GroundCheckTable.Length; i++)
            {
                colliders = Physics2D.OverlapCircleAll(m_GroundCheckTable[i].position, k_GroundedRadiusTable[i], m_whatIsGround);

                for (int j = 0; j < colliders.Length; j++)
                {
                    if (colliders[j].gameObject != gameObject)
                        m_Grounded = true;
                }

            }
        //}

        if (!m_Grounded && !inAir)
        {
            inAir = true;
        }
        m_Anim.SetBool("Ground", m_Grounded);

        if (m_Grounded && inAir)
        {
            //audioManager.PlaySound(landingSoundname);
            inAir = false;
        }


        //Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_whatIsGround);

    }

    /*void OnGUI()
    {
        hSliderValue = GUI.HorizontalSlider(new Rect(25, 45, 100, 30), hSliderValue, 0.0F, 3.0F);
    }*/

    public void Move(float xMove, float yMove, bool jump, bool roll, bool climb)
	{
        m_Rolling = roll;
        m_Climbing = climb;

        // disable climbing if player is rolling
        if (roll && climb)
            climb = false;


        // If rolling, check to see if the character can stand up
        if (!roll)
        {
            // If the character has a ceiling preventing them from standing up, keep them rolling
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_whatIsGround) && !climb)
            {
                roll = true;
            }
        }



        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_airControl)
		{
            // If rolling
            if (roll)
            {
                float speedRatio = 0f;

                // if player is spitting fire, stop the fire before rolling
                if (GetComponentInChildren<FireThrow>().m_isSpittingFire)
                {
                    GetComponentInChildren<FireThrow>().StopSpitFire();
                }

                // Reduce the speed by the m_rollSpeed multiplier
                xMove *= m_rollSpeed;
                m_Anim.SetBool("Roll", true);
                m_RollCollider.enabled = true;


                if (xMove == 0f && yMove == 0f)
                {
                    m_Anim.speed += m_IncrementAnimSpeed * Time.deltaTime;

                    if (m_Anim.speed > m_MaxAnimSpeed)
                        m_Anim.speed = m_MaxAnimSpeed;

                    speedRatio = (m_Anim.speed  - 1f)/ m_MaxAnimSpeed;

                    EnableSparkEffect(true);

                    ParticleSystem.MainModule main = m_SparkEffect.main;
                    main.startSpeed = speedRatio * m_MaxSparkSimulationSpeed;

                    ParticleSystem.EmissionModule emissionModule = m_SparkEffect.emission;
                    emissionModule.rateOverTime = speedRatio * m_MaxSparkParticles;

                }
                else
                {
                    m_Anim.speed = 1f;
                    EnableSparkEffect(false);
                }

                for (int i = 0; i < m_GroundCheckTable.Length; i++)
                {
                    m_GroundCheckTable[i].gameObject.SetActive(false); 
                }

                for (int i = 0; i < m_RollDisableColliderTable.Length; i++)
                {
                    // Disable the colliders when rolling
                    if (m_RollDisableColliderTable[i] != null)
                        m_RollDisableColliderTable[i].enabled = false;
                }

            }
            else
            {
                m_Anim.SetBool("Roll", false);
                m_RollCollider.enabled = false;

                for (int i = 0; i < m_GroundCheckTable.Length; i++)
                {
                    m_GroundCheckTable[i].gameObject.SetActive(true);
                }

                for (int i = 0; i < m_RollDisableColliderTable.Length; i++)
                {
                    // Enable the colliders when not rolling
                    if (m_RollDisableColliderTable[i] != null)
                        m_RollDisableColliderTable[i].enabled = true;
                }

                m_Anim.speed = 1f;
                EnableSparkEffect(false);
            }

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("Speed", Mathf.Abs(xMove));

            // Move the character by finding the target velocity
            Vector3 targetVelocity;

            if (climb)
            {
                this.GetComponentInChildren<FireThrow>().StopSpitFire();
                m_Anim.SetBool("Climb", true);

                if (Mathf.Abs(xMove) > 0.1f || Mathf.Abs(yMove) > 0.1f)
                {
                    m_Anim.SetBool("DirectionPressed", true);
                }
                else
                {
                    m_Anim.SetBool("DirectionPressed", false);
                }


                targetVelocity = new Vector2(xMove * m_climbSpeed * 10f, yMove * m_climbSpeed * 10f);
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
            }
            else
            {
                m_Anim.SetBool("Climb", false);
                targetVelocity = new Vector2(xMove * 10f, m_Rigidbody2D.velocity.y);
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = m_normalGravity;
            }

            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_velocity, m_movementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (xMove > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (xMove < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if ((m_Grounded && jump && m_Anim.GetBool("Ground")) || (jump && climb))
		{
            // Add a vertical force to the player.
            StartCoroutine(DisableClimbFor(m_JumpDisableClimbingTime));
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);

            if (climb)
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * m_JumpReductionWhenClimbing));
            else
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

        }
        

    }


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    IEnumerator DisableClimbFor(float deactivateTime)
    {
        m_climbingTrigger.enabled = false;
        yield return (new WaitForSeconds(deactivateTime));
        m_climbingTrigger.enabled = true;
    }

    void EnableSparkEffect(bool state)
    {
        if (state)
        {
            if (!m_SparkEffect.isPlaying)
                m_SparkEffect.Play();
        }
        else
        {
            if (m_SparkEffect.isPlaying)
                m_SparkEffect.Stop();
        }
    }


}
