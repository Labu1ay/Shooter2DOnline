using UnityEngine;

namespace Logic {
    public class Health : MonoBehaviour {
        [SerializeField] private HealthUI _healthUI;
        private int _max;
        private int _current;
        public void SetMax(int max){
            _max = max;
            UpdateHP();
        }
        public void SetCurrent(int current){
            _current = current;
            if (_current <= 0) {
                Destroy(gameObject);
                Messenger.Broadcast(MessengerKey.LOSE_GAME);
            }
            UpdateHP();
        } 
        public void ApplyDamage(int damage){
            _current -= damage;
            if (_current <= 0) {
                Destroy(gameObject);
                Messenger.Broadcast(MessengerKey.WIN_GAME);
            }
            UpdateHP();
        }
        private void UpdateHP() => _healthUI.UpdateHealth(_max, _current);
    }
}