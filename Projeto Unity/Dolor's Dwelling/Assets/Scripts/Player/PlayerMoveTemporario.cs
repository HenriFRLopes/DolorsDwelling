using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveTemporario : MonoBehaviour
{
    public InputController input = null;

    [Header("Estados do Movimento")]
    public bool canMove;
    public  bool grounded = false;
    public bool walled = false;
    public bool canDoubleJump = false;
    bool canDash = true;
    bool leftGround;
    public bool pulando;
    bool gliding;
    public bool doubleJumping;
    public bool canJump;
    public bool onWall;
    public bool slowed;

    Vector2 velocity, direction, desiredVelocity;
    Rigidbody2D rb;
    float maxSpeedChange, acceleration;
    float friction;
    float tempoExtraTimer;
    public bool wantToJump;
    public float bufferTimer;
    Vector2 contactDir;


    [Header("Valores publicos de movimentacao")]


    [Header("Pulo e Pulo Duplo")]
    public float jumpStrength;
    public float jumpHeight;
    public float regularGravity;
    public float currentGravity;
    public float doubleJumpStrength;
    public float maxJumpVelocity;
    public float tempoExtraPulo = 0.2f;
    public float buffer;

    [Header("Movimentacao Horizontal")]
    public float maxAirAcceleration;
    public float maxSpeed;
    public float maxAcceleration;

    [Header("Paredes!")]
    public float maxWallDragSpeed;
    bool wallJumping;
    public Vector2 wallJumpStrength = new Vector2(10.7f, 10f);
    float wallDirX;


    [Header("Glide")]
    public float glideFallSpeed;

    [Header("Dash")]
    public float dashRecoveryTimer;
    public float dashDistance;
    [Range(0.1f, 1f)]
    public float invencibilityDuration;
    [Range(0.1f, 1f)]
    public float dashDuration;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGravity = regularGravity;
        canMove = true;
    }

    private void Update()
    {
        if(!canMove)
        {
            return;
        }
        if(input.DashInput())
        {
            Dash();
        }
        direction.x = input.MoveInputX();
        desiredVelocity = new Vector2(direction.x, 0) * Mathf.Max(maxSpeed - friction, 0);
        gliding |= input.GliderInput();
        wantToJump |= input.JumpInput();

        if (input.JumpHold() && rb.linearVelocity.y > 0 && !doubleJumping && canJump)
        {
            rb.gravityScale = jumpStrength;
            leftGround = true;
            if (rb.linearVelocity.y > maxJumpVelocity)
            {
                velocity.y = maxJumpVelocity - 2;
                canJump = false;
            }
        }

        else if (!input.JumpHold() || rb.linearVelocity.y < 0 && !grounded && !gliding && !walled)
        {
            canJump = false;
            rb.gravityScale = currentGravity;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
            return;
        velocity = rb.linearVelocity;
        if (wantToJump)
        {
            wantToJump = false;
            bufferTimer = buffer;
        }

        else if (!wantToJump && bufferTimer > 0)
        {
            bufferTimer -= Time.deltaTime;
        }

        if (bufferTimer > 0)
        {
            if(onWall)
            {
                WallJump();
            }
            else if(!onWall)
            {
                Jump();
            }
        }

        acceleration = grounded ? maxAcceleration : maxAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        if(slowed)
        {
            velocity.x /= 2;
        }

        if(input.GliderInput() && rb.linearVelocity.y < 0)
        {
            rb.gravityScale = glideFallSpeed;
        }

        else if (rb.linearVelocity.y == 0)
        {
            rb.gravityScale = regularGravity;
        }


        if (grounded && rb.linearVelocity.y == 0)
        {
            canJump = true;
            tempoExtraTimer = tempoExtraPulo;
            pulando = false;
            onWall = false;
        }

        else
        {
            tempoExtraTimer -= Time.deltaTime;
        }

        #region Wall Movement

        if(walled && input.MoveInputX() == - contactDir.x && !grounded)
        {
            onWall = true;
        }

        else if(walled && input.MoveInputX() == contactDir.x || grounded)
        {
            onWall = false;
        }
        if(onWall && walled)
        {
            if (velocity.y < -maxWallDragSpeed)
            {
                velocity.y = -maxWallDragSpeed;
            }
            wallDirX = contactDir.x;
            wallJumping = false;
            canDash = true;
            if(wallDirX != 0)
            {
                Player.instance.Flip(wallDirX);
            }
        }

        if (grounded)
        {
            wallJumping = false;
        }

        #endregion
        if(velocity.x != 0 && grounded)
        {
            Player.instance.anim.SetBool("Running", true);
        }
        else
        {
            Player.instance.anim.SetBool("Running", false);
        }

        rb.linearVelocity = velocity;

    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision);
        CheckFriction(collision);
        if (collision.gameObject.CompareTag("Ground"))
        {
            leftGround = false;
            RecoverDash();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckCollision(collision);
        CheckFriction(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        friction = 0;

        Invoke("LeaveGround", tempoExtraPulo);
        walled = false;
        canDoubleJump = true;
        Player.instance.walled = false;
    }



    void CheckCollision(Collision2D col)
    {
        for (int i = 0; i < col.contactCount; i++)
        {

            contactDir = col.GetContact(i).normal;
            grounded |= contactDir.y >= 0.9f;
            walled |= Mathf.Abs(contactDir.x) >= 0.9f;
        }
        if (grounded)
        {
            Player.instance.grounded = true;
            doubleJumping = false;
            canDoubleJump = true;
            leftGround = false;
            onWall = false;
        }
        else if(walled)
        {
            if(wallJumping)
            {
                onWall = true;
                wallJumping = false;
            }
            Player.instance.walled = true;
            pulando = false;
            doubleJumping = false;
        }
    }

    void CheckFriction(Collision2D col)
    {
        PhysicsMaterial2D mat = col.rigidbody.sharedMaterial;

        friction = 0;
        if (mat != null)
        {
            friction = mat.friction;
        }
    }


    void Jump()
    {
        if (!doubleJumping)
        {
            if (tempoExtraTimer > 0 && canJump)
            {
                leftGround = true;
                float jumpForce = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
                jumpForce = Mathf.Max(jumpForce, velocity.y, 0);
                velocity.y += jumpForce;
                bufferTimer = 0;
                Debug.Log("Pulei");
                pulando = true;
                grounded = false;
                Player.instance.grounded = false;
                Player.instance.CancelShield();
            }
            else if (leftGround && !onWall && canDoubleJump || !grounded && !onWall && canDoubleJump)
            {
                canJump = false;
                rb.gravityScale = 0;
                doubleJumping = true;
                float jumpForce = Mathf.Sqrt(2f * doubleJumpStrength);
                if(rb.linearVelocityY < 0)
                {
                     jumpForce -= rb.linearVelocityY;
                    if(gliding)
                    {
                        jumpForce += glideFallSpeed;
                    }
                }
                rb.linearVelocityY = 0;
                velocity.y += jumpForce;
                bufferTimer = 0;
                rb.gravityScale = regularGravity;
                canDoubleJump = false;
            }
        }
    }

    void Dash()
    {
        if(canDash)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            rb.freezeRotation = true;
            canMove = false;
            canDash = false;
            this.gameObject.layer = LayerMask.NameToLayer("Invencivel");
            Invoke("EndIFrames", invencibilityDuration);
            Invoke("EndDash", dashDuration);
            Player.instance.CancelShield();
            rb.linearVelocityX += dashDistance * transform.localScale.x;
        }
    }

    void EndDash()
    {
        rb.linearVelocityX = 0;
        canMove = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        Invoke("RecoverDash", dashRecoveryTimer);
    }

    void EndIFrames()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    void RecoverDash()
    {
        canDash = true;
    }

    void WallJump()
    {
        bufferTimer = 0;
        velocity = new Vector2(wallJumpStrength.x * wallDirX, wallJumpStrength.y);
        onWall = false;
        wallJumping = true;
        canDoubleJump = true;
    }


    void LeaveGround()
    {
        grounded = false;
        Debug.Log("saidoChao");
        Player.instance.grounded = false;
    }
}
