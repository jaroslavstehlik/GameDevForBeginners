using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : InputController
{
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    public InputActionReference interactAction;

    private PlayerInput _playerInput = new PlayerInput();
    
    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();
        interactAction.action.Enable();
    }

    void UpdateActionButton(InputAction inputAction, ref ButtonInput buttonInput)
    {
        buttonInput.Update(inputAction.ReadValue<float>() > 0);
    }

    private void Update()
    {
        UpdateActionButton(sprintAction.action, ref _playerInput.sprint);
        UpdateActionButton(jumpAction.action, ref _playerInput.jump);
        UpdateActionButton(interactAction.action, ref _playerInput.interact);
        
        _playerInput.move = moveAction.action.ReadValue<Vector2>();
        _playerInput.look = lookAction.action.ReadValue<Vector2>();
        
        UpdateInput(_playerInput);
    }
}