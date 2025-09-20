using UnityEngine;

namespace GameDevForBeginners
{
    class FallBehaviour
    {
        private CollisionState collisionState;
        private PlayerInput playerInput;
        private MovementSettings movementSettings;
        private float fallStartTime;

        public FallBehaviour(CollisionState collisionState, PlayerInput playerInput, MovementSettings movementSettings)
        {
            this.collisionState = collisionState;
            this.playerInput = playerInput;
            this.movementSettings = movementSettings;
        }

        public void UpdatePlayerInput(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
        }

        public void Start(MovementStateData movementStateData)
        {
            fallStartTime = Time.fixedTime;
        }

        public MovementStateBehaviour Update(MovementStateData movementStateData)
        {
            GroundCollisionInfo groundCollisionInfo = collisionState.groundCollisionInfo;
            if (groundCollisionInfo != null && groundCollisionInfo.isGrounded)
            {
                return MovementStateBehaviour.Grounded;
            }
            
            float playerSpeed = movementSettings.moveSpeed;
            if (playerInput.crouch.isPressed)
            {
                playerSpeed *= movementSettings.crouchMultiplier;
            }
            else if (playerInput.sprint.isPressed)
            {
                playerSpeed *= movementSettings.sprintMultiplier;
            }
            
            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y);
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * playerSpeed;
            Vector3 playerMove = playerInputDirection.normalized * playerInputMagnitude;

            float fallDuration = Time.fixedTime - fallStartTime;
            float physicsDuration = 1f;
            float fallProgress = Mathf.Clamp01(fallDuration / physicsDuration);
            float fallAmount = Mathf.Pow(fallProgress, 2f) + 0.01f;
            
            Vector3 velocity = movementStateData.rotation * playerMove + movementSettings.fallSpeed * Physics.gravity * fallAmount;
            movementStateData.velocity = velocity / Time.fixedDeltaTime;
            
            return MovementStateBehaviour.Falling;
        }

        public void End(MovementStateData movementStateData)
        {
        }
    }
}