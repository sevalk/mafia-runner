using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform pivot;
    public ParticleSystem explosion;


    public void Fire(Transform target)
    {
        var bullet = PoolManager.Instance.Spawn(Pools.Types.Bullet, pivot.position, pivot.rotation);
        bullet.transform.DOMove(target.position + new Vector3(0f,2.5f,0f), 0.3f).OnComplete((() => PoolManager.Instance.Despawn(Pools.Types.Bullet,bullet)));
        explosion.Stop();
        explosion.Play();
        
    }
}
