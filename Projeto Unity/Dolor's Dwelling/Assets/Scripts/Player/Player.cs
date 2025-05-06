using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance;
    public InputController input;
    public Pena[] penasEquipadas;
    public PlayerMoveTemporario movement;
    public bool grounded, walled;
    public bool canBePushed;
    public float parryTimer;
    public GameObject shield;
    int shieldHits = 0;
    bool canBlock = true;
    bool shieldInput;
    public float blockRecoveryTimer;
    Rigidbody2D rb;
    public Animator anim;
    public float shieldHealTime;

    public enum BlockState { Blocking, Parrying, RecoveringBlock, None}
    public BlockState state;
    Pena penaAtual;
    int vida = 100;

    public int papeis;

    public GameObject tiro;

    void Awake()
    {
        penaAtual = penasEquipadas[0];

        if(instance!= null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if(input.MouseLeftClick() && penaAtual.canAttack)
        {
            if (input.MoveInputY() <= 0 && grounded || input.MoveInputY() == 0)
            {
                penaAtual.currentAttackArea = penaAtual.sideArea;
                penaAtual.currentAttackDir = penaAtual.attackSide;
                if(penaAtual.comboCount == 0)
                {
                    //Coisa de Animação
                    Invoke("ReturnMovement", penaAtual.delayAttack1);
                    penaAtual.Invoke("Attack", penaAtual.delayAttack1);
                }
                else
                {
                    if(penaAtual.comboCount == 1)
                    {
                        //Coisa de Animação
                        Invoke("ReturnMovement", penaAtual.delayAttack2);
                    }
                    else
                    {
                        //Coisa de Animação

                        movement.canMove = false;
                        Invoke("ReturnMovement", penaAtual.delayAttack3);
                    }
                    penaAtual.Attack();
                }
            }
            else if (input.MoveInputY() < 0 && !grounded)
            {
                penaAtual.currentAttackArea = penaAtual.downArea;
                penaAtual.currentAttackDir = penaAtual.attackDown;
                movement.canMove = false;
                Invoke("ReturnMovement", penaAtual.delayAttackDown);
                penaAtual.Invoke("Attack", penaAtual.delayAttackDown);
            }
            else if (input.MoveInputY() > 0)
            {
                penaAtual.currentAttackArea = penaAtual.upArea;
                penaAtual.currentAttackDir = penaAtual.attackUp;
                movement.canMove = false;
                Invoke("ReturnMovement", penaAtual.delayAttackUp);
                penaAtual.Invoke("Attack", penaAtual.delayAttackUp);
            }
        }

        if(input.ShieldInput())
        {
            BlockInput();
        }
        else if(!input.ShieldInput())
        {
            if(state == BlockState.Blocking)
            {
                CancelShield();
            }
        }

        if (input.MoveInputX() < 0)
        {
            Flip(-1);
        }

        if (input.MoveInputX() > 0)
        {
            Flip(1);
        }

        if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Flip(float i)
    {

        transform.localScale = new Vector2(i, transform.localScale.y);
 
    }

    public void PerderVida()
    {
        if (vida > 20)
        {
            vida -= 10;
        }
        if(vida <= 20)
        {
            vida -= 5;
        }
        Debug.Log(vida);
    }
    public void GanharVida(int valor)
    {
        vida += valor;
    }

    public void Morrer()
    {

    }

    void BlockInput()
    {
        if (state == BlockState.None && grounded)
        {
            if (canBlock)
            {
                Parry();
                Debug.Log(state);
                movement.slowed = true;
                penaAtual.canAttack = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void Parry()
    {
        if (state != BlockState.None)
        {
            return;
        }
        movement.slowed = true;
        state = BlockState.Parrying;
        shield.GetComponent<SpriteRenderer>().color = Color.blue;
        shield.SetActive(true);
        Invoke("EndParry", parryTimer);
    }

    void EndParry()
    {
        if (input.ShieldInput())
        {
            StartShield();
        }
        else
        {
            CancelShield();
        }
    }

    void StartShield()
    {
        shield.GetComponent<SpriteRenderer>().color = Color.green;
        state = BlockState.Blocking;
        penaAtual.canAttack = false;
    }

    public void ShieldDamage()
    {
        if (shieldHits == 0)
        {
            shieldHits++;
            shield.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (shieldHits == 1)
        {
            EndShield();
        }
    }
    void EndShield()
    {
        canBlock = false;
        penaAtual.canAttack = false;
        shield.SetActive(false);
        state = BlockState.RecoveringBlock;
        Invoke("RecoverBlock", blockRecoveryTimer);
        Invoke("ReturnMovement", blockRecoveryTimer / 2);
        Invoke("HealShield", shieldHealTime);
    }

    public void CancelShield()
    {
        shield.SetActive(false);
        penaAtual.canAttack = true;
        shieldHits = 0;
        state = BlockState.None;
        ReturnMovement();
        Invoke("HealShield", shieldHealTime);
    }

    void RecoverBlock()
    {
        canBlock = true;
        state = BlockState.None;
        penaAtual.canAttack = true;
    }

    void ReturnMovement()
    {
        movement.canMove = true;
        movement.slowed = false;

    }

    void HealShield()
    {
        shieldHits = 0;
    }
}
