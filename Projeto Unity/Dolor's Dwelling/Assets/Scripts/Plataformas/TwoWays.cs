using UnityEngine;

public class TwoWays : MonoBehaviour
{

    private GameObject player;
    private BoxCollider2D self_collision;
    private bool player_is_on_top;
    private float cooldown = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        self_collision = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        TwoWaysBehavior();
    }

    void TwoWaysBehavior(){
        //Alterar para o sistema dos botões oficiais, por enquanto só teste
        if(Input.GetKey("s") && Input.GetKeyDown("space") && self_collision.enabled && player_is_on_top){
            cooldown = 0.2f; //ajustar isso aqui melhor em testes
            return;
        }

        if(transform.position.y + transform.localScale.y/2 <= player.transform.position.y - player.transform.localScale.y/2 && cooldown <= 0f){
            self_collision.enabled = true;
        }else if(transform.position.y - transform.localScale.y/2 >= player.transform.position.y + player.transform.localScale.y/2 && cooldown <= 0f){
            self_collision.enabled = false;
        }else if(cooldown > 0){
            self_collision.enabled = false;
        }

        cooldown -= Time.deltaTime;
        if(cooldown < 0f) cooldown = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision){ 
        if (collision.gameObject.CompareTag("Player")){ 
            player_is_on_top = true;
        } 
    }

    void OnCollisionExit2D(Collision2D collision){ 
        if (collision.gameObject.CompareTag("Player")){ 
            player_is_on_top = false;
        } 
    }
}
