using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class Score : MonoBehaviour {
        private int _score;
        [SerializeField] private Text _scoreUI;

        public void SetScore(int value) {
            _score = value;
            UpdateScoreUI();
        }

        private void UpdateScoreUI() {
            _scoreUI.text = _score.ToString();
            Messenger.Broadcast(MessengerKey.UPDATE_SCORE, new Bundle().Set("score", _score));
        }
    }
}