using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Network {
    public class NetworkManager : SingletonMonoBehaviour<NetworkManager> {
        private NetworkConfig config;
        private WebSocketClient socket;
        private string jwtAccessToken;

        public RestApiClient Rest { get; private set; }
        public bool IsInitialized { get; private set; }

        protected override void Awake() {
            base.Awake();
            Initialize();
        }

        private void Update() {
            socket?.DispatchMessageQueue();
        }

        public void Initialize() {
            config = NetworkConfig.Load();
            if (config != null) {
                Rest = new RestApiClient(config.RestBaseUrl);
            }
            IsInitialized = Rest != null;
        }

        public void SetJwtToken(string accessToken) {
            if (!IsInitialized || string.IsNullOrEmpty(accessToken)) {
                Debug.LogError("NetworkManager.SetJwtToken::not initialized or invalid parameter");
                return;
            }

            jwtAccessToken = accessToken;
            Rest.SetJwtToken(accessToken);
        }

        public async UniTask ConnectSocketAsync(string roomID, string playerID) {
            if (!IsInitialized || string.IsNullOrEmpty(roomID) || string.IsNullOrEmpty(playerID)) {
                Debug.LogError("NetworkManager.ConnectSocketAsync::not initialized or invalid parameter");
                return;
            }

            if (socket != null) {
                await socket.DisconnectAsync();
            }

            socket = new WebSocketClient(config.GetWebSocketUrl(roomID, playerID));
            RegisterSocketLogHandlers(socket);
            socket.Connect(jwtAccessToken);
        }

        public async UniTask SendSocketJsonAsync<T>(T message) {
            if (!IsInitialized || message == null) {
                Debug.LogError("NetworkManager.SendSocketJsonAsync::not initialized or invalid parameter");
                return;
            }

            if (socket == null) {
                Debug.LogError("NetworkManager.SendSocketJsonAsync::socket is not created.");
                return;
            }

            await socket.SendJsonAsync(message);
        }

#region Test
        private static void RegisterSocketLogHandlers(WebSocketClient socketClient) {
            socketClient.Opened += () => Debug.Log("WebSocket OnOpen");
            socketClient.MessageReceived += message => Debug.Log($"WebSocket Message: {message}");
            socketClient.ErrorReceived += error => Debug.LogError($"WebSocket Error: {error}");
            socketClient.Closed += closeCode => Debug.Log($"WebSocket Closed: {closeCode}");
        }

        public async UniTask<NetworkTestSession> TestRestApiAsync() {
            // 1. 서버 상태 받아오기
            // HealthResponse health = await Rest.GetAsync<HealthResponse>("health");
            // Debug.Log($"REST Health: status={health.Status}, service={health.Service}");

            // 2. 방 리스트 받아오기
            // RoomListResponse roomList = await Rest.GetAsync<RoomListResponse>("rooms");
            // Debug.Log($"REST Rooms: count={roomList?.Rooms?.Length ?? 0}");

            // 3. 방 만들기: 여기서 방 Id 받음
            RoomResponse createdRoom = await Rest.PostAsync<object, RoomResponse>("rooms", null);
            if (string.IsNullOrEmpty(createdRoom?.Id)) {
                throw new InvalidOperationException("REST CreateRoom failed: room id is empty.");
            }
            Debug.Log($"REST CreateRoom: id={createdRoom.Id}, status={createdRoom.Status}");

            // 4. 플레이어 방에 참가시키기: 여기서 플레이어 Id 받음
            PlayerResponse player = await Rest.PostAsync<object, PlayerResponse>($"rooms/{createdRoom.Id}/players", null);
            Debug.Log($"REST CreateRoomPlayer: id={player.Id}, team={player.Team}, slot={player.Slot}");

            // 5. 방 상태 받아오기
            // RoomResponse room = await Rest.GetAsync<RoomResponse>($"rooms/{createdRoom.Id}");
            // Debug.Log($"REST GetRoom: id={room.Id}, status={room.Status}, players={room.Players?.Length ?? 0}");

            // 6. 방 시작하기
            RoomResponse startedRoom = await Rest.PostAsync<object, RoomResponse>($"rooms/{createdRoom.Id}/start", null);
            Debug.Log($"REST StartRoom: id={startedRoom.Id}, status={startedRoom.Status}, tick={startedRoom.LatestSnapshot?.Tick ?? 0}");

            return new NetworkTestSession(createdRoom.Id, player.Id);
        }
#endregion
    }
}
