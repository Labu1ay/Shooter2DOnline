using System;
using UnityEngine;

namespace Logic.PlayerLogic {
    public class PlayerGun : Gun {
        [SerializeField] private Transform _bulletPoint;
        [SerializeField] private float _bulletSpeed = 7f;
        [SerializeField] private float _shootDelay = 0.1f;
        private int _damage;
        private float _lastShootTime;

        private void Awake() => _damage = Constants.Damage;

        public bool TryShoot(out ShootInfo info){
            info = new ShootInfo();

            if(Time.time - _lastShootTime < _shootDelay) return false;
        
            Vector3 position = _bulletPoint.position;
            Vector3 velocity = _bulletPoint.forward * _bulletSpeed;

            _lastShootTime = Time.time;
            
             Instantiate(_bulletPrefab,position, _bulletPoint.rotation).Init(velocity, _damage);
             
            info.pX = position.x;
            info.pY = position.y;
            info.pZ = position.z;
            info.dX = velocity.x;
            info.dY = velocity.y;
            info.dZ = velocity.z;

            return true;
        }
    }
}