using System.Collections.Generic;

namespace Core.Player {
    public class PlayerManager {
        private static PlayerManager _instance;
        public static PlayerManager Instance => _instance ??= new PlayerManager();
        
        private Dictionary<int, PlayerListener> _playerListeners = new Dictionary<int, PlayerListener>();

        public void Initialize(/* PlayerInfos from server */) {
            
        }
        
        public void RegisterPlayer(PlayerListener playerListener) {
            _playerListeners.TryAdd(playerListener.Id, playerListener);
        }
        
        public void ClearListeners() {
            _playerListeners.Clear();
        }

        public void Move() {
            foreach (var playerListener in _playerListeners) {
                
            }
        }
        
        public void Look() {
            
        }
        
        public void Attack() {
            
        }
    }
}