using System.Collections.Generic;
using Colyseus.Schema;
using Multiplayer;
using UnityEngine;

namespace Logic {
    public class Coin : MonoBehaviour {
        [SerializeField] private float _speedRotationCoin = 120f;
        private Vector2Float _coin;

        public void Init(Vector2Float coin) {
            _coin = coin;
            _coin.OnChange += OnChange;
        }
        
        private void OnChange(List<DataChange> changes) {
        
            Vector3 position = transform.position;
            foreach (var change in changes) {
                switch (change.Field) {
                    case "cX":
                        position.x = (float)change.Value;
                        break;
                    case "cZ":
                        position.z = (float)change.Value; 
                        break;
                    default:
                        Debug.LogWarning("Field changes are not processed :" + change.Field);
                        break;
                }
            }

            //Debug.Log($"The coin has moved from its position {transform.position} in {position}");
            transform.position = position;
            gameObject.SetActive(true);
        }

        private void Update() => transform.Rotate(0, _speedRotationCoin * Time.deltaTime, 0);

        public void Collect() {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                {"id", _coin.id}
            };
            MultiplayerManager.Instance.SendMessage("collect", data);

            gameObject.SetActive(false);
        }

        public void Destroy() {
            if(_coin != null) _coin.OnChange -= OnChange;
            Destroy(gameObject);
        }
    }
}