using System;
using Infastructure;
using Infastructure.Services;
using Infastructure.Services.SceneLoader;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer {
    public class MatchmakingManager : MonoBehaviour {
        [SerializeField] private Button _findOpponentsButton;
        [SerializeField] private Button _cancelFindButton;

        [SerializeField] private GameObject _mainMenuCanvas;
        [SerializeField] private GameObject _matchmakingCanvas;
        
        private MultiplayerManager _multiplayerManager;
        
        private void Start() {

            _findOpponentsButton.onClick.AddListener(FindOpponent);
            _cancelFindButton.onClick.AddListener(CancelFind);

            _multiplayerManager = MultiplayerManager.Instance;
            
            Messenger.AddListener(MessengerKey.GET_READY, GetReady);
            Messenger.AddListener(MessengerKey.START_GAME, StartGame);
            Messenger.AddListener(MessengerKey.CANCEL_START, CancelStart);
        }
        
        public void OnDestroy() {
            Messenger.RemoveListener(MessengerKey.GET_READY, GetReady);
            Messenger.RemoveListener(MessengerKey.START_GAME, StartGame);
            Messenger.RemoveListener(MessengerKey.CANCEL_START, CancelStart);
        }
        
        public async void FindOpponent() {
            _cancelFindButton.gameObject.SetActive(false);
            _mainMenuCanvas.SetActive(false);
            _matchmakingCanvas.SetActive(true);
            
            await MultiplayerManager.Instance.Connect();
            _cancelFindButton.gameObject.SetActive(true); 
        }

        public void CancelFind() {
            _matchmakingCanvas.SetActive(false);
            _mainMenuCanvas.SetActive(true); 
            
            MultiplayerManager.Instance.LeaveRoom();
        }

        private void StartGame() {
            GameBootstrapper.Curtain.Show();
            
            AllServices.Container.Single<ISceneLoader>().Load(Constants.GameSceneName, () => {
                GameBootstrapper.Curtain.Hide();
                Instantiate(Resources.Load(Path.Hud));
                _multiplayerManager.CreateClients();
            });
        }

        private void GetReady() => _cancelFindButton.gameObject.SetActive(false);

        private void CancelStart() => _cancelFindButton.gameObject.SetActive(true);
    }
}