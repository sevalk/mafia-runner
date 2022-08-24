using System;
using System.Collections;
using System.Collections.Generic;
using AI.Combat;
using AI.Control;
using AI.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class BribeGate : Gate
{
     [Header("Bribe Gate"), Space(10)]
    public Transform EnemyGang;
    private PinkAIController[] _enemyGang;
    public GameObject money;

    private void Awake()
    {
        if (useGatePrice)
        {
            price = gatePrice;
        }
        
        _enemyGang = GetComponentsInChildren<PinkAIController>();
    }

    public override void TriggerEnter(Collider other)
    {
       
        if (other.GetComponent<PlayerParent>())
        {
         
            
            // foreach (PinkAIController enemy in _enemyGang)
            // {
            //    // enemy.anim.SetTrigger("death");
            //     enemy.transform.GetChild(0).DOLocalRotate(new Vector3(0f, 90f, 0f), 0.1f);
            //     
            //     enemy.transform.DOJump(enemy.transform.position - new Vector3( 12f,12f,0f),
            //         10f, 1, 4f).OnComplete((() => Destroy(enemy.gameObject)));
            //  
            //     enemy.health.TakeDamage(enemy.health.GetHealth() + 10f);
            //     gangManager.pinkAIs.Remove(enemy.gameObject);
            //     enemy.enabled = false;
            //
            //
            // }
            
            
            foreach (PinkAIController enemy in _enemyGang)
            {
                

                var _money = Instantiate(money, transform.position, money.transform.rotation);
               // _money.GetComponentInChildren<ParticleSystem>().Play();
                Renderer rend =  enemy.transform.GetChild(0).GetChild(0).GetComponent<Renderer>();

                _money.transform.DOJump(enemy.transform.position, 10f, 1, 0.8f).OnComplete(() =>
                {
                    _money.GetComponentInChildren<ParticleSystem>().Play();
                    Destroy(_money.transform.GetChild(0).gameObject);
                    rend.material.color = Color.white;
                    enemy.anim.SetTrigger("cheer");
                    
                   
                });
                
                // enemy.transform.DOJump(enemy.transform.position - new Vector3( 12f,12f,0f),
                //     10f, 1, 4f).OnComplete((() => Destroy(enemy.gameObject)));
             
                //enemy.health.TakeDamage(enemy.health.GetHealth() + 10f);
                gangManager.pinkAIs.Remove(enemy.gameObject);
                enemy.enabled = false;
                enemy.GetComponent<NavMeshAgent>().enabled = false;


            }
            
            base.TriggerEnter(other);
            
        }
    }
    
    public override void CalculatePrice()
    {
        if (!useGatePrice)
        {
            //enemy levelları şu an değişmiyor ama değişirse bu fonksiyonun, enemy levellarının değiştiği yerde çağırılması gerekli
            int x = 0;
            foreach (Transform enemy in EnemyGang)
            { 
                x += enemy.GetComponent<PinkAIController>().level;
            }
            price = 5 * x;
            base.CalculatePrice();
        }
    }

   
}
