using System;

namespace Infastructure.Services.SceneLoader {
    public interface ISceneLoader : IService{
        void Load(string name, Action callback = null);
    }
}