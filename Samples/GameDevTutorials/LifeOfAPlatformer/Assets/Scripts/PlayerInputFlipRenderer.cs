using System;
using UnityEngine;

public class PlayerInputFlipRenderer : MonoBehaviour
{
    public PlayerInputController playerInputController;
    public SpriteRenderer spriteRenderer;

    private void LateUpdate()
    {
        if (playerInputController.playerInput.move.x < 0f)
        {
            spriteRenderer.flipX = true;
        } else if (playerInputController.playerInput.move.x > 0f)
        {
            spriteRenderer.flipX = false;
        }
    }
}
