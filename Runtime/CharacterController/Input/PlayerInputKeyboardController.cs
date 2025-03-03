using UnityEngine;

public class PlayerInputKeyboardController : PlayerInputController
{
    private void Update()
    {
        PlayerInput playerInput = new PlayerInput();
            
        if (Input.GetKey(KeyCode.W))
        {
            playerInput.move.y = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            playerInput.move.y = -1f;
        }
        else
        {
            playerInput.move.y = 0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            playerInput.move.x = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerInput.move.x = 1f;
        }
        else
        {
            playerInput.move.x = 0f;
        }

        playerInput.jump = Input.GetKeyDown(KeyCode.Space);
        playerInput.sprint = Input.GetKey(KeyCode.LeftShift);
        playerInput.crouch = Input.GetKey(KeyCode.LeftControl);

        UpdateInput(playerInput);
    }
}