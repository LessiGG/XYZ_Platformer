using PixelCrew.Components.GoBased;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs.Boss
{
    public class BossNextStateStage : StateMachineBehaviour
    {
        [ColorUsage(true, true)]
        [SerializeField] private Color _stageColor;
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var spawner = animator.GetComponent<CircularProjectileSpawner>();
            spawner.Stage++;

            var lightChanger = animator.GetComponent<ChangeLightComponent>();
            lightChanger.SetColor(_stageColor);
        }
    }
}
