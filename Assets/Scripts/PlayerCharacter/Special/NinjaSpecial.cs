using System.Collections;
using Data;
using UnityEngine;

namespace PlayerCharacter.Special
{
    [CreateAssetMenu(menuName = "Data/Specials/Ninja", fileName = "Ninja-Special")]
    public class NinjaSpecial: SpeciallAttack
    {
        public AttackData attackData;
        public GameObject aoePrefab;
        public GameObject auraPrefab;
        public GameObject impactPrefab;

        public AudioClip windUpSfx;
        public AudioClip impactSfx;
        
        private AudioSource _audioSource;
        
        public override void StartSpecial(PlayerController playerController)
        {
            playerController.SetMoveRoutine(FallProcedure(playerController));
        }

        private IEnumerator FallProcedure(PlayerController playerController)
        {
            if (_audioSource == null)
            {
                _audioSource = playerController.gameObject.GetComponent<AudioSource>();
            }
            
            playerController.SetState(ControllerState.SpecialWindUp);
            playerController.gravityEnabled = false;
            playerController.verticalVelocity = 0;
            playerController.horizontalClampEnabled = false;
            _audioSource.PlayOneShot(windUpSfx);
            playerController.Disable();
            yield return new WaitForSeconds(attackData.windUp);
            playerController.Enable();

            playerController.SetState(ControllerState.SpecialActive);
            playerController.verticalVelocity = -10;

            var aura = Instantiate(auraPrefab, playerController.transform.position, Quaternion.identity, playerController.transform);
            while (!playerController.isGrounded)
            {
                yield return null;
            }
            
            Destroy(aura);
            var aoe = Instantiate(aoePrefab, playerController.transform.position, Quaternion.identity, playerController.transform); 
            var impactAura =  Instantiate(impactPrefab, playerController.transform.position, Quaternion.identity, playerController.transform);
            _audioSource.PlayOneShot(impactSfx);
            
            playerController.SetState(ControllerState.SpecialWindDown);
            playerController.gravityEnabled = true;
            yield return new WaitForSeconds(attackData.windDown);
            
            Destroy(impactAura);
            Destroy(aoe);
            playerController.BackToIdle();
            playerController.ResetMoveRoutine();
            playerController.horizontalClampEnabled = true;
        }

        public override bool Validate(PlayerController playerController)
        {
            return !playerController.isGrounded;
        }
    }
}