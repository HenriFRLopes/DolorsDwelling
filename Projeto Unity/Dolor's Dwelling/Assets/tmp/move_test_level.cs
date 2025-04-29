using UnityEngine;

public class move_test_level : MonoBehaviour
{

    public float speed;
    public float jump;

    private Vector2 dir;
    private Rigidbody2D rig;

    void Start()
    {
        dir = new Vector2(0, 0);
        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move();
        transform.Translate(dir * Time.deltaTime);
    }

    private void move(){
        dir = new Vector2(0, 0);
        if(Input.GetKey("a")){
            dir = new Vector2(-1, 0);
        }if(Input.GetKey("d")){
            dir = new Vector2(1, 0);
        }

        if(Input.GetKeyDown("space") && !Input.GetKey("s")){
            rig.AddForceY(jump);
        }
        dir = dir.normalized;
        dir = new Vector2(dir.x * speed, 0);
    }
}
