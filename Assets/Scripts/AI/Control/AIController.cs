using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AI.Combat;
using AI.Core;
using AI.Movement;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Control
{
    public class AIController : MonoBehaviour
    {

        public Gun gun;
        [SerializeField] float chaseDistance = 5f;
        

        [HideInInspector] public Health health;
        [HideInInspector] public Fighter fighter;
        [HideInInspector] public GameObject attackTarget;
        [HideInInspector] public Mover mover;
        [HideInInspector] public NavMeshAgent navMeshAgent;
        
        
         [HideInInspector] public GangManager _gangManager;
         [HideInInspector] public Animator anim;

         
       
         private void Start()
        {
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            anim = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();

        }
        void Update()
        {
            if (health.IsDead()) return;
            
            
            if (!attackTarget  || attackTarget.GetComponent<Health>().IsDead())
            {
                CheckEnemies();
            }
            
            if (!attackTarget) return;
            
            if (InAttackRangeOfPlayer() && fighter.CanAttack(attackTarget))
            {
                AttackBehavior();
               
            }
        }
        public virtual void CheckEnemies()
        {
        }
        

        public void AttackBehavior()
        {
            fighter.Attack(attackTarget);
           
        }

        public bool InAttackRangeOfPlayer()
        {
            float distance = Vector3.Distance(attackTarget.transform.position, transform.position);
            return distance <= chaseDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
            
        }
    }

}