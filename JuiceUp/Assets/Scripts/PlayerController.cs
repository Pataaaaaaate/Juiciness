using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float Speed = 1000;
    private bool RotateToDirection = false; // Rotate To The Movement Direction
    private bool RotateWithMouseClick = false; // Rotate To The Direction Of The Mouse When Click , Usefull For Attacking

    [Header("Jumping")]
    public float JumpPower = 800; // How High The Player Can Jump
    public float Gravity = 3; // How Fast The Player Will Pulled Down To The Ground, 6 Feels Smooth
    public int AirJumps = 1; // Max Amount Of Air Jumps, Set It To 0 If You Dont Want To Jump In The Air
    public LayerMask jumpableLayer; // The Layers That Represent The Ground, Any Layer That You Want The Player To Be Able To Jump In

    [Header("Dashing")]
    public float DashPower = 150; // It Is A Speed Multiplyer, A Value Of 2 - 3 Is Recommended.
    public float DashDuration = 0.50f; // Duration Of The Dash In Seconds, Recommended 0.20f.
    public float DashCooldown = 0.3f; // Duration To Be Able To Dash Again.
    public bool AirDash = true; // Can Dash In Air ?

    [Header("Attacking")]
    public GameObject BulletPrefab;

    // Private Variables
    bool canMove = true;
    bool canDash = true;

    float MoveDirection;
    int currentJumps = 0;
    bool wasInAir = false;

    private Vector3 originalScale;

    Rigidbody2D rb;
    BoxCollider2D col; // Change It If You Use Something Else That Box Collider, Make Sure You Update The Reference In Start Function


    public GameObject doubleJumpVFX;
    public GameObject dashVFX;

    ////// START & UPDATE :

    void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = Gravity;
        originalScale = transform.localScale; // garde sa forme originale

    }   
    void Update()
    {
        // Get Player Movement Input
        MoveDirection = (Input.GetAxisRaw("Horizontal")); 
        // Rotation
        RotateToMoveDirection();

        // Rotate and Attack When Click Left Mouse Button
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RotateToMouse();
            Attack();
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Dashing
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (MoveDirection != 0 && canDash)
            {
                if (!AirDash && !InTheGround())
                    return;


                StartCoroutine(Dash());
            }
        }

        // Ici, on vérifie l'état de contact avec le sol
        bool grounded = InTheGround();

        if (grounded && wasInAir)
        {
            StartCoroutine(SquashStretch());
        }

        // Met à jour le statut pour la frame suivante
        wasInAir = !grounded;
    }
    void FixedUpdate()
    {
        Move();
    } 

    ///// MOVEMENT FUNCTIONS :

    void Move()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(MoveDirection * Speed * Time.fixedDeltaTime, rb.linearVelocity.y);
            
        }

    } 
    bool InTheGround()
    {
        // Make sure you set the ground layer to the ground
        RaycastHit2D ray;

         if (transform.rotation.y == 0)
         {
            Vector2 position = new Vector2(col.bounds.center.x - col.bounds.extents.x, col.bounds.min.y);
             ray = Physics2D.Raycast(position, Vector2.down, col.bounds.extents.y + 0.2f, jumpableLayer);
         }
         else
         {
            Vector2 position = new Vector2(col.bounds.center.x + col.bounds.extents.x, col.bounds.min.y);
            ray = Physics2D.Raycast(position, Vector2.down, col.bounds.extents.y + 0.2f, jumpableLayer);
         }       

        if (ray.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Jump()
    {

        if (InTheGround())
        {
            rb.linearVelocity = Vector2.up * JumpPower;
            
            currentJumps = 0; // Reset le compteur de sauts quand on touche le sol
        }
        else
        {
            if (currentJumps >= AirJumps)
            {
                currentJumps++;
                rb.linearVelocity = Vector2.up * JumpPower;
                Instantiate(doubleJumpVFX, transform.position, Quaternion.identity);
            }
                //return;

            
           
        }

    }

    void Attack()
    {
        Instantiate(BulletPrefab, transform.position, transform.rotation);
    }

    void RotateToMoveDirection()
    {
        if (!RotateToDirection)
            return;

        if (MoveDirection != 0 && canMove)
        {
            if (MoveDirection > 0)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                
            }
            else
            {
                transform.rotation = new Quaternion(0, 180, 0, 0);
            }
        }
    }

    ///// SPECIAL  FUNCTIONS : 

    // Multiply The Speed With Certain Amount For A Certain Duration
    IEnumerator Dash()
    {
        canDash = false;
        float originalSpeed = Speed; 
       
        Speed *= DashPower;
        rb.gravityScale = 0f; // You can delete this line if you don't want the player to freez in the air when dashing
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        Instantiate(dashVFX, transform.position, Quaternion.identity);

        //  You Can Add A Camera Shake Function here

        yield return new WaitForSeconds(DashDuration); 

        rb.gravityScale = Gravity;
        Speed = originalSpeed;

        yield return new WaitForSeconds(DashCooldown - DashDuration);

        canDash = true;
    }

    // Make Player Facing The Mouse Cursor , Can Be Called On Update , Or When The Player Attacks He Will Turn To The Mouse Direction
    void RotateToMouse()
    {
        if (!RotateWithMouseClick)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        Vector2 myPos = transform.position;

        Vector2 dir = mousePos - myPos;  

        if (dir.x < 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    // Reset Jump Counts When Collide With The Ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D ray;
        ray = Physics2D.Raycast(col.bounds.center, Vector2.down, col.bounds.extents.y + 0.2f, jumpableLayer);

        if (ray.collider != null)
        {
            currentJumps = 0;
            //StartCoroutine(SquashStretch());
        }
    }

    IEnumerator SquashStretch()
    {
        Vector3 squashedScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);

        transform.localScale = squashedScale;

        yield return new WaitForSeconds(0.2f);

        transform.localScale = originalScale;
    }



}

