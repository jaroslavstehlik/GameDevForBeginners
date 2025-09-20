using UnityEngine;

namespace GameDevForBeginners
{
    public class JumpBehaviour
    {
        private CollisionState collisionState;
        private PlayerInput playerInput;
        private MovementSettings movementSettings;

        private Vector3 jumpPosition;
        private Vector3 jumpDirection;

        public JumpBehaviour(CollisionState collisionState, PlayerInput playerInput, MovementSettings movementSettings)
        {
            this.collisionState = collisionState;
            this.playerInput = playerInput;
            this.movementSettings = movementSettings;

            jumpPosition = default;
            jumpDirection = default;
        }
        
        public void UpdatePlayerInput(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
        }

        public void Start(MovementStateData movementStateData)
        {
            this.jumpPosition = movementStateData.position;
            this.jumpDirection = movementStateData.jumpDirection;
        }

        float GetCurrentJumpHeight(Vector3 currentPosition)
        {
            return Vector3.Dot(currentPosition - jumpPosition, jumpDirection);
        }

        public MovementStateBehaviour Update(MovementStateData movementStateData)
        {
            float currentJumpHeight = GetCurrentJumpHeight(movementStateData.position);

            // We got stuck on ceiling
            if (collisionState.ceilingSphereCastInfo.collides)
            {
                movementStateData.velocity = Vector3.zero;
                return MovementStateBehaviour.Falling;
            }
            
            // We reached our jump height, start falling
            if (currentJumpHeight >= movementSettings.jumpHeight - 0.01f)
            {
                movementStateData.velocity = Vector3.zero;
                return MovementStateBehaviour.Falling;
            }
            
            float jumpProgress = Mathf.Clamp01(currentJumpHeight / movementSettings.jumpHeight);
            float jumpAmount = 1f - Mathf.Pow(jumpProgress, 2f);

            float playerSpeed = movementSettings.moveSpeed;
            if (playerInput.crouch.isPressed)
            {
                playerSpeed *= movementSettings.crouchMultiplier;
            }
            else if (playerInput.sprint.isPressed)
            {
                playerSpeed *= movementSettings.sprintMultiplier;
            }

            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * playerSpeed;
            Vector3 playerMove = playerInputDirection * playerInputMagnitude;

            Vector3 velocity = movementStateData.rotation * playerMove + jumpDirection * movementSettings.jumpSpeed * jumpAmount;
            movementStateData.velocity = velocity / Time.fixedDeltaTime;
            
            return MovementStateBehaviour.Jumped;
        }

        public void End(MovementStateData movementStateData)
        {
        }
    }
}
