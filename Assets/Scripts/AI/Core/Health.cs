using System;
using System.Collections;
using System.Collections.Generic;
using AI.Control;
using DG.Tweening;
using udoEventSystem;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Core
{
    public class Health : MonoBehaviour
    {
        private GangManager _gangManager;
        [SerializeField] private float healthPoints = 100f;

        private void Start()
        {
            _gangManager = FindObjectOfType<GangManager>();
        }


        public float GetHealth()
        {
            return healthPoints;
        }

        public void SetHealth(float health)
        {
            healthPoints = health;
        }

        private bool _isDead = false;

        public bool IsDead()
        {
            return _isDead;
        }


        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);

            if (healthPoints == 0)
            {
                Die();
                if (gameObject.activeSelf)
                {
                    this.InvokeAfterSeconds(3f, () => { gameObject.SetActive(false); });
                }

               
                
            }
        }
        

        private void Die()
        {
            if (_isDead) return;

            GetComponent<NavMeshAgent>().enabled = false;

            if (GetComponent<BlueAIController>())
            {
                transform.parent = PoolManager.Instance.pools[0].pool.transform;

                EventManager.Get<GatePriceCalculated>().Execute();
                if (_gangManager.IsAllGangMemberDead())
                {
                    _gangManager.gangLevelText.transform.localPosition = new Vector3(0f, 0f, 0f);
                    _gangManager.SetGangManagerLevelTextInactive();
                    if (Player.Instance.isEndGame)
                    {
                        //GameManager.Instance.splineFollower.follow = false;
                    }
                    else
                    {
                        EventManager.Get<AllGangMembersDied>().Execute();
                    }
                }
            }
            else if (GetComponent<PinkAIController>())
            {
                _gangManager.pinkAIs.Remove(gameObject);
                if (!Player.Instance.isEndGame)
                {
                    var money = PoolManager.Instance.Spawn(Pools.Types.Money,
                        transform.position,
                        Quaternion.identity);
                    money.GetComponent<MoneyCollectible>().amount = GetComponent<PinkAIController>().moneyAmount;
                    money.transform.DOJump(new Vector3(transform.position.x + 10f, 0.432f, transform.position.z + 5f),
                        5f, 1, 0.5f);
                    money.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                }
            }


            if (gameObject.CompareTag("enemyBoss"))
            {
                _gangManager.pinkAIs.Remove(gameObject);
                _gangManager.isEnemyBossDied = true;
                EventManager.Get<LevelCompleted>().Execute();
            }

            GetComponent<Animator>().SetTrigger("death");
            _isDead = true;
            GetComponent<ActionSchedular>().CancelCurrentAction();
        }
    }
}