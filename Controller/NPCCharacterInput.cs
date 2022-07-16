using System;
using Transition;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
/*
namespace CharacterInput
{
    
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCCharacterInput : MonoBehaviour, ICharacterInput
    {
        private NavMeshAgent agent;
        private TransitionHandler _walkruntransitionHandler;
        public float horizontal;
        public float vertical;
        public Vector3 DesiredDirection;
        public bool isSprinting=false;
        private MovementHistory _verticalHistory;
        private MovementHistory _horizontalHistory;
        private int historyMaxLength=4;
        
        public const float SprintDistanceThreshold = 10f;

        public UnityEvent OnRun;
        public UnityEvent OnWalk;

        private void Awake()
        {
            agent=GetComponent<NavMeshAgent>();
            _walkruntransitionHandler=GetComponent<TransitionHandler>();
            _verticalHistory= new MovementHistory(historyMaxLength);
            _horizontalHistory= new MovementHistory(historyMaxLength);
        }
        private void Update()
        {
            CalculateVariables();
            CalculateRunning();
        }

        private void CalculateRunning()
        {
            if(agent.remainingDistance> SprintDistanceThreshold)
            {
                OnRun?.Invoke();
            }
            else
            {
                OnWalk?.Invoke();
            }
            if(_walkruntransitionHandler.TargetMultiplier==1f)
            {
                isSprinting=true;
            }
            else if(_walkruntransitionHandler.TargetMultiplier==0f)
            {
                isSprinting=false;
            }
        }

        private void CalculateVariables()
        {
            if(agent.stoppingDistance >= Vector3.Distance(transform.position,agent.destination))
            {
                horizontal=0f;
                vertical=0f;
                DesiredDirection=Vector3.zero;
            }
            else if(agent.hasPath)
            {
                var firstPoint=  agent.path.corners[1];
                DesiredDirection =firstPoint-transform.position;
                horizontal=DesiredDirection.x;
                vertical=DesiredDirection.z;
            }
            _verticalHistory.Tick(vertical);
            _horizontalHistory.Tick(horizontal);
        }

        public float MovementHorizontal()
        {
            return horizontal;
        }

        public float MovementVertical()
        {
            return vertical;
        }
        public bool IsRunning()
        {
            return isSprinting;
        }

        public bool IsThereMovement()
        {
            float verticalAverage=_verticalHistory.Average();
            float horizontalAverage=_horizontalHistory.Average();
        
            bool isMovementhere= verticalAverage > 0.0025 || horizontalAverage > 0.0025;
            return isMovementhere;
        }

        public bool AttemptingToJump()
        {
            throw new NotImplementedException();
        }

        public bool Attack()
        {
            throw new NotImplementedException();
        }

        public void ResetAttack()
        {
            throw new NotImplementedException();
        }

        public Transform ViewTransform()
        {
            throw new NotImplementedException();
        }
    }
}
*/