using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractTrigger : MonoBehaviour
{
    public UnityEvent onInteractStart = new UnityEvent();
    private InputController _inputController = null;

    // Subscribe to input controller when component enables
    private void OnEnable()
    {
        if (_inputController != null)
        {
            _inputController.onInteractStart.AddListener(OnInteractStartHandler);
        }
    }

    // Unsubscribe to input controller when component disables
    private void OnDisable()
    {
        if (_inputController != null)
        {
            _inputController.onInteractStart.RemoveListener(OnInteractStartHandler);
        }
    }

    // Ubsubscribe previous controller and subscribe to new one
    void SetInputController(InputController inputController)
    {
        if (_inputController != null)
        {
            _inputController.onInteractStart.RemoveListener(OnInteractStartHandler);
        }

        _inputController = inputController;
        if (_inputController != null)
        {
            _inputController.onInteractStart.AddListener(OnInteractStartHandler);
        }
    }
    
    // Provide multiple function overrides for simpler usage
    public void Activate(Collider2D collider)
    {
        ActivateInternal(collider.gameObject);
    }

    public void Activate(Collider collider)
    {
        ActivateInternal(collider.gameObject);
    }

    public void Deactivate(Collider2D collider)
    {
        DeactivateInternal(collider.gameObject);
    }

    public void Deactivate(Collider collider)
    {
        DeactivateInternal(collider.gameObject);
    }

    // Implement the activation and deactivation once
    void ActivateInternal(GameObject gameObject)
    {
        InputControllerMarker inputControllerMarker = gameObject.GetComponent<InputControllerMarker>();
        if(inputControllerMarker == null)
            return;

        SetInputController(inputControllerMarker.inputController);
    }
    
    void DeactivateInternal(GameObject gameObject)
    {
        InputControllerMarker inputControllerMarker = gameObject.GetComponent<InputControllerMarker>();
        if(inputControllerMarker.inputController != _inputController)
            return;

        SetInputController(null);
    }
    
    void OnInteractStartHandler()
    {
        onInteractStart?.Invoke();
    }
}
