using System;
using System.Collections;
using System.Collections.Generic;
using AI.Combat;
using AI.Core;
using AI.Movement;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Control
{
    public class BlueAIController : AIController
    {
        private TMP_Text _levelText;
        
        public void SetLevelText(int level)
        {
            _levelText.text = "Level " + level.ToString();
        }
        
       
        private void Awake()
        {
            _gangManager = FindObjectOfType<GangManager>();
            tag = "pink";
            _levelText = GetComponentInChildren<TMP_Text>();
        }

     
        private void LateUpdate()
        {
            Walk();
        }

        public void Walk()
        {
            if (navMeshAgent.enabled && !health.IsDead() && !attackTarget)
            {
                mover.MoveTo(new Vector3(transform.position.x , transform.position.y ,
                    Player.Instance.spawnPoint.transform.position.z  ));

            }
        }

        public override void CheckEnemies( )
        {

            foreach (var pink in _gangManager.pinkAIs)
            {
                attackTarget = pink;
                if (InAttackRangeOfPlayer() && fighter.CanAttack(attackTarget))
                {
                    return;
                }
                else
                {
                    attackTarget = null;
                    fighter.Cancel();
                }
               
            }
        }
        

        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.GetComponent<PinkAIController>() || other.CompareTag("enemyBoss"))
        //     {
        //         if(health.IsDead()) return;
        //         if(other.GetComponent<Health>().IsDead() ) return;
        //         // print("hedef bulundu");
        //         ens.Add(other.GetComponent<PinkAIController>());
        //     }
        // }
        // private void OnTriggerExit(Collider other)
        // {
        //     if (other.GetComponent<PinkAIController>() || other.CompareTag("enemyBoss"))
        //     {
        //         ens.Remove(other.GetComponent<PinkAIController>());
        //     }
        // }

        // private void OnTriggerStay(Collider other)
        // {
        //     if (other.GetComponent<PinkAIController>() || other.CompareTag("enemyBoss"))
        //     {
        //         if(health.IsDead()) return;
        //         if(other.GetComponent<Health>().IsDead() ) return;
        //         // print("hedef bulundu");
        //         if(attackTarget == null || attackTarget.GetComponent<Health>().IsDead() )
        //         { 
        //             attackTarget = other.gameObject; 
        //             AttackBehavior();
        //            
        //         }
        //     }
        // }

    }
    
    
}