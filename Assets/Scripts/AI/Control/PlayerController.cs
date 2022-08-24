using System;
using System.Collections;
using System.Collections.Generic;
using AI.Combat;
using AI.Core;
using AI.Movement;
using UnityEngine;

namespace AI.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Health _health;
        private void Start()
        {
            _health = GetComponent<Health>();
        }

        void Update()
        {
            //if (_health.IsDead()) return;
          //  if (InteractWithCombat()) return;
            
        }

        // private bool InteractWithCombat()
        // {
        //    
        //    
        //         CombatTarget target = Target.transform.GetComponent<CombatTarget>();
        //
        //         if (target == null) return false;
        //         
        //         if (!GetComponent<Fighter>().CanAttack(target.gameObject)) return false;
        //
        //         GetComponent<Fighter>().Attack(target.gameObject);
        //        
        //         return true;
        //    
        // }

      

      
    }

}