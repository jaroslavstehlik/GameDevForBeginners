using System;
using UnityEngine;

public abstract class PlayerInputController : MonoBehaviour
{
    public event Action<PlayerInput> onPlayerInputChanged;

    protected void UpdateInput(PlayerInput playerInput)
    {
        onPlayerInputChanged?.Invoke(playerInput);
    }
}