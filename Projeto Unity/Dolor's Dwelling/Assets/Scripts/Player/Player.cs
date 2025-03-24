using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public InputController input;
    public Pena[] penasEquipadas;
    public bool grounded;
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
    }

    void Update()
    {
        CheckInput();
        Flip();
    }

    void CheckInput()
    {
        if(input.MouseLeftClick())
        {
            if (input.MoveInputY() <= 0 && grounded || input.MoveInputY() == 0)
            {
                penaAtual.currentAttackArea = penaAtual.sideArea;
                penaAtual.currentAttackDir = penaAtual.attackSide;
                if(penaAtual.comboCount == 0)
                {
                    penaAtual.Invoke("Attack", penaAtual.delayAttack1);
                }
                else
                {
                    penaAtual.Attack();
                }
            }
            else if (input.MoveInputY() < 0 && !grounded)
            {
                penaAtual.currentAttackArea = penaAtual.downArea;
                penaAtual.currentAttackDir = penaAtual.attackDown;
                penaAtual.Invoke("Attack", penaAtual.delayAttackDown);
            }
            else if (input.MoveInputY() > 0)
            {
                penaAtual.currentAttackArea = penaAtual.upArea;
                penaAtual.currentAttackDir = penaAtual.attackUp;
                penaAtual.Invoke("Attack", penaAtual.delayAttackUp);
            }
        }
    }

    public void Flip()
    {
        if(input.MoveInputX() < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if(input.MoveInputX() > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);

        }
    }

    public void PerderVida()
    {
        if(vida > 20)
        {
            vida -= 10;
        }
        if(vida < 20)
        {
            vida -= 5;
        }
    }
    public void GanharVida(int valor)
    {
        vida += valor;
    }

    public void Morrer()
    {

    }
}
