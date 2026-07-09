using System;
using UnityEngine;

namespace Core.Player {
    public class AttackManager : MonoBehaviour {
        private CooldownController cooldownController;

        public void Initialize() {
            cooldownController = new CooldownController(
                CharacterManager.Instance.MyCharacter.maxBullets, 
                GameConfig.NormalAttackCoolDown, 
                CharacterManager.Instance.MyCharacter.skillAttackCoolDown
                );
        }

        private void Update() {
            if (cooldownController == null) return;

            cooldownController.Tick(Time.deltaTime);
        }

        public bool TryNormalAttack() => cooldownController.TryNormalAttack();
        public bool TrySkillAttack() => cooldownController.TrySkillAttack();
    }
}