using UnityEngine;

public abstract class Pena : MonoBehaviour
{
    bool attacking, buffered;
    public bool canAttack = true;
    public int damage;
    public float delayAttack1, delayAttack2, delayAttack3, delayAttackUp, delayAttackDown;
    public int comboCount;
    public float comboTimer, recovery;
    public float pushForce;
    public Transform attackSide, attackUp, attackDown;
    public Vector2 sideArea, upArea, downArea;
    public Transform currentAttackDir;
    public Vector2 currentAttackArea;
    public LayerMask attackableLayer;
    public GameObject hitBoxDirectionVizualizer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackSide.position, sideArea);
        Gizmos.DrawWireCube(attackUp.position, upArea);
        Gizmos.DrawWireCube(attackDown.position, downArea);
    }

    private void ResetAttack()
    {
        attacking = false;
        buffered = false;
        canAttack = true;
    }

    public void ResetCombo()
    {
        comboCount = 0;
        canAttack = false;
        Invoke("ResetAttack", recovery);
    }

    public void Attack()
    {
        if (!canAttack)
        {
            return;
        }
        if (attacking && !buffered)
        {
            if(comboCount == 1)
            {
                Debug.Log("Ataque2");
                buffered = true;
                Invoke("Attack", delayAttack2);
                CancelInvoke("ResetCombo");
            }
            else if(comboCount == 2)
            {
                Debug.Log("Ataque3");
                buffered = true;
                Invoke("Attack", delayAttack3);
                CancelInvoke("ResetCombo");
            }
            return;
        }
        comboCount++;
        attacking = true;
        Collider2D[] colider = Physics2D.OverlapBoxAll(currentAttackDir.position, currentAttackArea, 0, attackableLayer);
        GameObject hahahaha = Instantiate(hitBoxDirectionVizualizer, currentAttackDir.position, Quaternion.identity);
        Invoke("ResetCombo", comboTimer);
        foreach (Collider2D col in colider)
        {
            if (col.gameObject.GetComponent<Inimigo>() != null && col.gameObject.GetComponent<Inimigo>().staggerable)
            {
               
                col.gameObject.GetComponent<Inimigo>().rb.AddForce(transform.right * pushForce, ForceMode2D.Impulse);
            }

        }
        if(currentAttackDir == attackDown || currentAttackDir == attackUp)
        {
            comboCount = 0;
        }
        ExtraEffect();
        buffered = false;
    }

    public abstract void ExtraEffect();
}
