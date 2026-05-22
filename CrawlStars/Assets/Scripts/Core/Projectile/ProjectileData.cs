using UnityEngine;

namespace Core.Projectile {
    public class ProjectileData {
        public Vector2 Pos { get; set; }
        public Vector2 Dir { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float Radius { get; set; }
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public bool IsDestroyed { get; set; }
    }
}