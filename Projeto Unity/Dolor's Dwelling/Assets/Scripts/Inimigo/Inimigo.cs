using UnityEngine;
using UnityEngine.AI;

public class Inimigo : MonoBehaviour
{
    public bool staggerable = true, stunado;
    public int vida;
    public bool recovering;
    public float stunTime;
    public Rigidbody2D rb;
    public NavMeshAgent agent;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
