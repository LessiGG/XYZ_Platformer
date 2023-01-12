using UnityEngine;

namespace PixelCrew.Creatures.Mobs.Boss.FlyTheBoss
{
    public class FlyPrepareState : StateMachineBehaviour
    {
        private FlyController _fly;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _fly = animator.GetComponent<FlyController>();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _fly.StopMoving();
        }
    }
}