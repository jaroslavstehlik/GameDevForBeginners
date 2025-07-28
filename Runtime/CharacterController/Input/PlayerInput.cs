using UnityEngine;

public struct ActionTimeStamp
{
    public int frameIndex;
    public float time;

    public ActionTimeStamp(int frameIndex, float time)
    {
        this.frameIndex = frameIndex;
        this.time = time;
    }
    
    public static ActionTimeStamp Empty()
    {
        return new ActionTimeStamp(0, 0);
    }

    public static ActionTimeStamp Now()
    {
        return new ActionTimeStamp(Time.frameCount, Time.unscaledTime);
    }

    public void Reset()
    {
        this.frameIndex = 0;
        this.time = 0;
    }

    public bool isEmpty()
    {
        return this.frameIndex == 0 && this.time == 0;
    }
}

public class ButtonInput
{
    private ActionTimeStamp _pressed;
    private ActionTimeStamp _released;
    private bool _beingPressed;

    public void Update(bool pressed)
    {
        if (_beingPressed == pressed)
        {
            return;
        }

        if (_beingPressed && !pressed)
        {
            Release();
            return;
        }
        
        Press();
    }
    
    void Press()
    {
        _pressed = ActionTimeStamp.Now();
        _beingPressed = true;
    }

    void Release()
    {
        _released = ActionTimeStamp.Now();
        _beingPressed = false;
    }

    public float pressedDuration => _released.time - _pressed.time;
    public int pressedFrames => _released.frameIndex - _pressed.frameIndex;
    public bool isPressed
    {
        get
        {
            if (_pressed.frameIndex != 0 && _released.frameIndex != 0 &&
                _pressed.frameIndex == _released.frameIndex)
                return true;
            
            return _beingPressed;
        }
    }

    public void Reset()
    {
        _beingPressed = false;
        _pressed.Reset();
        _released.Reset();
    }

    public static ButtonInput Empty()
    {
        return new ButtonInput()
        {
            _beingPressed = false,
            _pressed = ActionTimeStamp.Empty(),
            _released = ActionTimeStamp.Empty()
        };
    }

    public bool isEmpty()
    {
        return _pressed.isEmpty() && _released.isEmpty() && !_beingPressed;
    }
}

public class PlayerInput
{
    public Vector2 move;
    public Vector2 look;
    public ButtonInput jump = ButtonInput.Empty();
    public ButtonInput crouch = ButtonInput.Empty();
    public ButtonInput sprint = ButtonInput.Empty();
    public ButtonInput interact = ButtonInput.Empty();

    public void ResetMovement()
    {
        move = Vector2.zero;
    }
}