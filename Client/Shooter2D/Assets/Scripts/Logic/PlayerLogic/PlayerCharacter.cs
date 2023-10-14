using System;
using System.Collections.Generic;
using Colyseus.Schema;
using Infastructure.Services;
using Infastructure.Services.Input;
using UnityEngine;

namespace Logic.PlayerLogic {
    public class PlayerCharacter : Character {
        [SerializeField] private Health _health;
        [SerializeField] private Score _score;
        
        private CharacterController _characterController;
        
        private IInputService _inputService;
        private Camera _camera;

        public event Action _shooted;

        private void Awake() {
            _inputService = AllServices.Container.Single<IInputService>();

            _characterController = GetComponent<CharacterController>();
        }

        private void Start() {
            _camera = Camera.main;
            
            _health.SetMax(MaxHealth);
            _health.SetCurrent(MaxHealth);
        }

        private void Update() {
            Vector3 movementVector = Vector3.zero;

            if (_inputService.Axis.sqrMagnitude > Constants.Epsilon) {
                movementVector = _camera.transform.TransformDirection(_inputService.Axis);
                movementVector.y = 0;
                movementVector.Normalize();

                transform.forward = Vector3.Lerp(transform.forward, movementVector, Time.deltaTime * Constants.RotationRate);
            }

            if (_inputService.IsShootButton()) 
                _shooted?.Invoke();

            movementVector += Physics.gravity;
            
            _characterController.Move(Speed * movementVector * Time.deltaTime);
        }

        private void FixedUpdate() => CheckCollision();

        private void CheckCollision() {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
            for (int i = 0; i < colliders.Length; i++) {
                if (colliders[i].gameObject.TryGetComponent(out Coin coin)) {
                    coin.Collect();
                }
            }
        }

        public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateY){
            position = transform.position;
            velocity = _characterController.velocity;
            
            rotateY = transform.eulerAngles.y;
        }
        
        internal void OnChange(List<DataChange> changes){
            foreach (var dataChange in changes){
                switch(dataChange.Field){
                    case "currentHP":
                        _health.SetCurrent((sbyte)dataChange.Value);
                        Debug.Log((sbyte)dataChange.Value);
                        break;
                    case "score":
                        _score.SetScore((ushort)dataChange.Value);
                        break;
                    default:
                        Debug.Log("Field changes are not processed " + dataChange.Field);
                        break;
                }
            }
        }
    }
}