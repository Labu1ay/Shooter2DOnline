using UnityEngine;

namespace Infastructure.Services.Input {
    public class StandaloneInputService : InputService {
        public override Vector2 Axis {
            get {
                Vector2 axis = SimpleInputAxis();

                if (axis == Vector2.zero)
                    axis = UnityAxis();
                
                return axis;
            }
        }

        public override bool IsShoot {
            get {
                bool isShoot = IsShootButton();

                if (isShoot == false)
                    isShoot = UnityIsShootButton();

                return isShoot;
            }
        }

        private bool UnityIsShootButton() => UnityEngine.Input.GetMouseButtonDown(0);

        private static Vector2 UnityAxis() => 
            new(UnityEngine.Input.GetAxis(Horizontal), UnityEngine.Input.GetAxis(Vertical));
    }
}