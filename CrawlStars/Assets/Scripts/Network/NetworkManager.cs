using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Network {
    public sealed class NetworkManager : SingletonMonoBehaviour<NetworkManager> {
        private NetworkConfig config;
        private WebSocketClient socket;
        private string jwtAccessToken;

        public RestApiClient Rest { get; private set; }

        protected override void Awake() {
            base.Awake();
            config = NetworkConfig.Load();
            Rest = new RestApiClient(config.RestBaseUrl);
        }

        private void Update() {
            socket?.DispatchMessageQueue();
        }

        public void SetJwtToken(string accessToken) {
            jwtAccessToken = accessToken;
            Rest.SetJwtToken(accessToken);
        }

        public async UniTask ConnectSocketAsync(string roomID, string playerID) {
            if (socket != null) {
                await socket.DisconnectAsync();
            }

            socket = new WebSocketClient(config.GetWebSocketUrl(roomID, playerID));
            RegisterSocketLogHandlers(socket);
            socket.Connect(jwtAccessToken);
        }

        public async UniTask SendSocketJsonAsync<T>(T message) {
            if (socket == null) {
                Debug.LogError("NetworkManager.SendSocketJsonAsync::socket is not created.");
                return;
            }

            await socket.SendJsonAsync(message);
        }

#region Test
        private void RegisterSocketLogHandlers(WebSocketClient socketClient) {
            socketClient.Opened += () => Debug.Log("WebSocket OnOpen");
            socketClient.MessageReceived += message => Debug.Log($"WebSocket Message: {message}");
            socketClient.ErrorReceived += error => Debug.LogError($"WebSocket Error: {error}");
            socketClient.Closed += closeCode => Debug.Log($"WebSocket Closed: {closeCode}");
        }

        public async UniTask<NetworkTestSession> TestRestApiAsync() {
            HealthResponse health = await Rest.GetAsync<HealthResponse>("health");
            Debug.Log($"REST Health: status={health.Status}, service={health.Service}");

            RoomListResponse roomList = await Rest.GetAsync<RoomListResponse>("rooms");
            Debug.Log($"REST Rooms: count={roomList?.Rooms?.Length ?? 0}");

            RoomResponse createdRoom = await Rest.PostAsync<object, RoomResponse>("rooms", null);
            if (string.IsNullOrEmpty(createdRoom?.Id)) {
                throw new InvalidOperationException("REST CreateRoom failed: room id is empty.");
            }
            Debug.Log($"REST CreateRoom: id={createdRoom.Id}, status={createdRoom.Status}");

            PlayerResponse player = await Rest.PostAsync<object, PlayerResponse>($"rooms/{createdRoom.Id}/players", null);
            Debug.Log($"REST CreateRoomPlayer: id={player.Id}, team={player.Team}, slot={player.Slot}");

            RoomResponse room = await Rest.GetAsync<RoomResponse>($"rooms/{createdRoom.Id}");
            Debug.Log($"REST GetRoom: id={room.Id}, status={room.Status}, players={room.Players?.Length ?? 0}");

            RoomResponse startedRoom = await Rest.PostAsync<object, RoomResponse>($"rooms/{createdRoom.Id}/start", null);
            Debug.Log($"REST StartRoom: id={startedRoom.Id}, status={startedRoom.Status}, tick={startedRoom.LatestSnapshot?.Tick ?? 0}");

            return new NetworkTestSession(createdRoom.Id, player.Id);
        }
#endregion
    }
}
