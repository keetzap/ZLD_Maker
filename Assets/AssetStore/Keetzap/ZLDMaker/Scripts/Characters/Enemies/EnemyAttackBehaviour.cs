using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class EnemyAttackBehaviour : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInParent<EnemyController>().CheckAttackBehaviour();
        }
    }
}