using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerParent>())
        {
            TriggerEnter(other);
            transform.DOScale(0.1f, 0.1f).OnComplete((() =>
            {
                Destroy(gameObject);
            }));
        }
       
    }

    public virtual void TriggerEnter(Collider other)
    {
       
    }
}
