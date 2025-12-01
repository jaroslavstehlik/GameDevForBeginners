using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Mount controller

public class MountController : MonoBehaviour
{
    public Transform rootTransform;
    public InputActionReference interactAction;
    
    public UnityEvent onMounted;
    public UnityEvent onUnmounted;

    private Mountable _mountable;
    
    private void Start()
    {
        interactAction.action.Enable();
    }
    
    // When trigger is detected search for mountable script
    public void OnTriggerEnter2D(Collider2D collider2D)
    {
        // We are already standing inside a mountable collider.
        if (_mountable != null)
            return;
        
        // When mountable script is detected, assign it to our controller.
        // This makes our mount controller occupied/mounted and call
        _mountable = collider2D.GetComponent<Mountable>();
    }
    
    public void OnTriggerExit2D(Collider2D collider2D)
    {
        if (_mountable == null)
            return;

        // ignore when we are mounted 
        if(_mountable.mounted)
            return;

        Mountable candidate = collider2D.GetComponent<Mountable>();
        // we have to exit the same collider we have entered first
        if(_mountable != candidate)
            return;
        
        _mountable = null;
    }
    
    public void Mount()
    {
        if(_mountable == null)
            return;
        
        rootTransform.SetParent(_mountable.transform);
        
        // Call Mount event on mountable.
        _mountable.Mount();
        
        // Call Mount event on this mount controller.
        onMounted?.Invoke();
    }
    
    public void Unmount()
    {
        if(_mountable == null)
            return;
        
        rootTransform.SetParent(null);
        
        // Call Unmount event on mountable.
        _mountable.Unmount();
        
        // Call Unmount event on this mount controller.
        onUnmounted?.Invoke();
    }
    
    
    public void SetMounted(bool value)
    {
        if (value)
        {
            Mount();
        }
        else
        {
            Unmount();
        }
    }

    private void Update()
    {
        // Detect if interaction has been pressed
        if (interactAction.action.triggered)
        {
            // Detect if mountable exists.
            if (_mountable != null)
            {
                SetMounted(!_mountable.mounted);
            }
        }
    }
}


