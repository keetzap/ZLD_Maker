using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public enum TypeOfParameter
    {
        Idle, Patrol, Chase, Attack
    }

    public class EnemyExitBehaviour : StateMachineBehaviour
    {
        [Space(5)]
        public TypeOfParameter animatorParameter;

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInParent<EnemyController>().DisableAnimatorParameter(animatorParameter);
        }
    }
}