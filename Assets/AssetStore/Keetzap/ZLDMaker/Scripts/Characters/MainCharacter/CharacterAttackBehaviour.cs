using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class CharacterAttackBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInParent<PlayerController>().OnAttackEvent(true);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInParent<PlayerController>().OnAttackEvent(false);
        }
    }
}