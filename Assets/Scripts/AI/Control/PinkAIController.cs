using System;
using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.Control
{
    public class PinkAIController : AIController
    {
        public int level;
        public int moneyAmount;
       
        private void Awake()
        {
            _gangManager = FindObjectOfType<GangManager>();
            _gangManager.SetEnemyGangLevel(this , level);
            tag = "blue";
         
            _gangManager.pinkAIs.Add(gameObject);
           
        }

        private void OnDisable()
        {
            _gangManager.pinkAIs.Remove(gameObject);
        }

        public override void CheckEnemies()
        {
            
            foreach (Transform pink in GangManager.Instance.transform)
            {
                attackTarget = pink.gameObject;
                if (InAttackRangeOfPlayer() && fighter.CanAttack(attackTarget))
                {
                    return;
                }
                else
                {
                    attackTarget = null;
                }
               
            } 

        }
        private IEnumerator Check()
        {
            foreach (Transform pink in GangManager.Instance.transform)
            {
                attackTarget = pink.gameObject;
                if (InAttackRangeOfPlayer() && fighter.CanAttack(attackTarget))
                {
                    yield return new WaitWhile((() => !attackTarget.GetComponent<Health>().IsDead()));
                }
                else
                {
                    attackTarget = null;
                    fighter.Cancel();
                }

            }
        }
        
        // private void OnTriggerStay(Collider other)
        // {
        //     if (other.GetComponent<BlueAIController>())
        //     {
        //         if(health.IsDead()) return;
        //         if(other.GetComponent<Health>().IsDead() ) return;
        //         // print("hedef bulundu");
        //         if(attackTarget == null || attackTarget.GetComponent<Health>().IsDead() )
        //         { 
        //             attackTarget = other.gameObject; 
        //             AttackBehavior();
        //         }
        //     }
        // }
    }
}