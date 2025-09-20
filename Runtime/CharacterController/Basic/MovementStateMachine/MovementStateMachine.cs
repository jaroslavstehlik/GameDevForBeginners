using System;
using UnityEngine;

namespace GameDevForBeginners
{
    public enum MovementStateBehaviour
    {
        Falling,
        Grounded,
        Jumped,
    }

    public class MovementStateData
    {
        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }
        public Vector3 jumpDirection { get; private set; }
        public Vector3 velocity;

        public void SetPositionAndRotation(Rigidbody rigidbody)
        {
            this.position = rigidbody.position;
            this.rotation = rigidbody.rotation;
        }

        public void SetJumpDirection(Vector3 jumpDirection)
        {
            this.jumpDirection = jumpDirection.normalized;
        }
    }

    public class MovementStateMachine
    {
        private MovementStateBehaviour _lastMovementStateBehaviour;
        private MovementStateBehaviour _currentMovementStateBehaviour;

        private JumpBehaviour _jumpBehaviour;
        private FallBehaviour _fallBehaviour;
        private GroundedBehaviour _groundedBehaviour;

        void UpdateState(MovementStateData movementStateData, Action<MovementStateData> onStart, Func<MovementStateData, MovementStateBehaviour> onUpdate, Action<MovementStateData> onEnd)
        {
            if (_lastMovementStateBehaviour != _currentMovementStateBehaviour)
            {
                onStart.Invoke(movementStateData);
                _lastMovementStateBehaviour = _currentMovementStateBehaviour;
            }

            var newState = onUpdate.Invoke(movementStateData);
            if (newState != _currentMovementStateBehaviour)
            {
                _lastMovementStateBehaviour = _currentMovementStateBehaviour;
                _currentMovementStateBehaviour = newState;
                onEnd.Invoke(movementStateData);
            }
        }
        
        public void UpdatePlayerInput(PlayerInput playerInput)
        {
            _jumpBehaviour.UpdatePlayerInput(playerInput);
            _fallBehaviour.UpdatePlayerInput(playerInput);
            _groundedBehaviour.UpdatePlayerInput(playerInput);
        }

        public MovementStateMachine(CollisionState collisionState, PlayerInput playerInput,
            MovementSettings movementSettings)
        {
            _lastMovementStateBehaviour = MovementStateBehaviour.Falling;
            _currentMovementStateBehaviour = MovementStateBehaviour.Falling;

            _jumpBehaviour = new JumpBehaviour(collisionState, playerInput, movementSettings);
            _fallBehaviour = new FallBehaviour(collisionState, playerInput, movementSettings);
            _groundedBehaviour = new GroundedBehaviour(collisionState, playerInput, movementSettings);
        }

        public void Update(MovementStateData movementStateData)
        {
            switch (this._currentMovementStateBehaviour)
            {
                case MovementStateBehaviour.Falling:
                    UpdateState(movementStateData, _fallBehaviour.Start, _fallBehaviour.Update, _fallBehaviour.End);
                    break;
                case MovementStateBehaviour.Grounded:
                    UpdateState(movementStateData, _groundedBehaviour.Start, _groundedBehaviour.Update, _groundedBehaviour.End);
                    break;
                case MovementStateBehaviour.Jumped:
                    UpdateState(movementStateData, _jumpBehaviour.Start, _jumpBehaviour.Update, _jumpBehaviour.End);
                    break;
            }
        }
    }
}