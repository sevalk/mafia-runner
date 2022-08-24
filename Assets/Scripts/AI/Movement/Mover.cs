using System;
using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Movement
{
    public class Mover : MonoBehaviour , IAction
    {

        private NavMeshAgent _navMeshAgent;
        private Health _health;


        void Start()
        {
            _health = GetComponent<Health>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

       
        void Update()
        {
            //_navMeshAgent.enabled = !_health.IsDead();
        }
        
        public void StartMovementAction(Vector3 destination)
        {
            GetComponent<ActionSchedular>().StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.destination = destination;
                _navMeshAgent.isStopped = false;
            }
          
        }

        public void Cancel()
        {
            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.isStopped = true;
            }
            
        }

    }
}
