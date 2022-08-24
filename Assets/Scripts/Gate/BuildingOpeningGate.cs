using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BuildingOpeningGate : Gate
{
    [Header("Gate"), Space(10)]
    public Transform spawnPosition;
    public GameObject building;

    public ParticleSystem particle;
    private void Awake()
    {
        price = gatePrice;
    }

    public override void TriggerEnter(Collider other)
    {
     
        if (other.GetComponent<PlayerParent>())
        {
            particle.Play();
            var a = Instantiate(building,transform.position, building.transform.rotation);
            a.transform.localScale = Vector3.one * 0.2f;

            a.transform.DOScale(Vector3.one, 1.5f);
            this.InvokeAfterSeconds(0.02f, () =>
            {
                base.TriggerEnter(other);
                a.transform.DOJump(spawnPosition.position, 15f , 1 , 1.5f).OnComplete((() =>
                {
                    VibrateManager.Instance.ContiniusHaptic();
                    a.GetComponentInChildren<ParticleSystem>().Play();
                    a.transform.DOMove(spawnPosition.position + new Vector3(0f, 0.7f, 0f), 0.2f)
                        .SetLoops(2, LoopType.Yoyo);
                    a.transform.DOShakeRotation(1f, 6f, 3);
                }));
            });
           
            
          
            
        }
    }
    
    public override void CalculatePrice()
    {
        base.CalculatePrice();
    }
}
