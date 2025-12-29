using System;
using UnityEngine;

namespace GameDevForBeginners
{        
    [AddComponentMenu("GMD/Character/CharacterController")]
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        Transform cameraTransform => _cameraTransform == null ? Camera.main.transform : _cameraTransform;

        void FixedUpdate()
        {
            
        }
    }
}