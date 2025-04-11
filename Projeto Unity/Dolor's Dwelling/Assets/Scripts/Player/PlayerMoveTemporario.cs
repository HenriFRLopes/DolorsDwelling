using UnityEngine;

public class PlayerMoveTemporario : MonoBehaviour
{
    public InputController input = null;
    public bool canMove;
    public  bool grounded = false;
    bool canDoubleJump = false;
    bool canDash = true;
    Vector2 velocity, direction, desiredVelocity;
    Rigidbody2D rb;
    float maxSpeedChange, acceleration;
    float friction;
    public bool wantToJump;
    float tempoExtraTimer;
    bool pulando;
    public float bufferTimer;


    [Header("Valores publicos de movimentacao")]


    [Header("Pulo e Pulo Duplo")]
    public float jumpStrength;
    public float jumpHeight;
    public float regularGravity;
    public float currentGravity;
    public float doubleJumpStrength;
    public float maxJumpVelocity;
    public float tempoExtraPulo = 0.2f; //Coyote time
    public float buffer;

    [Header("Movimentacao Horizontal")]
    public float maxAirAcceleration;
    public float maxSpeed;
    public float maxAcceleration;

    [Header("Paredes!")]
    public float wallDrag; // Nao Feito Ainda

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
        wantToJump |= input.JumpInput();
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
            Jump();
        }

        acceleration = grounded ? maxAcceleration : maxAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        if (input.JumpHold() && rb.linearVelocity.y > 0)
        {
            rb.gravityScale = jumpStrength;

            if (rb.linearVelocity.y > maxJumpVelocity)
            {
                velocity.y = maxJumpVelocity - 2;
            }
        }

        else if(input.JumpHold() && rb.linearVelocity.y < 0)
        {
            rb.gravityScale = glideFallSpeed;
        }

        else if (!input.JumpHold() || rb.linearVelocity.y < 0 && !grounded)
        {
            rb.gravityScale = currentGravity;
        }

        else if (rb.linearVelocity.y == 0)
        {
            rb.gravityScale = regularGravity;
        }


        if (grounded && rb.linearVelocity.y == 0)
        {
            tempoExtraTimer = tempoExtraPulo;
            pulando = false;
        }

        else
        {
            tempoExtraTimer -= Time.deltaTime;
        }

        rb.linearVelocity = velocity;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);
        CheckFriction(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGround(collision);
        CheckFriction(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        friction = 0;
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
            Player.instance.grounded = false;
        }
    }



    void CheckGround(Collision2D col)
    {
        for (int i = 0; i < col.contactCount; i++)
        {
            if (col.gameObject.CompareTag("Ground"))
            {
                grounded = true;
                Player.instance.grounded = true;
                canDoubleJump = true;
                return;
            }
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
        if (tempoExtraTimer > 0)
        {
            float jumpForce = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
            jumpForce = Mathf.Max(jumpForce, velocity.y, 0);
            velocity.y += jumpForce;
            bufferTimer = 0;
            pulando = true;
            grounded = false;
            Player.instance.grounded = false;
        }
        else if (pulando && canDoubleJump)
        {
            float jumpForce = Mathf.Sqrt(-2f * Physics2D.gravity.y * doubleJumpStrength);
            jumpForce = Mathf.Max(jumpForce, velocity.y, 0);
            velocity.y += jumpForce;
            bufferTimer = 0;
            canDoubleJump = false;
        }
    }

    void Dash()
    {
        if(canDash)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            canMove = false;
            canDash = false;
            this.gameObject.layer = LayerMask.NameToLayer("Invencivel");
            Invoke("EndIFrames", invencibilityDuration);
            Invoke("EndDash", dashDuration);

            rb.linearVelocityX += dashDistance * transform.localScale.x;
        }
    }

    void EndDash()
    {
        canMove = true;
        rb.constraints = RigidbodyConstraints2D.None;
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

}
