namespace G_AI.Example
{
 
    using UnityEngine;
    using UnityEngine.AI;

    public class TargetSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private float delay = 4f;

        private float currentDelay;
    
        private void Update()
        {
            currentDelay += Time.deltaTime;

            if (currentDelay >= delay)
            {
                var randomDirection = Random.insideUnitSphere * 20;
                NavMesh.SamplePosition(randomDirection, out var hit, 20, 1);
                if (hit.position == Vector3.positiveInfinity) return;
                Instantiate(targetPrefab, hit.position, Quaternion.identity);
                currentDelay = 0;
            }
        }
    }
    
}
