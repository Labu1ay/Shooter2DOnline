using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

namespace Logic.EnemyLogic {
    public class EnemyCharacter : Character {
        public Vector3 targetPosition { get; private set; } = Vector3.zero;
        
        [SerializeField] private Health _health;
        private string _sessionId;
        private float _velocityMagnitude = 0;
        private Vector3 _localEulerAnglesY;

        public void Init(string sessionId) => _sessionId = sessionId;

        private void Start() => targetPosition = transform.position;

        private void Update() {
            if (_velocityMagnitude > .1f) {
                float maxDistance = _velocityMagnitude * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxDistance);
            }
            else {
                transform.position = targetPosition;
            }

            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(_localEulerAnglesY),
                Constants.RotationRate * Time.deltaTime);
        }
        
        public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval) {
            targetPosition = position + (velocity * averageInterval);
            _velocityMagnitude = velocity.magnitude;

            Velocity = velocity;
        }
        
        public void SetMaxHP(int value){
            MaxHealth = value;
            _health.SetMax(value);
            _health.SetCurrent(value);
        } 
        public void RestoreHP(int newValue){
            _health.SetCurrent(newValue);
        }
        
        public void ApplyDamage(int damage){
            _health.ApplyDamage(damage);

            Dictionary<string, object> data = new Dictionary<string, object>(){
                { "id", _sessionId },
                { "value", damage }
            };
            MultiplayerManager.Instance.SendMessage("damage", data);
        }
        
        public void SetRotateY(float value) => _localEulerAnglesY = new Vector3(0f, value, 0f);
    }
}