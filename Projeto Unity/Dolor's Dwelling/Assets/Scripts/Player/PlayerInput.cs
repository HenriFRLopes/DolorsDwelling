using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "Scriptable Objects/PlayerInput")]
public class PlayerInput : InputController
{
    public override bool JumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public override float MoveInputX()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public override float MoveInputY()
    {
        return Input.GetAxisRaw("Vertical");
    }
    public override bool JumpHold()
    {
        return Input.GetButton("Jump");
    }

    public override bool MouseLeftClick()
    {
        return Input.GetMouseButtonDown(0);
    }

    public override bool ShieldInput()
    {
        return Input.GetKey(KeyCode.E);
    }

    public override bool DashInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
}
