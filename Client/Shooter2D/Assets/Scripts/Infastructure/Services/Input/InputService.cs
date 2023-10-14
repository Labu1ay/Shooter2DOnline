using UnityEngine;

namespace Infastructure.Services.Input {
    public abstract class InputService : IInputService {
        protected const string Horizontal = "Horizontal";
        protected const string Vertical = "Vertical"; 
        protected const string Button = "0"; 
        
        public abstract Vector2 Axis { get; }
        public abstract bool IsShoot { get; }

        public bool IsShootButton() => SimpleInput.GetButtonDown(Button);

        protected static Vector2 SimpleInputAxis() => 
            new(SimpleInput.GetAxis(Horizontal), SimpleInput.GetAxis(Vertical));
    }
}