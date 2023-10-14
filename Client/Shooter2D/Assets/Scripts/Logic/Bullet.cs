using System.Collections;
using UnityEngine;

namespace Logic {
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour {
        [SerializeField] private float _lifeTime = 5f;
        private Rigidbody _rigidbody;
        private int _damage;
        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        public void Init(Vector3 velocity, int damage = 0){
            _damage = damage;
            _rigidbody.velocity = velocity;
            StartCoroutine(DelayDestroy(_lifeTime));
        }

        private IEnumerator DelayDestroy(float delay){
            yield return new WaitForSecondsRealtime(delay);
            Destroy();
        }
        private void Destroy() => Destroy(gameObject);

        private void OnCollisionEnter(Collision other) {
            if(other.collider.TryGetComponent(out TakeDamage takeDamage)){
                takeDamage.ApplyDamage(_damage);
            }
            Destroy();
        }
    }
}