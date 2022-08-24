using  System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using udoEventSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class MultiplyerGate : Gate
{
    [Header("Multiplyer Gate"), Space(10)]
    public int count;
    public State state;
    private int _manCount;

    public GameObject spawn;
    public ParticleSystem spawnParticle;
 
    public enum State
    {
        Add,
        Multiply
    }

    private void Awake()
    {
        if (useGatePrice)
        {
            price = gatePrice;
        }
        else
        {
            price = 0;
        }
       
        if (state == State.Add)
        {
            visual.GetChild(2).GetComponent<TMP_Text>().text = "+" + count.ToString();
        }
        else
        {
            visual.GetChild(2).GetComponent<TMP_Text>().text = "x" + count.ToString();
        }
    }
    
    // Lenslerden geçerken alıp çetemize eklediğmiz level 1 adamların birim fiyatı 50 dolar. Level 1 
    // üzerindeki her level için, +10 dolar daha bu fiyata ekleniyor
            
    // Lens çete sayısını bir sayı ile çarpıyorsa, bize eklenen adamların toplam bedelinin yarısı kadar para 
    //ödüyoruz.
    
    public override void TriggerEnter(Collider other)
    {
     
        if (other.GetComponent<PlayerParent>())
        {
            if (state == State.Add)
            {
                _manCount = count;
            }
            else
            { 
                _manCount = gangManager.transform.childCount * (count - 1);
            }
            base.TriggerEnter(other);
           
           // for (int i = 0; i < _manCount; i++)
           //  {
           //      
           //      var randomPosition = new Vector3(Random.Range(-2f,2f), 
           //          0f, Random.Range(-2f,5f) );
           //      // print(randomPosition);
           //  
           //  
           //      var newspawn = Instantiate(spawn, transform.position, spawn.transform.rotation);
           //
           //      newspawn.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuad);
           //      newspawn.transform.DOJump(Player.Instance.spawnPoint.position + randomPosition, 6f,1,0.7f).OnComplete((() =>
           //      {
           //          
           //          var blue = PoolManager.Instance.Spawn(Pools.Types.MafiaMemberBlue ,
           //              newspawn.transform.position  ,quaternion.identity , gangManager.transform);
           //          Destroy(newspawn);
           //
           //      }));
           //
           //     
           //  }
           //
           //  EventManager.Get<GatePriceCalculated>().Execute();
           StartCoroutine(SpawnMembers());
        }
    }

    
    
    public override void CalculatePrice()
    {
        if (!useGatePrice)
        {
            if (state == State.Add)
            {
                _manCount = count;
                price =( 50 + ((GangManager.Instance.GetGangLevel() - 1) * 10)) * _manCount;
            
            }
            else
            { 
                _manCount = GangManager.Instance.transform.childCount * (count - 1);
                price =( 50 + ((GangManager.Instance.GetGangLevel() - 1) * 10)) * _manCount / 2;
            }
            base.CalculatePrice();
        }
        
       
    }


    private IEnumerator SpawnMembers()
    {
        for (int i = 0; i < _manCount; i++)
        {
            
             
            var randomPosition = new Vector3(Random.Range(-2f,2f), 
                0f, Random.Range(-2f,5f) );
            // print(randomPosition);
            
            
            var newspawn = Instantiate(spawn, transform.position, spawn.transform.rotation);

            newspawn.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuad);
            newspawn.transform.DOJump(Player.Instance.spawnPoint.position + randomPosition, 6f,1,0.7f).OnComplete((() =>
            {
                    
                var blue = PoolManager.Instance.Spawn(Pools.Types.MafiaMemberBlue ,
                    newspawn.transform.position  ,quaternion.identity , gangManager.transform);
                Destroy(newspawn);

            }));


            yield return new WaitForSeconds(0.05f);
        }
        
        EventManager.Get<GatePriceCalculated>().Execute();
    }
}
