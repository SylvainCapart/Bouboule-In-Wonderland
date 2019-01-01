using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_rollSpeed = .76f;          // Amount of maxSpeed applied to rolling movement. 1 = 100%
    [Range(0, 1)] [SerializeField] private float m_climbSpeed = .5f;          // Amount of maxSpeed applied to climbing movement. 1 = 100%
    [Range(0, 1)] [SerializeField] private float m_SwimSpeed = .5f;          // Amount of maxSpeed applied to swimming movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_movementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_airControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_whatIsGround;							// A mask determining what is ground to the character                                                                   //[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private Transform[] m_GroundCheckTable;                           // A positions table marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
    [SerializeField] private Collider2D[] m_RollDisableColliderTable;                // A collider that will be disabled when rolling
    [SerializeField] private Collider2D m_RollCollider;                // A collider that will be disabled when rolling
    [SerializeField] private Collider2D m_climbingTrigger;                // A collider that detects objects where climbing is possible
    [SerializeField] private string[] m_RollMovableTags;                // apply force on objets when olling

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private float[] k_GroundedRadiusTable = { .1f, .05f, .05f }; // Radius of the overlap circle to determine if grounded
    private float[] k_GroundedRadiusWaterTable = { .02f, .02f, .02f }; // Radius of the overlap circle to determine if grounded

    [SerializeField] private bool m_Grounded;            // Whether or not the player is grounded.
    private bool m_Rolling = false;             // Wether player is currently rolling or not
    private bool m_Climbing = false;            // Wether player is currently climbing or not
    private bool m_Jump = false;        // Wether player is currently jumping or not
    private bool m_Swim = false;        // Wether player can swim or not
    private bool m_Swimming = false;        // Wether player is currently swimming or not
    private bool m_Charging = false;        // Wether player is currently charging or not

    private bool m_StepAllowed = true;

    // event for the status above
    public delegate void OnMovementStatusChange(string statusName, bool state);
    public static event OnMovementStatusChange MovementStatusChange;

    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_velocity = Vector3.zero;
    private Animator m_Anim;
    private float m_MaxAnimSpeed = 3f;
    private float m_IncrementAnimSpeed = 1.5f;

    private bool inAir = false;       // to prevent the foot steps sound to be played twice
    private float m_normalGravity = 3.5f;
    private float m_archimedeGravity = 0.5f;
    private float m_DrowningSpeedReduction = 1f;
    private float m_JumpDisableClimbingTime = 0.2f;
    private float m_JumpReductionWhenClimbing = 0.5f;

    [SerializeField] private ParticleSystem m_SparkEffect;
    private float m_MaxSparkParticles = 60f;
    private float m_MaxSparkSimulationSpeed = 3f;
    private const float EPSILON = 0.01f;

    private AudioManager audioManager;

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), "" + (int)(1.0f / Time.smoothDeltaTime));

    }

    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audioManager found in " + this.name);
        }
    }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D == null)
            Debug.LogError(this.name + " : RB not found");

        m_Anim = this.GetComponentInParent<Animator>();
        if (m_Anim == null)
            Debug.LogError(this.name + " : Animator not found");



    }

    private void OnEnable()
    {
        Player.OnDrowning += OnDrowningHandler;
    }

    private void OnDisable()
    {
        Player.OnDrowning -= OnDrowningHandler;
    }

    private void FixedUpdate()
    {

        Grounded = false;
        Collider2D[] colliders;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.

        for (int i = 0; i < m_GroundCheckTable.Length; i++)
        {
            if (!Swim)
                colliders = Physics2D.OverlapCircleAll(m_GroundCheckTable[i].position, k_GroundedRadiusTable[i], m_whatIsGround);
            else
                colliders = Physics2D.OverlapCircleAll(m_GroundCheckTable[i].position, k_GroundedRadiusWaterTable[i], m_whatIsGround);

            for (int j = 0; j < colliders.Length; j++)
            {
                if (colliders[j].gameObject != gameObject)
                    Grounded = true;
            }

        }


        if (!Grounded && !inAir)
        {
            inAir = true;
        }
        m_Anim.SetBool("Ground", Grounded);




        if (Grounded && inAir)
        {
            audioManager.PlaySound("Landing");
            //StartCoroutine(StepShutOff(0.4f));
            inAir = false;
        }





        //Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_whatIsGround);

    }

    /*void OnGUI()
    {
        hSliderValue = GUI.HorizontalSlider(new Rect(25, 45, 100, 30), hSliderValue, 0.0F, 3.0F);
    }*/


    public void Move(float xMove, float yMove, bool jump, bool roll, bool charging, bool climb, bool swim)
    {

        // If rolling, check to see if the character can stand up
        if (!roll && !swim)
        {
            // If the character has a ceiling preventing them from standing up, keep them rolling
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_whatIsGround) && !climb)
            {
                roll = true;
            }
        }

        Rolling = roll;
        Climbing = climb;
        Charging = charging;
        Jump = jump;
        Swim = swim;

        // disable climbing if player is rolling
        if (roll && climb)
            climb = false;

        // if swimming, disable jump, rolling and climbing
        if (swim)
        {
            climb = false;
            roll = false;
            jump = false;
            Charging = false;
            Grounded = false;
        }
        else
        {
            Swimming = false;
        }
        // if not grounded, cancel charging
        Charging &= Grounded;

        m_Anim.SetBool("Swim", swim);



        //only control the player if grounded or airControl is turned on
        if (Grounded || m_airControl)
        {
            // If rolling
            if (roll)
            {
                float speedRatio = 0f;

                // Reduce the speed by the m_rollSpeed multiplier
                xMove *= m_rollSpeed;
                m_Anim.SetBool("Roll", true);
                m_RollCollider.enabled = true;


                if (System.Math.Abs(xMove) < EPSILON && System.Math.Abs(yMove) < EPSILON && Charging)
                {
                    RollCharge();

                    speedRatio = (m_Anim.speed - 1f) / m_MaxAnimSpeed;

                    ParticleSystem.MainModule main = m_SparkEffect.main;
                    main.startSpeed = speedRatio * m_MaxSparkSimulationSpeed;

                    ParticleSystem.EmissionModule emissionModule = m_SparkEffect.emission;
                    emissionModule.rateOverTime = speedRatio * m_MaxSparkParticles;

                }
                else
                {
                    RollChargeRelease();
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

                if (IsPlayerMovingX(xMove))
                    RollChargeRelease();
                else
                    RollChargeReset();

            }

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("XSpeed", Mathf.Abs(xMove));
            m_Anim.SetFloat("YSpeed", Mathf.Abs(xMove));

            // Move the character by finding the target velocity
            Vector3 targetVelocity = Vector3.zero;

            if (climb)
            {
                m_Anim.SetBool("Climb", true);

                if (Mathf.Abs(xMove) > 0.1f || Mathf.Abs(yMove) > 0.1f)
                {
                    m_Anim.SetBool("DirectionPressed", true);
                }
                else
                {
                    m_Anim.SetBool("DirectionPressed", false);
                }

                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
                targetVelocity = new Vector2(xMove * m_climbSpeed * 10f, yMove * m_climbSpeed * 10f);
            }
            else if (swim)
            {
                if (IsPlayerMoving(xMove, yMove) && !Grounded)
                {
                    Swimming = true;
                }
                else
                {
                    Swimming = false;
                }

                m_Anim.SetBool("Swimming", Swimming);
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = m_archimedeGravity;
                targetVelocity = new Vector2(xMove * m_SwimSpeed * 10f * m_DrowningSpeedReduction, yMove * m_SwimSpeed * 10f * m_DrowningSpeedReduction);
            }
            else
            {
                m_Anim.SetBool("Climb", false);
                m_Anim.SetBool("Swimming", false);
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = m_normalGravity;
                targetVelocity = new Vector2(xMove * GetSpeedRatio(), m_Rigidbody2D.velocity.y);
            }




            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_velocity, m_movementSmoothing);

            if (!Swimming && !Swim)
            {
                // If the input is moving the player right and the player is facing left...
                if (xMove > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    FlipScale();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (xMove < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    FlipScale();
                }
            }
            else if (Swim)
            {
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, GetSwimAngle(xMove, yMove)));

                // If the input is moving the player right and the player is facing left...
                if (xMove >= 0 && !m_FacingRight && IsPlayerMoving(xMove, yMove))
                {

                    FlipScale();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (xMove < 0 && m_FacingRight && IsPlayerMoving(xMove, yMove))
                {

                    FlipScale();
                }
            }
            else
            {
                // Do nothing
            }
        }
        // If the player should jump...
        if (!Swim && ((Grounded && jump && m_Anim.GetBool("Ground")) || (jump && climb) ))
        {
            // Add a vertical force to the player.
            StartCoroutine(DisableClimbFor(m_JumpDisableClimbingTime));
            Grounded = false;
            m_Anim.SetBool("Ground", false);

            if (climb)
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * m_JumpReductionWhenClimbing));
            else
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

        }

        if (Grounded && !inAir && m_StepAllowed && IsPlayerMovingX(xMove) && !roll)
        {
            audioManager.PlaySound("FootStep");
            StartCoroutine(StepShutOff(0.4f));

        }

       
            

        /*if (Rolling && Grounded)
        {
            audioManager.PlaySound("Rolling");
        }
        else if(!Rolling || !Grounded)
        {
            audioManager.StopSound("Rolling");
        }*/
    }




    private void FlipScale()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void FlipRotate()
    {

        transform.Rotate(new Vector3(0, 180, 0));
        //   transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0, transform.rotation.z));
        //transform.Rotate Quaternion.Euler(new Vector3(transform.rotation.x, 180, transform.rotation.z));
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

    }



    private void GroundSecurityCheck()
    {

    }

    float GetSwimAngle(float x, float y)
    {
        float moveDetected = 0.0f;
        float retAngle = 0f;

        if (x > moveDetected && y > moveDetected) //up right
            retAngle = 45f;
        else if (x > moveDetected && System.Math.Abs(y) < EPSILON) // right
            retAngle = 0f;
        else if (x > moveDetected && y < moveDetected) // bottom right
            retAngle = -45f;
        else if (System.Math.Abs(x) < EPSILON && y < moveDetected) // bottom
            retAngle = -90f;
        else if (x < moveDetected && y < moveDetected) // bottom left
            retAngle = 45f;
        else if (x < moveDetected && System.Math.Abs(y) < EPSILON) // left
            retAngle = 0f;
        else if (x < moveDetected && y > moveDetected) // up left
            retAngle = -45f;
        else if (System.Math.Abs(x) < EPSILON && y > moveDetected) // up
            retAngle = 90f;
        else if (System.Math.Abs(x) < EPSILON && System.Math.Abs(y) < EPSILON) // no move
            retAngle = 0f;
        else
        {
            retAngle = 0f;
            Debug.LogError("angle not possible");
        }
        return retAngle;
    }

    IEnumerator DisableClimbFor(float deactivateTime)
    {
        m_climbingTrigger.enabled = false;
        yield return (new WaitForSeconds(deactivateTime));
        m_climbingTrigger.enabled = true;
    }



    void EnableParticleEffect(ParticleSystem part, bool state)
    {
        if (state)
        {
            if (!part.isPlaying)
                part.Play();
        }
        else
        {
            if (part.isPlaying)
                part.Stop();
        }
    }



    void RollCharge()
    {
        m_Anim.speed += m_IncrementAnimSpeed * Time.deltaTime;

        if (m_Anim.speed > m_MaxAnimSpeed)
            m_Anim.speed = m_MaxAnimSpeed;

        if (!Jump)
            EnableParticleEffect(m_SparkEffect, true);
        else
            EnableParticleEffect(m_SparkEffect, false);

        audioManager.PlaySoundAt("FastRolling", (m_Anim.speed *2f) -0.825f); //hardcoded, to be changed

    }

    void RollChargeRelease()
    {
        m_Anim.speed -= 1 * m_IncrementAnimSpeed * Time.deltaTime;

        if (m_Anim.speed <= 1f)
        {
            m_Anim.speed = 1f;


        }
        EnableParticleEffect(m_SparkEffect, false);
        audioManager.StopSound("FastRolling");
    }

    void RollChargeReset()
    {
        m_Anim.speed = 1f;
        EnableParticleEffect(m_SparkEffect, false);
        audioManager.StopSound("FastRolling");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Rolling)
        {
            foreach (string tag in m_RollMovableTags)
            {
                if (collision.gameObject.tag == tag)
                {
                    Vector2 oppositeForce;
                    oppositeForce.y = 0f;

                    if (m_FacingRight)
                        oppositeForce.x = -7.5f * m_Anim.speed / collision.rigidbody.mass - 20f;
                    else
                        oppositeForce.x = 7.5f * m_Anim.speed / collision.rigidbody.mass + 20f;

                    collision.rigidbody.AddForce(oppositeForce);
                }
            }
        }
    }


    public void OnDrowningHandler(bool drowning)
    {
        if (drowning == true)
            m_DrowningSpeedReduction = 0.5f;
        else
            m_DrowningSpeedReduction = 1f;
    }

    float GetSpeedRatio()
    {
        float speedRatio = (-0.7f * m_Anim.speed + 6.3f) * m_Anim.speed;
        return speedRatio;
    }

    bool IsPlayerMoving(float x, float y)
    {
        return (Mathf.Abs(x) > 0.1f || Mathf.Abs(y) > 0.1f);
    }

    bool IsPlayerMovingX(float x)
    {
        return (Mathf.Abs(x) > 0.1f);
    }



    public bool Rolling
    {
        get
        {
            return m_Rolling;
        }

        set
        {



            if (m_Rolling == value) return;
            m_Rolling = value;
            if (MovementStatusChange != null)
                MovementStatusChange("Rolling", value);
        }
    }

    public bool Climbing
    {
        get
        {
            return m_Climbing;
        }

        set
        {
            if (m_Climbing == value) return;
            m_Climbing = value;
            if (MovementStatusChange != null)
                MovementStatusChange("Climbing", value);
        }
    }

    public bool Jump
    {
        get
        {
            return m_Jump;
        }

        set
        {
            if (m_Jump == value) return;
            m_Jump = value;
            if (MovementStatusChange != null)
                MovementStatusChange("Jump", value);
        }
    }

    public bool Swim
    {
        get
        {
            return m_Swim;
        }

        set
        {
            if (m_Swim == value) return;
            m_Swim = value;
            if (MovementStatusChange != null)
                MovementStatusChange("Swim", value);
        }
    }

    public bool Swimming
    {
        get
        {
            return m_Swimming;
        }

        set
        {
            if (m_Swimming == value) return;
            m_Swimming = value;
            if (MovementStatusChange != null)
                MovementStatusChange("Swimming", value);

        }
    }

    public bool Charging
    {
        get
        {
            return m_Charging;
        }

        set
        {
            /*if (m_Charging == true && value == true)
            {
                if (!audioManager.IsSoundPlayed("FastRolling"))
                {
                    audioManager.StopSound("FastRolling");
                    audioManager.PlaySoundAt("FastRolling", 4f);
                }

            }*/
            if (m_Charging == value) return;
            m_Charging = value;
            if (MovementStatusChange != null)
                MovementStatusChange("Charging", value);
        }
    }

    public bool Grounded
    {
        get
        {
            return m_Grounded;
        }

        set
        {
            if (m_Grounded == value) return;
            m_Grounded = value;
            if (MovementStatusChange != null)
                MovementStatusChange("Grounded", value);
        }
    }

    public IEnumerator StepShutOff(float delay)
    {
        m_StepAllowed = false;
        yield return (new WaitForSeconds(delay));
        m_StepAllowed = true;
    }

}
