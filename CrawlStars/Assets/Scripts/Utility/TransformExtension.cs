using UnityEngine;

namespace Utility {
    public static class TransformExtension {
        public static void LookAtZAxisOnly(this Transform transform, Vector3 target, float rotationSpeed = 10f) {
            Vector3 direction = target - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f); 
        
            // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.rotation = targetRotation;
        }
    }
}