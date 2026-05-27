using System;
using CameraControl;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace Managing {
    public class SceneController : SingletonMonoBehaviour<SceneController> {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI progressText;

        public const string PlaySceneName = "Play";
        public const string MainSceneName = "Main";
        
        private bool isChangingScene;

        private void Start() {
            loadingScreen.gameObject.SetActive(false);
        }

        public async UniTask ChangeSceneAsync(string sceneName, Action beforeActivateAction, Action afterActivateAction) {
            if (isChangingScene) {
                Debug.LogWarning($"SceneController.ChangeScene::already changing scene to {sceneName}");
                return;
            }

            isChangingScene = true;
            Scene currentScene = SceneManager.GetActiveScene();

            // 다음 씬 Load
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (op == null) {
                Debug.LogError($"SceneController.ChangeScene::failed {sceneName}");
                isChangingScene = false;
                return;
            }

            progressBar.fillAmount = 0f;
            progressText.text = "Loading...";
            loadingScreen.gameObject.SetActive(true);

            op.allowSceneActivation = false;

            while (op.progress < 0.9f) {
                float progress = Mathf.Clamp01(op.progress / 0.9f);

                progressBar.fillAmount = progress;
                progressText.text = $"{progress * 100f:0}%";

                await UniTask.Yield();
            }

            progressBar.fillAmount = 1f;
            progressText.text = "100%";

            // fake loading
            await UniTask.Delay(300);
            beforeActivateAction?.Invoke();

            op.allowSceneActivation = true;
            await UniTask.WaitUntil(() => op.isDone);

            // 다음 씬 Activate
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(loadedScene);

            // 기존 씬 Unload
            op = SceneManager.UnloadSceneAsync(currentScene);
            if (op != null) {
                await UniTask.WaitUntil(() => op.isDone);
            } else {
                Debug.LogError($"SceneController.ChangeSceneAdditive::failed unload {currentScene.name}");
            }

            CommonCache.OnChangeScene();
            afterActivateAction?.Invoke();

            loadingScreen.SetActive(false);
            isChangingScene = false;
        }
    }
}