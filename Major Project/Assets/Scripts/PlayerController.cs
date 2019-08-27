using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float m_currentMovementSpeed = 10;
    public float m_velocityChange = 10;
    public float m_jumpHeight = 10;
    public float m_gravity = 20;
    public float m_colliderHeight = 2.4f;
    public float m_colliderRadius = 0.4f;
    public float m_minColorDistance;
    public float m_maxColorDistance;

    public bool m_freezeInputs = false;

    public float m_climbMinHeight = 1f;
    public float m_climbMaxHeight = 2f;
    public float m_climbMaxDistance = 1f;
    public float m_timeToClimb = 1;

    public bool m_jumpEvent = false;
    public bool m_climbEvent = false;

    private int lerpFactorID = 0;
    private bool m_grounded = true;
    private bool m_jumped = false;

    private Vector3 m_targetVelocity;
    private GameObject m_sideCameraPosition;
    private GameObject m_topCameraPosition;
    private GameObject m_background;

    private CapsuleCollider m_collider;
    private Camera m_mainCamera;
    private Rigidbody m_rigidbody;
    private RaycastHit m_groundHit;
    public enum State { Top, Side };
    public State playerState;
    private List<BlueColorer> colorerList = new List<BlueColorer>();

    private Animator m_animator;

    private bool m_isClimbing = false;
    private bool m_climbed = false;

    private Vector3 m_climbPosition;

    private Vector3 m_climbStartPosition;

    private float m_currentClimbTime = 0;

    private float m_currentGravity;

    private void OnValidate()
    {
        m_currentMovementSpeed = Mathf.Clamp(m_currentMovementSpeed, 0, float.MaxValue);
        m_velocityChange = Mathf.Clamp(m_velocityChange, 0, float.MaxValue);
        m_jumpHeight = Mathf.Clamp(m_jumpHeight, 0, float.MaxValue);
        m_gravity = Mathf.Clamp(m_gravity, 0, float.MaxValue);
        m_colliderHeight = Mathf.Clamp(m_colliderHeight, 0, float.MaxValue);
        m_colliderRadius = Mathf.Clamp(m_colliderRadius, 0, float.MaxValue);
        m_minColorDistance = Mathf.Clamp(m_minColorDistance, 0, float.MaxValue);
        m_maxColorDistance = Mathf.Clamp(m_maxColorDistance, 0, float.MaxValue);
    }


    void Awake()
    {
        //colorerList = new List<BlueColorer>();
        m_mainCamera = Camera.main;
        m_rigidbody = GetComponent<Rigidbody>();
        m_background = GameObject.Find("Background");
        m_sideCameraPosition = GameObject.Find("SideCameraPosition");
        m_topCameraPosition = GameObject.Find("TopCameraPosition");

        lerpFactorID = Shader.PropertyToID("_LerpFactor");

        m_collider = gameObject.AddComponent<CapsuleCollider>();
        m_currentGravity = m_gravity;


        //m_collider = GetComponentInChildren<CapsuleCollider>();
        m_collider.radius = m_colliderRadius;
        m_collider.height = m_colliderHeight;
        m_collider.center = new Vector3(0, m_colliderHeight * 0.5f, 0);

        playerState = State.Side;

        m_animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = 0;

        if (playerState == State.Top)
        {
            TopUpdate();
            angle = Mathf.Atan2(-Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * Mathf.Rad2Deg;
        }
        else if (playerState == State.Side)
        {
            SideUpdate();
            angle = Mathf.Atan2(0, Input.GetAxis("Horizontal")) * Mathf.Rad2Deg;
        }

        if (Input.GetAxis("Vertical") != 0 && m_grounded || Input.GetAxis("Horizontal") != 0 && m_grounded) //true when moving
        {
            transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(0, angle - 90, 0));
        }


        //Moves the Background with the x movement of the player
        m_background.transform.position = new Vector3(transform.position.x, m_background.transform.position.y, m_background.transform.position.z);

    }

    private void FixedUpdate()
    {
        UpdateAnimator();
        if (playerState == State.Top)
        {
            FixedTopUpdate();
        }
        else if (playerState == State.Side)
        {
            FixedSideUpdate();
        }

        GroundCheck();
        // Manual gravity for more control
        m_rigidbody.AddForce(new Vector3(0, -m_currentGravity * m_rigidbody.mass, 0));

    }

    void TopUpdate()
    {
        if (!m_freezeInputs)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerState = State.Side;
                ChangeAllColor();
            }

            m_targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        //sets camera position and rotation
        m_mainCamera.transform.position = m_topCameraPosition.transform.position;
        m_mainCamera.transform.rotation = m_topCameraPosition.transform.rotation;

        //Inputs never in fixedUpdate
    }

    void SideUpdate()
    {
        //sets camera position and rotation
        m_mainCamera.transform.position = m_sideCameraPosition.transform.position;
        m_mainCamera.transform.rotation = m_sideCameraPosition.transform.rotation;
        if (!m_freezeInputs)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerState = State.Top;
                ChangeAllColor();
            }


            //Inputs never in fixedUpdate
            m_targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        }

        if (!m_jumped)
        {
            Climping();
        }
    }

    void FixedTopUpdate()
    {
        if (m_grounded)
        {
            // Calculates how fast the player moves
            m_targetVelocity = transform.TransformDirection(m_targetVelocity);
            m_targetVelocity *= m_currentMovementSpeed;

            // Adds the grounds direction to the movement
            m_targetVelocity = Vector3.Cross(m_targetVelocity, m_groundHit.normal);
            m_targetVelocity = Vector3.Cross(-m_targetVelocity, m_groundHit.normal);

            // Applys a force that attempts to reach the target velocity
            Vector3 velocity = m_rigidbody.velocity;
            Vector3 velocityChange = (m_targetVelocity - velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -m_velocityChange, m_velocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -m_velocityChange, m_velocityChange);
            velocityChange.y = 0;

            m_rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        }
    }

    void FixedSideUpdate()
    {
        if (m_grounded)
        {
            // Calculates how fast the player moves
            m_targetVelocity = transform.TransformDirection(m_targetVelocity);
            m_targetVelocity *= m_currentMovementSpeed;

            // Adds the grounds direction to the movement
            m_targetVelocity = Vector3.Cross(m_targetVelocity, m_groundHit.normal);
            m_targetVelocity = Vector3.Cross(-m_targetVelocity, m_groundHit.normal);

            // Applys a force that attempts to reach the target velocity
            Vector3 velocity = m_rigidbody.velocity;
            Vector3 velocityChange = (m_targetVelocity - velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -m_velocityChange, m_velocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -m_velocityChange, m_velocityChange);
            velocityChange.y = 0;

            if (!m_isClimbing)
            {
                m_rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }


            if (m_jumpEvent)
            {
                m_rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                m_jumpEvent = false;
            }
        }
    }

    private void GroundCheck()
    {
        if (Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + m_colliderRadius * 2, transform.position.z), m_colliderRadius, -transform.up, out m_groundHit, 0.1f + m_colliderRadius))
        {
            if (Vector3.Angle(m_groundHit.normal, Vector3.up) != 0)
            {
                m_currentGravity = m_gravity;
            }
            else
            {
                m_currentGravity = m_gravity * 0.5f;
            }
            m_grounded = true;
            m_jumped = false;
        }
        else
        {
            m_grounded = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + m_colliderRadius - 0.1f, transform.position.z), m_colliderRadius);
    }

    private float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2 * m_jumpHeight * m_gravity);
    }

    public void AddColorerList(BlueColorer _colorer)
    {
        colorerList.Add(_colorer);
        ChangeColor(_colorer);
    }

    public void RemoveColorerList(BlueColorer _colorer)
    {
        colorerList.Remove(_colorer);
    }

    private void ChangeColor(BlueColorer _colorer)
    {
        if (playerState == State.Side)
        {
            float tmp = _colorer.gameObject.transform.position.z - transform.position.z;
            if (tmp < m_minColorDistance)
            {
                _colorer.m_material.SetFloat(lerpFactorID, 0);
            }
            else
            {
                _colorer.m_material.SetFloat(lerpFactorID, Mathf.Min(tmp / m_maxColorDistance, 1));
            }

        }
        else
        {
            _colorer.m_material.SetFloat(lerpFactorID, 0);
        }
    }

    private void ChangeAllColor()
    {
        foreach (BlueColorer item in colorerList)
        {
            ChangeColor(item);
        }
    }

    void UpdateAnimator()
    {
        float f = Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical"));
        m_animator.SetFloat("MovementSpeed", f);

        if (m_jumped)
        {
            m_animator.SetTrigger("Jump");
            m_jumped = false;
        }

        if (m_climbed)
        {
            m_animator.SetTrigger("Climb");
            m_climbed = false;

        }

        m_animator.SetFloat("Y Velocity", m_rigidbody.velocity.y);
    }

    public void Climping()
    {

        if (Input.GetButtonDown("Jump") && m_grounded && !m_freezeInputs)
        {
            if (!m_isClimbing)
            {
                RaycastHit frontHit = new RaycastHit();
                //Checks if something is in front of the player
                Vector3 rayPosFront = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
                if (Physics.BoxCast(new Vector3(transform.position.x, transform.position.y + (m_climbMaxHeight - m_climbMinHeight) * 0.5f, transform.position.z), new Vector3(0.01f, (m_climbMaxHeight - m_climbMinHeight) * 0.4f, 0.01f), transform.GetChild(0).forward, out frontHit, transform.GetChild(0).rotation, m_climbMaxDistance))
                {
                    RaycastHit topHit = new RaycastHit();
                    Vector3 offset = (frontHit.point - rayPosFront).normalized * 0.1f;
                    Vector3 rayPosTop = new Vector3(frontHit.point.x, rayPosFront.y + m_climbMaxHeight, frontHit.point.z) + offset;

                    //Checks the height of the object infront of the player
                    if (Physics.Raycast(rayPosTop, -transform.up, out topHit, m_climbMaxHeight - m_climbMinHeight))
                    {
                        Debug.DrawRay(rayPosTop, -transform.up, Color.red, 5f);
                        //Checks if the player would fit on object
                        if (!Physics.CapsuleCast(new Vector3(frontHit.point.x, topHit.point.y + m_colliderRadius, frontHit.point.z),
                                                new Vector3(frontHit.point.x, topHit.point.y + m_colliderHeight - m_colliderRadius, frontHit.point.z), m_colliderRadius, transform.GetChild(0).forward,
                                                Vector3.Distance(new Vector3(frontHit.point.x, topHit.point.y + m_colliderRadius, frontHit.point.z), topHit.point)))
                        {
                            m_climbPosition = topHit.point;
                            m_isClimbing = true;
                            m_climbed = true;
                            m_climbStartPosition = transform.position;
                        }
                    }
                }
            }

            if (!m_isClimbing)
            {
                m_jumped = true;
            }

        }


        if (m_climbEvent)
        {
            m_currentClimbTime += Time.deltaTime * m_timeToClimb;
            transform.position = Vector3.Lerp(m_climbStartPosition, m_climbPosition, m_currentClimbTime);
            if (m_currentClimbTime >= 1)
            {
                m_climbEvent = false;
                m_isClimbing = false;
                m_currentClimbTime = 0;
            }
        }
    }
}
