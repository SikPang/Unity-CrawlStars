using System;
using System.Collections.Generic;
using Core.Player;
using UnityEngine;

// 나중에 클라에서도 tick 을 계산해야 하기에 서버로 이주 후에는 이름을 TickManager로 바꾸기
public class Simulator : MonoBehaviour {
    [SerializeField] private InputProvider _inputProvider;

    private float _accumulator = 0f;

    private const int TickRate = 30;
    private const float TickThreshold = 1f / TickRate;

    private void Update() {
        _accumulator += Time.deltaTime;

        if (_accumulator >= TickThreshold) {
            Tick();
            _accumulator -= TickThreshold;
        }
    }

    private void Tick() {
        // Get player's input
        Vector2 moveDirection = _inputProvider.GetMoveDirection();
        Vector2 lookDirection = _inputProvider.GetLookingSide();
        bool isPressedAttack = _inputProvider.GetAttack();

        // Simulate
        
        // Return to clients
        PlayerManager.Instance.Move();
        PlayerManager.Instance.Look();
        PlayerManager.Instance.Attack();
    }
}
