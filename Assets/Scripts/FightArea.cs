using System;
using System.Collections;
using System.Collections.Generic;
using AI.Combat;
using AI.Control;
using AI.Core;
using UnityEngine;

public class FightArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PinkAIController>())
        {
        
            var aıController = GetComponent<AIController>();
            if (!aıController.attackTarget || aıController.attackTarget.GetComponent<Health>().IsDead())
            {
                aıController.attackTarget = other.gameObject;
                GetComponent<Fighter>().Attack(aıController.attackTarget);
            }
        }
    }
}
