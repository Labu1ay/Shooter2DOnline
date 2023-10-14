using System;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

namespace Logic.PlayerLogic {
    public class PlayerController : MonoBehaviour{
        [SerializeField] private PlayerCharacter _player;
        [SerializeField] private PlayerGun _gun;
        
        private MultiplayerManager _multiplayerManager;
        
        private void Start() {
            _multiplayerManager = MultiplayerManager.Instance;
            _player._shooted += TryShoot;
        }

        private void OnDestroy() => _player._shooted -= TryShoot;

        private void TryShoot() { 
            if(_gun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);
        }

        private void Update() {
            SendMove();
        }
        
        private void SendShoot(ref ShootInfo shootInfo){
            shootInfo.key = _multiplayerManager.clientID;
            string json = JsonUtility.ToJson(shootInfo);
            _multiplayerManager.SendMessage("shoot", json);
        }
        
        private void SendMove() {
            _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateY);
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "pX", position.x },
                { "pY", position.y },
                { "pZ", position.z },
                { "vX", velocity.x },
                { "vY", velocity.y },
                { "vZ", velocity.z },
                { "rY", rotateY }
            };
            _multiplayerManager.SendMessage("move", data);
        }
    }
    
    [System.Serializable]
    public struct ShootInfo {
        public string key;
        public float pX;
        public float pY;
        public float pZ;
        public float dX;
        public float dY;
        public float dZ;
    }
}