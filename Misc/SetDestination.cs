using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetDestination : MonoBehaviour
{
    [SerializeField] private Transform Target;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.isOnNavMesh)
        {
            agent.SetDestination(Target.position);
            DrawPath();
        }
    }
    
    private void DrawPath()
    {
        for (int i = 0; i < agent.path.corners.Length - 1; i++)
        {
            Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.red);
        }
    }
}
