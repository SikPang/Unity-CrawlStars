using System;
using System.Collections.Generic;
using Core.Controller;
using Core.Player;
using Core.Projectile;
using UnityEngine;

namespace Core.Simulator {
    // 나중에 클라에서도 tick 을 계산해야 하기에 서버로 이주 후에는 이름을 TickManager로 바꾸기
    public class Simulator : MonoBehaviour {
        [SerializeField] private InputProvider inputProvider;

        public int TickCount { get; private set; }
        private float accumulator;
        private bool isStarted;

        private const int TickRate = 30;
        private const float TickThreshold = 1f / TickRate;

        private void Update() {
            if (!isStarted) return;

            accumulator += Time.deltaTime;

            if (accumulator >= TickThreshold) {
                Tick();
                ++TickCount;
                accumulator -= TickThreshold;
            }
        }

        public void Initialize() {
            accumulator = 0;
            TickCount = 0;
            isStarted = false;
        }

        public void StartTime() {
            isStarted = true;
        }

        private List<ProjectileData> projectiles = new List<ProjectileData>();
        private List<ProjectileData> willRemoveProjectiles = new List<ProjectileData>();

        // temp data
        private Vector2 playerPos = Vector2.up;

        private ProjectileData BaseProjectile => new ProjectileData {
            Pos = playerPos,
            Speed = 0.8f,
            Damage = 10f,
            Radius = 0.3f,
            OwnerId = 1234
        };
        private const float MoveSpeed = 0.2f;
        private const float PlayerRadius = 0.5f;

        private void Tick() {
            // ----- Receive client's input -----
            Vector2 moveDirection = inputProvider.GetMoveDirection();
            Vector2 attackDirection = inputProvider.CaptureAttackDirection();

            // ----- Simulate -----
            if (moveDirection != Vector2.zero) {
                Vector2 movement = MoveSpeed * TickThreshold * moveDirection;
                playerPos = Physics.GetPlayerNextPos(playerPos, movement, PlayerRadius);
            }

            SimulateProjectiles();
            ProjectileData newProjectile = null; 
            if (attackDirection != Vector2.zero) {
                newProjectile = BaseProjectile;
                newProjectile.Dir = attackDirection;
                projectiles.Add(newProjectile);
            }

            // ----- Send to client -----
            PlayerManager.Instance.Move(playerPos);
            PlayerManager.Instance.Rotate(moveDirection);
            if (attackDirection != Vector2.zero) {
                PlayerManager.Instance.Attack(playerPos, attackDirection);
            }

            ProjectileManager.Instance.UpdateProjectiles(projectiles);
            if (newProjectile != null) {
                ProjectileManager.Instance.Create(newProjectile);
            }
        }

        private void SimulateProjectiles() {
            // Move Projectiles
            foreach (var projectile in projectiles) {
                // var nextPos = Physics.GetProjectileNextPos(projectile);
                
            }

            // Remove Collided Projectiles
            if (willRemoveProjectiles.Count > 0) {
                foreach (var projectile in willRemoveProjectiles) {
                    // need abandon prefab only client
                    projectiles.Remove(projectile);
                }
                willRemoveProjectiles.Clear();
            }
        }
    }
}