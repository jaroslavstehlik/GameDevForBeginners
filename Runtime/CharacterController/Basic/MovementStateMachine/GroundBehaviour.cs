using UnityEngine;

namespace GameDevForBeginners
{
    public class GroundedBehaviour
    {
        private CollisionState collisionState;
        private PlayerInput playerInput;
        private MovementSettings movementSettings;

        public GroundedBehaviour(CollisionState collisionState, PlayerInput playerInput,
            MovementSettings movementSettings)
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
        }

        public MovementStateBehaviour Update(MovementStateData movementStateData)
        {
            GroundCollisionInfo groundCollisionInfo = collisionState.groundCollisionInfo;
            if (groundCollisionInfo == null)
            {
                return MovementStateBehaviour.Falling;
            }
            
            //Debug.Log($"isGrounded: {groundCollisionInfo.isGrounded}, isTooSteep: {groundCollisionInfo.isTooSteep}");
            if (!groundCollisionInfo.isGrounded)
            {
                return MovementStateBehaviour.Falling;
            }

            if (playerInput.jump.isPressed && !groundCollisionInfo.isTooSteep)
            {
                return MovementStateBehaviour.Jumped;
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

            Vector3 playerInputDirection = new Vector3(playerInput.move.x, 0f, playerInput.move.y).normalized;
            float playerInputMagnitude = Mathf.Clamp(playerInputDirection.magnitude, 0f, 1f) * playerSpeed;
            Vector3 playerMove = playerInputDirection * playerInputMagnitude;

            playerMove = ProjectVelocityOnNormal(playerInputDirection, playerInputMagnitude, collisionState.up,
                groundCollisionInfo.localGroundNormal);

            if (groundCollisionInfo.isTooSteep)
            {
                float rampPLayerLocalDotProduct = Vector3.Dot(playerInputDirection, groundCollisionInfo.localGroundNormal);
                if (rampPLayerLocalDotProduct < 0f)
                {
                    playerMove *= 0f;
                }
            }
            
            Vector3 gravityDirection = Physics.gravity.normalized;
            Vector3 velocity = (movementStateData.rotation * playerMove + gravityDirection * groundCollisionInfo.rampDistance);
            movementStateData.velocity = velocity / Time.fixedDeltaTime; 
            return MovementStateBehaviour.Grounded;
        }

        public void End(MovementStateData movementStateData)
        {
        }

        static Vector3 ProjectVelocityOnNormal(Vector3 velocityDirection, float velocityMagnitude, Vector3 playerUp,
            Vector3 groundNormal)
        {
            Vector3 rotationAxis = Vector3.Normalize(Vector3.Cross(velocityDirection, playerUp));
            Vector3 tangent = Vector3.Normalize(Vector3.Cross(groundNormal, rotationAxis));
            return tangent * velocityMagnitude;
        }
    }
}
