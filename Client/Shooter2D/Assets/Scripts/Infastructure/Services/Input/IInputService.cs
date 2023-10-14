using UnityEngine;

namespace Infastructure.Services.Input {
    public interface IInputService : IService {
        Vector2 Axis { get; }
        bool IsShootButton();
    }
}