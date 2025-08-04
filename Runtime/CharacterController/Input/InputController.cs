using System;
using UnityEngine.Events;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private PlayerInput _playerInput = new PlayerInput();
    public PlayerInput playerInput => _playerInput;
    public event Action<PlayerInput> onPlayerInputChanged;
    
    public UnityEvent onInteractStart = new UnityEvent();

    public void UpdateInput(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        onPlayerInputChanged?.Invoke(playerInput);
        
        if (_playerInput.interact.wasPressedThisFrame)
        {
            onInteractStart?.Invoke();
        }
    }
}
