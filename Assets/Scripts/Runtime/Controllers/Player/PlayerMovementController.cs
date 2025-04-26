using Runtime.Data.ValueObjects;
using Runtime.Keys;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;

        #endregion
        
        #region Private Variables

        [ShowInInspector] private PlayerMovementData _data;
        [ShowInInspector] private bool _isReadyToMove, _isReadyToPlay;
        [ShowInInspector] private float _xValue;

        private float2 _clampValues;
        
        #endregion

        #endregion

        internal void SetData(PlayerMovementData data)
        {
            _data = data;
        }

        private void FixedUpdate()
        {
            if (!_isReadyToPlay)
            {
                StopPlayer();
                return;
            }
            
            if(_isReadyToMove)
            {
                MovePlayer();
            }
            else
            {
                StopPlayerHorizontally();
            }
        }
        private void StopPlayer()
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        
        private void StopPlayerHorizontally()
        {
            rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, _data.ForwardSpeed);
            rigidbody.angularVelocity = Vector3.zero;
        }
        
        private void MovePlayer()
        {
            var velocity = rigidbody.linearVelocity;
            velocity = new Vector3(_xValue * _data.SideWaySpeed, velocity.y, _data.ForwardSpeed);
            rigidbody.linearVelocity = velocity;
            
            var position = rigidbody.position;
            position = new Vector3(Mathf.Clamp(rigidbody.position.x, _clampValues.x , _clampValues.y), 
                position.y, 
                position.z);
            rigidbody.position = position;
        }

        internal void IsReadyToPlay(bool condition)
        {
            _isReadyToPlay = condition;
        }
        
        internal void IsReadyToMove(bool condition)
        {
            _isReadyToMove = condition;
        }

        internal void UpdateInputParams(HorizontalInputParams inputParams)
        {
            _xValue = inputParams.HorizontalValue;
            _clampValues = inputParams.ClampValues;
        }

        internal void OnReset()
        {
            StopPlayer();
            _isReadyToMove = false;
            _isReadyToPlay = false;
        }
    }
}