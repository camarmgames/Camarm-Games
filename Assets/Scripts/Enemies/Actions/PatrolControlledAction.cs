
namespace BehaviourAPI.UnityToolkit
{
    using Core;
    using UnityEngine;

    [SelectionGroup("MOVEMENT")]
    public class PatrolControlledAction : UnityAction
    {
        private Vector3[] patrolPoints;
        public PatrolControlledAction() { }

        public override void Start()
        {
            
        }
        public override Status Update()
        {

            return Status.Success;
        }
        public override void Stop()
        {
        }

        public override string ToString()
        {
            return "Controlled Patrol";
        }
    }
}