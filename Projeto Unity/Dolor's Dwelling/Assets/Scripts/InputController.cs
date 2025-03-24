using UnityEngine;

public abstract class InputController : ScriptableObject
{
    public abstract float MoveInputX();
    public abstract float MoveInputY();

    public abstract bool JumpInput();
    public abstract bool JumpHold();
    public abstract bool MouseLeftClick();
}
