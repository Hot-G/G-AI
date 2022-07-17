namespace G_AI.Example
{
    using UnityEngine;

    public class PawnController : MonoBehaviour
    {
        [SerializeField] private RunBehaviorTree behaviorTreeRunner;
        [SerializeField] private Detector detector;
        private Blackboard blackboard;
        private Transform target;

        private void Start()
        {
            blackboard = behaviorTreeRunner.blackboard;
            detector.onDetected += OnTargetDetected;
            detector.onLostDetection += OnLostDetection;
        }

        private void OnTargetDetected(GameObject detectedGameObject)
        {
            if (target) return;
            
            target = detectedGameObject.transform;
            blackboard.SetTransformValue("target", target);
            blackboard.SetBoolValue("isSeeTarget", true);
        }
        
        private void OnLostDetection(GameObject obj)
        {
            if (target == null) return;
            if (target != obj.transform) return;
            
            ChangeTarget();
        }

        private void ChangeTarget()
        {
            if (detector.DetectedGameObjectLength() == 0)
            {
                target = null;
                blackboard.SetBoolValue("isSeeTarget", false);
                blackboard.SetTransformValue("target", null);   
            }
            else
            {
                target = detector.GetFirstDetectedObject().transform;
                blackboard.SetTransformValue("target", target);   
            }
        }

        public void AttackTarget()
        {
            Destroy(target.gameObject, 2);
            target.gameObject.SetActive(false);
            ChangeTarget();
        }
    }

}
