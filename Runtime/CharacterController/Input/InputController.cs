using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private PlayerInput _playerInput = new PlayerInput();
    public PlayerInput playerInput => _playerInput;
    public event Action<PlayerInput> onPlayerInputChanged;

    public void UpdateInput(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        onPlayerInputChanged?.Invoke(playerInput);
    }
}
