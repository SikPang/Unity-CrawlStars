using Core.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Managing {
    public class MainSceneHandler : MonoBehaviour {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button exitButton;

        private void Start() {
            playButton.onClick.AddListener(OnClickPlayButton);
            settingButton.onClick.AddListener(OnClickSettingButton);
            exitButton.onClick.AddListener(OnClickExitButton);
        }

        private void OnClickPlayButton() {
            Debug.Log("OnClickPlayButton");
            SceneController.Instance.ChangeSceneAsync(SceneController.PlaySceneName, 
                GameManager.Instance.Initialize, 
                PlayerManager.Instance.FocusCamera).Forget();
        }

        private void OnClickSettingButton() {
            Debug.Log("OnClickSettingButton");
        }

        private void OnClickExitButton() {
            Debug.Log("OnClickExitButton");
        }
    }
}