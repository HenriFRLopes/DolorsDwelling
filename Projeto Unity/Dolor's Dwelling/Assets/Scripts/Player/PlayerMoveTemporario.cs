using UnityEngine;

public class PlayerMoveTemporario : MonoBehaviour
{
    public InputController input = null;
    bool grounded = false;
    bool canDoubleJump = false;
    Vector2 velocity, direction, desiredVelocity;
    Rigidbody2D rb;
    float maxSpeedChange, acceleration;
    float friction;
    bool wantToJump;
    float tempoExtraTimer;
    bool pulando;
    float bufferTimer;


    [Header("Valores publicos de movimenta��o")]

    public float jumpStrength;
    public float jumpHeight;
    public float regularGravity;
    public float currentGravity;
    public float doubleJumpStrength;
    public float maxJumpVelocity;
    public float tempoExtraPulo = 0.2f; //Tipo Looney Tunes
    public float buffer;


    public float maxAirAcceleration;
    public float maxSpeed;
    public float maxAcceleration;


    public float wallDrag; // N�o Feito Ainda

    [Header("Camadas")]
    //Para IFRAMES\\ N�o Implementado
    public LayerMask layerNormal;
    public LayerMask invencibilidade;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGravity = regularGravity;
    }

    private void Update()
    {
        direction.x = input.MoveInputX();
        desiredVelocity = new Vector2(direction.x, 0) * Mathf.Max(maxSpeed - friction, 0);
        wantToJump |= input.JumpInput();
    }

    private void FixedUpdate()
    {
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

        else if (!input.JumpHold() || rb.linearVelocity.y < 0)
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

}
