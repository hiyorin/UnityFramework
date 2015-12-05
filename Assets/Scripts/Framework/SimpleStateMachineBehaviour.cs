using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    public class StateMachineEvent : UnityEvent<Animator, AnimatorStateInfo, int> {}

    public class SimpleStateMachineBehaviour : StateMachineBehaviour
    {
        public readonly StateMachineEvent onStateEnter = new StateMachineEvent();
        public readonly StateMachineEvent onStateExit = new StateMachineEvent ();
        public readonly StateMachineEvent onStateIK = new StateMachineEvent ();
        public readonly StateMachineEvent onStateUpdate = new StateMachineEvent ();

        public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter (animator, stateInfo, layerIndex);
            onStateEnter.Invoke (animator, stateInfo, layerIndex);
        }

        public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit (animator, stateInfo, layerIndex);
            onStateExit.Invoke (animator, stateInfo, layerIndex);
        }

        public override void OnStateIK (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateIK (animator, stateInfo, layerIndex);
            onStateIK.Invoke (animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate (animator, stateInfo, layerIndex);
            onStateUpdate.Invoke (animator, stateInfo, layerIndex);
        }
    }
}