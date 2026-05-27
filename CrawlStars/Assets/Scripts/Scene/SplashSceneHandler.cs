using System;
using Cysharp.Threading.Tasks;
using Managing;
using UnityEngine;

public class SplashSceneHandler : MonoBehaviour {
    private void Start() {
        SceneController.Instance.ChangeSceneAsync(SceneController.MainSceneName).Forget();
    }
}
