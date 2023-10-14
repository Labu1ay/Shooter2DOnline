using UnityEngine;

namespace Logic {
    public abstract class Character : MonoBehaviour {
        [field: SerializeField] public int MaxHealth {get; protected set;} = Constants.MaxHealth;
        [field: SerializeField] public float Speed {get; protected set;} = Constants.Speed;
        public Vector3 Velocity {get; protected set;}
    }
}