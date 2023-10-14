using Logic.EnemyLogic;
using UnityEngine;

namespace Logic {
    public class TakeDamage : MonoBehaviour {
        [SerializeField] private EnemyCharacter _enemyCharacter;

        public void ApplyDamage(int damage) => _enemyCharacter.ApplyDamage(damage);
    }
}