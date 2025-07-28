using UnityEngine;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Camera/FirstPersonCamera")]
    public class FirstPersonCamera : MonoBehaviour
    {
        public InputController InputController;
        public Transform target;
        public float mouseSensitivity = 100f;
        public bool flipMouseY = true;
        public float cameraYaw = 0;
        public float cameraPitch = 0;
        
        private PlayerInput _playerInput = new PlayerInput();

        private void OnEnable()
        {
            InputController.onPlayerInputChanged += OnInputChanged;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void OnInputChanged(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        private void LateUpdate()
        {
            float mouseX = _playerInput.look.x * mouseSensitivity * Time.deltaTime;
            float mouseY = (flipMouseY ? -_playerInput.look.y : _playerInput.look.y)  * mouseSensitivity * Time.deltaTime;

            cameraYaw += mouseX;
            cameraPitch = Mathf.Clamp(cameraPitch + mouseY, -90f, 90f);

            transform.position = target.position;
            transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        }
        
        private void OnDisable()
        {
            InputController.onPlayerInputChanged -= OnInputChanged;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}