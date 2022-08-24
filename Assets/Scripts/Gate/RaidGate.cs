using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RaidGate : Gate
{
     [Header("Raid Gate"), Space(10)]
    
    public Transform spawnPosition;
    public GameObject car;
    public Transform enemyGang;

    private void Awake()
    {
        price = gatePrice;
    }
    public override void TriggerEnter(Collider other)
    {
     
        if (other.GetComponent<PlayerParent>())
        {
           
           
            base.TriggerEnter(other);
            //print("raid gate");

            
            var a = Instantiate(car, spawnPosition.position + new Vector3( 0f , 20f , 0f), Quaternion.identity);
            a.transform.DOMove(spawnPosition.position, 0.3f).OnComplete((() =>
            {
                //a.GetComponentInChildren<ParticleSystem>().Play();
                a.transform.DOMove(spawnPosition.position + new Vector3(0f, 0.7f, 0f), 0.05f)
                    .SetLoops(2, LoopType.Yoyo).OnComplete((() =>
                    {
                        a.transform.DOMove(new Vector3(a.transform.position.x, a.transform.position.y, enemyGang.position.z), 0.5f);
                    }));
                a.transform.DOShakeRotation(0.2f, 10f, 2);
            }));
           
        }
    }

    public override void CalculatePrice()
    {
        // Çetedeki her adam için 7 x adamların leveli kadar
        price = 7 * gangManager.transform.childCount;
        base.CalculatePrice();
    }
}
