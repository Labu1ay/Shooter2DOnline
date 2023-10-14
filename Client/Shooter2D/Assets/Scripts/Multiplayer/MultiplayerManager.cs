using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using Logic;
using Logic.EnemyLogic;
using Logic.PlayerLogic;
using Unity.VisualScripting;
using UnityEngine;

namespace Multiplayer {
    public class MultiplayerManager : ColyseusManager<MultiplayerManager> {
        private const string RoomName = "state_handler";

        private const string GetReadyName = "GetReady";
        private const string StartGameName = "Start";
        private const string CancelStartName = "CancelStart";
        private const string ShootName = "Shoot";
        private const string LeaveName = "Leave";

        private ColyseusRoom<State> _room;

        private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();

        public string clientID {
            get {
                if (_room == null) return "";
                else return _room.SessionId;
            }
        }

        protected override void Awake() {
            base.Awake();
            Instance.InitializeClient();
            DontDestroyOnLoad(gameObject);
        }

#region Server
        public async Task Connect() {
            Dictionary<string, object> data = new Dictionary<string, object>() { { "hp", Constants.MaxHealth } };
            _room = await Instance.client.JoinOrCreate<State>(RoomName, data);
            
            _room.OnMessage<object>(GetReadyName, (empty) => Messenger.Broadcast(MessengerKey.GET_READY));
            _room.OnMessage<object>(StartGameName, (empty) => Messenger.Broadcast(MessengerKey.START_GAME));
            _room.OnMessage<object>(CancelStartName, (empty) => Messenger.Broadcast(MessengerKey.CANCEL_START));

            _room.OnMessage<string>(ShootName, ApplyShoot);
            _room.OnMessage<string>("Leave", (id) => {
                if (_enemies.ContainsKey(id) == false) return;
                _enemies[id].Destroy();
                Messenger.Broadcast(MessengerKey.WIN_GAME);
            } );
        }
        
        private void ApplyShoot(string jsonShootInfo) {
            ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);
            if (_enemies.ContainsKey(shootInfo.key) == false) {
                Debug.LogError("Enemy was gone, but he tried to shoot");
                return;
            }

            _enemies[shootInfo.key].Shoot(shootInfo);
        }
        
        public void CreateClients() {
            _room.State.players.ForEach((key, player) => {
                if (key == _room.SessionId) CreatePlayer(player);
                else CreateEnemy(key, player);
            });

            _room.State.players.OnAdd += CreateEnemy;
            _room.State.players.OnRemove += RemoveEnemy;
            
            _room.State.coins.ForEach(CreateCoins);
            
            _room.State.coins.OnAdd += (key, coin) => CreateCoins(coin);
            _room.State.coins.OnRemove += (key, coin) => RemoveCoins(coin);
        }
#endregion

#region Players
        private void CreatePlayer(Player player) {
            var position = new Vector3(player.pX, player.pY, player.pZ);
            Quaternion rotation = Quaternion.Euler(0, player.rY, 0);

            PlayerCharacter playerCharacter = Instantiate(Resources.Load(Path.Player).GetComponent<PlayerCharacter>(),
                position, rotation);
            player.OnChange += playerCharacter.OnChange;
        }

        private void CreateEnemy(string key, Player player) {
            var position = new Vector3(player.pX, player.pY, player.pZ);

            var enemy = Instantiate(Resources.Load(Path.Enemy).GetComponent<EnemyController>(), position,
                Quaternion.identity);
            enemy.Init(key, player);
            
            if (_enemies.ContainsKey(key) == false) _enemies.Add(key, enemy);
        }

        private void RemoveEnemy(string key, Player player) {
            if (_enemies.ContainsKey(key) == false) return;
            var enemy = _enemies[key];

            enemy.Destroy();
            _enemies.Remove(key);
        }
#endregion

#region Coin
        [SerializeField] private Coin _coinPrefab;
        private Dictionary<Vector2Float, Coin> _coins = new Dictionary<Vector2Float, Coin>();
    
        private void CreateCoins(Vector2Float vector2Float) {
            Vector3 position = new Vector3(vector2Float.cX, 0, vector2Float.cZ);
            
            Coin coin = Instantiate(_coinPrefab, position, Quaternion.identity);
            coin.Init(vector2Float);
        
            _coins.Add(vector2Float, coin);
        }
    
        private void RemoveCoins(Vector2Float vector2Float) {
            if (_coins.ContainsKey(vector2Float) == false) return;

            Coin coin = _coins[vector2Float];
            _coins.Remove(vector2Float);

           coin.Destroy();

        }
#endregion
        
        public void Leave() => SendMessage("leave", clientID);

        public void LeaveRoom() {
            _room.Leave();
            _room = null;
        }
        
        public void SendMessage(string key, Dictionary<string, object> data) => _room.Send(key, data);
        public void SendMessage(string key, string data) => _room.Send(key, data);
    }
}