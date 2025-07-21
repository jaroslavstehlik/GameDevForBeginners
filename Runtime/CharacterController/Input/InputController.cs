using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public event Action<PlayerInput> onPlayerInputChanged;

    public void UpdateInput(PlayerInput playerInput)
    {
        onPlayerInputChanged?.Invoke(playerInput);
    }
}
