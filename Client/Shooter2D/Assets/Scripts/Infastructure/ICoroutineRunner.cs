using System.Collections;
using UnityEngine;

namespace Infastructure {
    public interface ICoroutineRunner {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}