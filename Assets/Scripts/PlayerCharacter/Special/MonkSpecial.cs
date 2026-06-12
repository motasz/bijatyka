using System.Collections;
using Data;
using UnityEngine;

namespace PlayerCharacter.Special
{
    [CreateAssetMenu(menuName = "Data/Specials/Monk", fileName = "Monk-Special")]
    public class MonkSpecial: SpeciallAttack
    {
        public AttackData specialAttackData;

        public GameObject impactPrefab;
        public GameObject miragePrefab;
        public float teleportOffset = 1;
        public float blinkDelay = 0.5f;
        public Vector3 shadowlandPosition;
        public override void StartSpecial(PlayerController playerController)
        {
            playerController.SetMoveRoutine(MonkParryProcedure(playerController));
        }

        private IEnumerator MonkParryProcedure(PlayerController playerController)
        {
            playerController.isInvincible = true;
            var wasHit = false;
            var isEnemyToTheRight = playerController.IsEnemyToTheRight();
            
            playerController.SetState(ControllerState.SpecialWindUp);
            playerController.OnHit += () => wasHit = true;
            var timer = 0f;

            while (timer < specialAttackData.windUp && !wasHit)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (!wasHit)
            {
                playerController.BackToIdle();
                playerController.ResetMoveRoutine();
                yield return null;
            }
            
            SpawnMirage(playerController);
            HidePlayer(playerController);

            yield return new WaitForSeconds(blinkDelay);
            
            StealthTeleport(playerController, isEnemyToTheRight);
            playerController.isInvincible = false;
            playerController.SetState(ControllerState.SpecialActive);
            yield return null;
            playerController.canTurn = false;
            var impact = Instantiate(impactPrefab, playerController.transform.position, playerController.transform.rotation, playerController.transform); 
            impact.tag = playerController.gameObject.tag;
            
            yield return new WaitForSeconds(specialAttackData.active);
            Destroy(impact); 
            
            playerController.SetState(ControllerState.SpecialWindDown);
            playerController.enemy.canTurn = true;
            yield return new WaitForSeconds(specialAttackData.windDown);

            playerController.canTurn = true;
            playerController.BackToIdle();
            playerController.ResetMoveRoutine();
        }

        private void SpawnMirage(PlayerController playerController)
        {
            Instantiate(miragePrefab, playerController.transform.position, playerController.transform.rotation);
        }

        private void StealthTeleport(PlayerController playerController, bool isEnemyToTheRight)
        {
            playerController.MakeVisible();
            playerController.gravityEnabled = true;
            var newPosX = playerController.enemy.transform.position.x + (isEnemyToTheRight ? 1 : -1) * teleportOffset;
            var newPos = new Vector3(newPosX, playerController.verticalClamp, playerController.transform.position.z); 
            
            playerController.transform.position = newPos;
        }

        private void HidePlayer(PlayerController playerController)
        {
            playerController.enemy.canTurn = false;
            playerController.MakeInvisible();
            playerController.gravityEnabled = false;
            playerController.transform.position += shadowlandPosition;
        }

        public override bool Validate(PlayerController playerController)
        {
            return playerController.isGrounded;
        }
    }
}