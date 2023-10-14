using UnityEngine;

namespace Infastructure.Services.Input {
    public class MobileInputService : InputService {
        public override Vector2 Axis => SimpleInputAxis();
        public override bool IsShoot => IsShootButton();
    }
}