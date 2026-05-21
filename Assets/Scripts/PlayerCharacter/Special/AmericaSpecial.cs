using System.Collections;
using Data;
using UnityEngine;

namespace PlayerCharacter.Special
{
    [CreateAssetMenu(menuName = "Data/America-Special", fileName = "America-Special")]
    public class AmericaSpecial: SpeciallAttack
    {
        public GameObject shotPrefab;
        public Vector2 shotPosition;
        public AttackData specialAttackData;
        public AudioClip shotSound;

        private AudioSource _audioSource;
        
        public override void StartSpecial(PlayerController playerController)
        {
            playerController.SetMoveRoutine(ShotProcedure(playerController));
        }

        private IEnumerator ShotProcedure(PlayerController playerController)
        {
            if (_audioSource == null)
            {
                _audioSource = playerController.gameObject.GetComponent<AudioSource>();
            }
            
            
            playerController.SetState(ControllerState.SpecialWindUp);
            yield return new WaitForSeconds(specialAttackData.windUp);
            
            playerController.SetState(ControllerState.SpecialActive);
            var shot = Instantiate(shotPrefab, shotPosition, Quaternion.identity, playerController.transform);
            shot.transform.localPosition = shotPosition;
            _audioSource.PlayOneShot(shotSound);
            yield return new WaitForSeconds(specialAttackData.active);
            
            playerController.SetState(ControllerState.SpecialWindDown);
            yield return new WaitForSeconds(specialAttackData.windDown);
            
            playerController.BackToIdle();
            playerController.ResetMoveRoutine();
        }
    }
}