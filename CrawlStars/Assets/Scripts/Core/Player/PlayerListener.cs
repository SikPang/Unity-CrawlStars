using UnityEngine;

namespace Core.Player {
    public class PlayerListener : MonoBehaviour {
        public int Id { get; set; }

        public void MoveTo(Vector2 pos) {
            transform.position = pos;
        }

        public void Attack(Vector2 dir) {
            // attack to dir
        }
    }
}