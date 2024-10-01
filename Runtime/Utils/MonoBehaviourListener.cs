using System;
using UnityEngine;
using UnityEngine.Events;

public class MonoBehaviourListener : MonoBehaviour
{
    public UnityEvent<GameObject> onAwake;
    public UnityEvent<GameObject> onEnable;
    public UnityEvent<GameObject> onStart;
    public UnityEvent<GameObject> onUpdate;
    public UnityEvent<GameObject> onLateUpdate;
    public UnityEvent<GameObject> onDisable;
    public UnityEvent<GameObject> onDestroy;

    private void Awake()
    {
        onAwake?.Invoke(gameObject);
    }

    void OnEnable()
    {
        onEnable?.Invoke(gameObject);
    }

    void Start()
    {
        onStart?.Invoke(gameObject);
    }
    
    void Update()
    {
        onUpdate?.Invoke(gameObject);
    }
    
    void LateUpdate()
    {
        onLateUpdate?.Invoke(gameObject);
    }
    
    void OnDisable()
    {
        onDisable?.Invoke(gameObject);
    }
    
    void OnDestroy()
    {
        onDestroy?.Invoke(gameObject);
    }
}
