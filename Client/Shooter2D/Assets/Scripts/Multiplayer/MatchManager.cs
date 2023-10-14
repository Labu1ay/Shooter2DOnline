using Infastructure;
using Infastructure.Services;
using Infastructure.Services.SceneLoader;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer {
    public class MatchManager : MonoBehaviour {
        [SerializeField] private Button [] _leaveRoomButtons;
        [SerializeField] private Text[] _scoreTexts;

        [SerializeField] private GameObject _canvasWinGame;
        [SerializeField] private GameObject _canvasLoseGame;
        [SerializeField] private GameObject _canvasMainGame;
        
        private MultiplayerManager _multiplayerManager;
        
        private void Start() {
            for (int i = 0; i < _leaveRoomButtons.Length; i++) {
                _leaveRoomButtons[i].onClick.AddListener(LeaveRoom);
            }
            _multiplayerManager = MultiplayerManager.Instance;
            
            Messenger.AddListener(MessengerKey.LOSE_GAME, LoseGame);
            Messenger.AddListener(MessengerKey.WIN_GAME, WinGame);
            Messenger.AddListener(MessengerKey.UPDATE_SCORE, UpdateScore);
        }

        private void OnDestroy() {
            Messenger.RemoveListener(MessengerKey.LOSE_GAME, LoseGame);
            Messenger.RemoveListener(MessengerKey.WIN_GAME, WinGame);
            Messenger.RemoveListener(MessengerKey.UPDATE_SCORE, UpdateScore);
        }

        private void UpdateScore(Bundle bundle) {
            for (int i = 0; i < _scoreTexts.Length; i++) {
                _scoreTexts[i].text = bundle.Get<int>("score").ToString();
            }
        }

        private void LeaveRoom() {
            GameBootstrapper.Curtain.Show();
            _multiplayerManager.Leave();
            
            AllServices.Container.Single<ISceneLoader>().Load(Constants.LobbySceneName, () => {
                GameBootstrapper.Curtain.Hide();
            });
        }

        private void WinGame() {
            _canvasMainGame.SetActive(false);
            _canvasWinGame.SetActive(true);
        }

        private void LoseGame() {
            _canvasMainGame.SetActive(false);
            _canvasLoseGame.SetActive(true);
        }
    }
}