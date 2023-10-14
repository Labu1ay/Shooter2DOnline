using System;
using Infastructure.Services;
using Infastructure.Services.Input;
using Infastructure.Services.SceneLoader;
using Logic;
using Unity.VisualScripting;
using UnityEngine;

namespace Infastructure {
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner {
        public static LoadingCurtain Curtain { get; private set; }
        private AllServices _services;

        private void Start() {
            _services = AllServices.Container;
            RegisterService();

            Curtain = Instantiate(Resources.Load(Path.Curtain).GetComponent<LoadingCurtain>());
            Curtain.Hide();
            AllServices.Container.Single<ISceneLoader>().Load(Constants.LobbySceneName);
            
            DontDestroyOnLoad(this);
        }

        private void RegisterService() {
            _services.RegisterSingle<IInputService>(RegisterInputService());
            _services.RegisterSingle<ISceneLoader>( new SceneLoader(this));
        }

        private static InputService RegisterInputService() {
            if (Application.isEditor) {
                return new StandaloneInputService();
            }
            else {
                return new MobileInputService();
            }
        }
    }
}