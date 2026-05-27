using Core.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Managing {
    public class PlaySceneHandler : MonoBehaviour {
        [SerializeField] private Button leaveButton;

        private void Start() {
            leaveButton.onClick.AddListener(OnClickLeaveButton);
        }

        private void OnClickLeaveButton() {
            Debug.Log("OnClickLeaveButton");
            SceneController.Instance.ChangeSceneAsync(SceneController.MainSceneName, GameManager.Instance.Dispose).Forget();
        }
    }
}