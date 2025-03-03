using UnityEngine;

public struct PlayerInput
{
    public Vector2 move;
    public bool jump;
    public bool crouch;
    public bool sprint;

    public void ResetJump()
    {
        jump = false;
    }
        
    public void ResetMovement()
    {
        move = Vector2.zero;
    }

    public static PlayerInput Empty
    {
        get { return new PlayerInput() { move = Vector2.zero, jump = false, sprint = false }; }
    }

    public bool isEmpty
    {
        get { return move == Vector2.zero && jump == false && sprint == false; }
    }
}