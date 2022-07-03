namespace G_AI.Example
{
    
    using UnityEngine;

    public class PawnController : MonoBehaviour
    {
        [SerializeField] private RunBehaviorTree behaviorTreeRunner;
        [SerializeField] private AISense aiSense;
        private Blackboard blackboard;
        private Transform target;
        private bool isSeeTarget;

        private void Start()
        {
            blackboard = behaviorTreeRunner.blackboard;
            aiSense.OnSenseUpdated += OnSenseUpdated;
        }

        private void OnSenseUpdated(int seecount, Collider[] sensingcolliders)
        {
            if (seecount == 0)
            {
                isSeeTarget = false;
                blackboard.SetBoolValue("isSeeTarget", isSeeTarget);
                return;
            }
        
            target = sensingcolliders[0].transform;
            isSeeTarget = true;
            blackboard.SetTransformValue("target", target);
            blackboard.SetBoolValue("isSeeTarget", true);
        }

        public void AttackTarget()
        {
            Destroy(target.gameObject, 2);
            target.gameObject.SetActive(false);
            blackboard.SetBoolValue("isSeeTarget", false);
            isSeeTarget = false;
            target = null;
        }
    }

}
