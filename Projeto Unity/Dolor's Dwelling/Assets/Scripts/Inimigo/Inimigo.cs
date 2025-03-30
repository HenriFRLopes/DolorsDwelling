using UnityEngine;
using UnityEngine.AI;

public class Inimigo : MonoBehaviour
{
    public bool staggerable = true, stunado;
    public int vida;
    public bool recovering;
    public float stunTime;
    public Rigidbody2D rb;
    public Transform attackDir;
    public GameObject hitBoxDirectionVizualizer;
    public Vector2 attackArea;
    public bool canAttack = false;
    public float attackPause;
    public float attackBuildUp;
    public float pushForce;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Invoke("Attack", attackBuildUp);
    }
    public void TomarDano(int valor)
    {
        vida -= valor;
        Debug.Log("ui");
    }


    public void Recovery()
    {
        recovering = false;
        stunado = false;
    }

    public void Stun()
    {
        Invoke("Recovery", stunTime);
        recovering = true;
        stunado = true;
    }

    void Attack()
    {
        if (!canAttack)
        {
            return;
        }

        Collider2D[] colider = Physics2D.OverlapBoxAll(attackDir.position, attackArea, 0);
        GameObject hahahaha = Instantiate(hitBoxDirectionVizualizer, attackDir.position, Quaternion.identity);
        Destroy(hahahaha, 1f);
        foreach (Collider2D col in colider)
        {
            if (col.gameObject.GetComponent<Player>() != null)
            {
                Player p1 = col.gameObject.GetComponent<Player>();
                if (p1.state == Player.BlockState.Parrying)
                {
                    ParryReaction();
                }

                else if (p1.state == Player.BlockState.Blocking)
                {
                    p1.ShieldDamage();
                }
                else
                {
                    col.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.forward * pushForce, ForceMode2D.Impulse);
                    p1.PerderVida();
                }
            }
            
        }
        ResetAttack();
    }
    void ResetAttack()
    {
        canAttack = true;
        Invoke("Attack", attackBuildUp);
    }

    public void ParryReaction()
    {
        Destroy(this.gameObject);  
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackDir.position, attackArea);
    }
}
