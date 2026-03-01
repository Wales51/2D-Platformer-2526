using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float JumpForce;
    Rigidbody2D rb;
    private Collider2D KillPoint;
    private bool Grounded;
    //private int overlaps = 0;
    public int MaxJumps;
    private int ExtraJumps;
    private GameObject GameManager;
    private Animator AC;
    private AnimatorOverrideController AOC;
    private GameManagement Manager;
    private ContactFilter2D Overfilter = new ContactFilter2D().NoFilter();
    private List<Collider2D> results = new List<Collider2D>();
    private bool Control = false;
    public int Health;
    private float SpawnAnimLength;
    private float DeathAnimLength;
    private float HurtAnimLength;
    private bool Spawning;
    private bool Dead;
    private bool hurt;
    private bool invulnerable = false;
    private double timer;
    private Vector2 ColliderSize;
    private CapsuleCollider2D cc;
    private float slopeCheckDistance;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private bool isOnSlope;
    private bool isJumping;
    private bool canWalkOnSlope;
    private Vector2 slopeNormalPerp;
    public float maxSlopeAngle;
    private Vector2 newVelocity;
    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullFriction;
    private LayerMask whatIsGround;
    private Vector3 groundCheck;
    private float groundCheckRadius;
    private bool canJump;
    private Vector2 newForce;
    private int Layer = 3; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        GameManager = GameObject.Find("GameManager");
        Manager = GameManager.GetComponent <GameManagement>();
        rb = GetComponent<Rigidbody2D>();
        AC = GetComponent<Animator>();
        KillPoint = transform.Find("KillPoint").gameObject.GetComponent<Collider2D>();
        ExtraJumps = MaxJumps - 1;
        Control = false;
        timer = 0;
        Spawning = true;
        hurt = false;
        AOC = new AnimatorOverrideController(AC.runtimeAnimatorController);
        AC.runtimeAnimatorController = AOC;
        DeathAnimLength = AOC["Ded"].length;
        SpawnAnimLength = AOC["Spawn"].length;
        HurtAnimLength = AOC["Hurt"].length;
        cc = GetComponent<CapsuleCollider2D>();
        ColliderSize = cc.size;
        groundCheckRadius = (ColliderSize.x/2)-(ColliderSize.x/10);
        slopeCheckDistance = (ColliderSize.x/2) + 0.1f;
        whatIsGround = 1 << Layer;
    }

    // Update is called once per frame

    void Update()
    {
        AC.SetBool("Hurt",hurt);
        if (Input.GetButtonDown("Jump") && Control==true)
        {
            Jump();
        }
        if (rb.linearVelocityX < 0.1f && rb.linearVelocityX > -0.1f)
        {
            AC.SetFloat("VelocityX",0);
        }
        else 
        {
            AC.SetFloat("VelocityX",rb.linearVelocityX);
        }
        AC.SetBool("Grounded",Grounded);
        AC.SetFloat("VelocityY",rb.linearVelocityY);
        if ((rb.linearVelocityX) >= 0.1)
        {
            AC.SetFloat("DirectionX",1);
        }
        if ((rb.linearVelocityX) <= -0.1)
        {
            AC.SetFloat("DirectionX",-1);
        }
    }

void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck, groundCheckRadius);
    }
    void FixedUpdate()
    {
        if(Spawning == true)
        {
            timer = timer + 0.02;
            if (timer >= SpawnAnimLength)
            {
                Spawning = false;
                Control = true;
                timer = 0;
            }
        }
        else if(Dead==true)
        {
            timer = timer + 0.02;
            if (timer >= DeathAnimLength)
            {
                Manager.Death(this.gameObject);
            }
        }
        else if (hurt == true)
        {
            timer = timer + 0.02;
            if (timer >= HurtAnimLength + 0.5)
            {
                Control = true;
                hurt = false;
                invulnerable = false;
                timer = 0;
            }
        }
        //AnimationUpdate();
        if(Control == true)
        {
        OverlapCheck();
        ApplyMovement();
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            ExtraJumps -= 1;
            canJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rb.linearVelocity = newVelocity;
            newForce.Set(0.0f, JumpForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }
    } 

    void OnEnd(InputValue value)
    {
        if (value.isPressed)
        {
            Manager.EndGame();
        }
    }

    void OverlapCheck()
    {
        CheckGround();
        SlopeCheck();
        //overlaps = GroundPoint.Overlap(new List<Collider2D> { });
        KillPoint.Overlap(Overfilter,results);
        foreach (Collider2D lap in results)
        {
            if (lap.tag == "Enemy")
            {
                Destroy(lap.GameObject());
            }
        }
    }

    private void CheckGround()
    {
        groundCheck = transform.position - new Vector3(0,(ColliderSize.y/2)-(ColliderSize.x/2)+0.3f,0);
        Grounded = Physics2D.OverlapCircle(groundCheck, groundCheckRadius, whatIsGround);

        if (ExtraJumps >= 1)
        {
            canJump = true;
        }

        if(Grounded && slopeDownAngle <= maxSlopeAngle)
        {
            isJumping = false;
            ExtraJumps = MaxJumps - 1;
            canJump = true;
        }

        if (Grounded)
        {
            canJump = true;
        }

        else isJumping = true;

    }
    public void Hurt()
    {
        if (invulnerable == false)
        {
            Control =false;
            --Health;
            if (Health <= 0)
            {
                AC.SetBool("Ded",true);
                Dead = true;
            }
            else
            {
                hurt = true;
                AC.SetBool("Hurt",true);
                invulnerable = true;
            }
        }
    }

    public void DeathZoneKill()
    {
        Health = 1;
        Hurt();
    }

    public void SpawnLoc(Vector3 Location)
    {
        Manager.CheckP(Location);
    }
    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, ColliderSize.y / 2));

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);
        // Ray Checks
        Debug.DrawLine(checkPos, new Vector2(checkPos.x+slopeCheckDistance,checkPos.y), Color.blue);
        Debug.DrawLine(checkPos, new Vector2(checkPos.x-slopeCheckDistance,checkPos.y), Color.blue);

        if (slopeHitFront)
        {
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            if (slopeSideAngle > 85 && Grounded)
            {
                isOnSlope = false;
                slopeSideAngle = 0.0f;
            }
            else isOnSlope = true;
        }
        else if (slopeHitBack)
        {
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            if (slopeSideAngle > 85 && Grounded)
            {
                isOnSlope = false;
                slopeSideAngle = 0.0f;
            }
            else isOnSlope = true;
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {      
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;            

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }                       

            lastSlopeAngle = slopeDownAngle;
           
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope && Input.GetAxis("Horizontal") == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }
    private void ApplyMovement()
    {
        if (Grounded && !isOnSlope && !isJumping) //if not on slope
        {
            Debug.Log("This one");
            newVelocity.Set(Speed * Input.GetAxis("Horizontal"), rb.linearVelocityY);
            rb.linearVelocity = newVelocity;
        }
        else if (Grounded && isOnSlope && canWalkOnSlope && !isJumping) //If on slope
        {
            newVelocity.Set(Speed * slopeNormalPerp.x * -Input.GetAxis("Horizontal"), Speed * slopeNormalPerp.y * -Input.GetAxis("Horizontal"));
            rb.linearVelocity = newVelocity;
        }
        else if (!Grounded) //If in air
        {
            newVelocity.Set(Speed * Input.GetAxis("Horizontal"), rb.linearVelocityY);
            rb.linearVelocity = newVelocity;
        }

    }

    public void Push(Vector2 Force)
    {
        rb.linearVelocity = Force;
    }
}