using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMoveBasics : MonoBehaviour
{
    public Image tmp_dash_time;
    public float speed;
    public float speed_on_air;
    public float inertial_force;
    public float break_force;
    public float dash_force;
    public float dash_sustain;
    public float cooldown_dash;
    public float jump_force;
    public float jump_sustain;
    public float gravity_scale;
    public float max_velocity_falling;
    public float coyote_time;
    public float force_on_wall;
    public float impulse_on_wall;
    public float max_velocity_falling_on_wall;
    public Vector2 size_box_down;
    public float box_down_distance;
    public LayerMask ground_layer;
    public Vector2 size_box_left;
    public float box_left_distance;
    public Vector2 size_box_right;
    public float box_right_distance;

    private Rigidbody2D rb;
    private int dir;
    private int looking_at;
    private bool is_falling;
    private bool is_jumping;
    private bool buffer_jump;
    private bool on_air;
    private bool on_ground;
    private bool can_jump;
    private float cooldown_coyote_behavior;
    private bool coyote_behavior;
    private float time_jump;
    private bool jump_key_press;
    private int count_jump;
    private float cooldown_dash_time;
    private bool is_dashing;
    private float time_dash;
    private bool on_wall;
    private bool obj_on_left;
    private bool obj_on_right;
    private bool was_on_wall;
    private int dir_jump;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        looking_at = 1;
        count_jump = 0;
    }

    void FixedUpdate(){
        OnGround();
        OnWall();
    }

    void Update(){
        tmp_dash_time.fillAmount = cooldown_dash_time/cooldown_dash;
        OnAir();

        walk();
        jump();
        dash();
        gravity();
    }

    void gravity(){
        if(!on_wall){
            rb.linearVelocityY -= gravity_scale * Time.deltaTime;
            if(rb.linearVelocityY < -max_velocity_falling){
                rb.linearVelocityY = -max_velocity_falling;
            }
        }else{
            if(rb.linearVelocityY > 0) rb.linearVelocityY = 0;
            rb.linearVelocityY -= gravity_scale/force_on_wall * Time.deltaTime;
            if(rb.linearVelocityY < -max_velocity_falling_on_wall){
                rb.linearVelocityY = -max_velocity_falling_on_wall;
            }
        }

        if(rb.linearVelocityY < 0.2f && !on_ground) is_falling = true;
        else is_falling = false;
    }

    void walk(){
        //mudar depois para os triggers adequados
        if(Input.GetKey("a") && !is_dashing && !obj_on_left && !was_on_wall){
            dir = -1;
            looking_at = dir;
        }else if(Input.GetKey("d") && !is_dashing && !obj_on_right && !was_on_wall){
            dir = 1;
            looking_at = dir;
        }else dir = 0;

        if(Mathf.Abs(rb.linearVelocityX) < speed && Mathf.Abs(dir) == 1){
            rb.linearVelocityX += dir * speed * inertial_force * Time.deltaTime;
        }
        if(Mathf.Abs(rb.linearVelocityX) > speed && Mathf.Abs(dir) == 1 && on_ground){
            rb.linearVelocityX = dir * speed;
        }

        if(on_air && Mathf.Abs(dir) == 1 && !was_on_wall){
            rb.linearVelocityX = dir * speed_on_air;
        }

        if(on_wall && !on_ground) rb.linearVelocityX = 0;

        if(rb.linearVelocityX > 0f && dir == 0){
            rb.linearVelocityX -= break_force * Time.deltaTime;
        }
        if(rb.linearVelocityX < 0f && dir == 0){
            rb.linearVelocityX += break_force * Time.deltaTime;
        }
    }

    void jump(){
        if(is_falling) is_jumping = false;

        if(on_ground) cooldown_coyote_behavior = 0f;
        if(on_wall) cooldown_coyote_behavior = coyote_time;
        if(is_falling && cooldown_coyote_behavior < coyote_time){
            coyote_behavior = true;
            cooldown_coyote_behavior += Time.deltaTime;
        }else coyote_behavior = false;

        if(on_air && !coyote_behavior && count_jump > 1) can_jump = false; //verificar tbm a quantidade de pulos pro double jump;
        if(on_ground || on_wall) can_jump = true; //verificar tbm se a personagem está em outras superficies que resetam os pulos;

        if(on_ground && !is_jumping) count_jump = 0;
        if(on_wall) count_jump = 1; //aqui fica 1 porque quando pula da parede existe um tempo em que você ainda está na parede mesmo já tendo pulado, e isso acaba subindo a quantidade de pulos para 3.

        Debug.Log(count_jump);

        if(Input.GetKeyDown(KeyCode.Space) && is_falling && count_jump > 1){
            buffer_jump = true;
        }

        if((buffer_jump || Input.GetKeyDown(KeyCode.Space)) && can_jump && !Input.GetKey("s")){ //temporario esse getkey("s")
            count_jump++;
            is_jumping = true;
            buffer_jump = false;
            time_jump = 0;
            jump_key_press = true;
            is_dashing = false;
            if(on_wall){
                rb.linearVelocityX = 0; //mudei aqui, só pra lembrar
                was_on_wall = true;
                dir_jump = dir;
            }else was_on_wall = false;
        }
        if(Input.GetKey(KeyCode.Space) && jump_key_press){
            if(time_jump < jump_sustain){
                if(was_on_wall){
                    if(dir_jump == -1 && !obj_on_left){
                        rb.linearVelocityY = jump_force - Mathf.Pow(Mathf.Lerp(0, jump_force, time_jump),2) * Time.deltaTime;
                        rb.linearVelocityX = -impulse_on_wall - Mathf.Pow(Mathf.Lerp(0, impulse_on_wall, time_jump),2) * Time.deltaTime;
                    }
                    if(dir_jump == 1 && !obj_on_right){
                        rb.linearVelocityY = jump_force - Mathf.Pow(Mathf.Lerp(0, jump_force, time_jump),2) * Time.deltaTime;
                        rb.linearVelocityX = impulse_on_wall - Mathf.Pow(Mathf.Lerp(0, impulse_on_wall, time_jump),2) * Time.deltaTime;
                    }
                }else{
                    rb.linearVelocityY = jump_force - Mathf.Pow(Mathf.Lerp(0, jump_force, time_jump),2) * Time.deltaTime;
                }
            }else{
                was_on_wall = false;
            }
            time_jump += Time.deltaTime;
        }else{
            jump_key_press = false;
            was_on_wall = false;
        }
    }

    void dash(){
        if((on_ground || on_wall) && cooldown_dash_time < cooldown_dash) cooldown_dash_time += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.LeftShift) && cooldown_dash_time >= cooldown_dash){
            cooldown_dash_time = 0;
            time_dash = 0;
            is_dashing = true;
        }
        if(is_dashing){
            if(time_dash <= dash_sustain){
                rb.linearVelocityY = 0;
                rb.linearVelocityX = looking_at * dash_force;
            }else{
                rb.linearVelocityX = 0;
                is_dashing = false;
            }
            time_dash += Time.deltaTime;
        }
    }

    void OnAir(){
        on_air = (is_falling || is_jumping) ? true : false;
    }

    void OnGround(){
        if(Physics2D.BoxCast(transform.position + Vector3.down*box_down_distance, size_box_down, 0, Vector2.down, 0, ground_layer)){
            on_ground = true;
        }else{
            on_ground = false;
        }
    }

    void OnWall(){
        on_wall = false;

        RaycastHit2D hitleft = Physics2D.BoxCast(transform.position + Vector3.left*box_left_distance, size_box_left, 0, Vector2.left, 0);
        obj_on_left = hitleft;
        if(hitleft) if(hitleft.collider.gameObject.tag == "Wall") on_wall = true;

        RaycastHit2D hitright = Physics2D.BoxCast(transform.position + Vector3.right*box_right_distance, size_box_right, 0, Vector2.right, 0);
        obj_on_right = hitright;
        if(hitright) if(hitright.collider.gameObject.tag == "Wall") on_wall = true;
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position + Vector3.down * box_down_distance, size_box_down);
        Gizmos.DrawWireCube(transform.position + Vector3.left * box_left_distance, size_box_left);
        Gizmos.DrawWireCube(transform.position + Vector3.right * box_right_distance, size_box_right);
    }
}