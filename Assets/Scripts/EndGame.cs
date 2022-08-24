using System;
using System.Collections;
using System.Collections.Generic;
using AI.Combat;
using AI.Control;
using AI.Core;
using Cinemachine;
using DG.Tweening;
using Dreamteck.Splines;
using udoEventSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePivot;
    public Transform target;
    private int _bulletCount = -1;
    public Transform enemyGang;
    public ParticleSystem confetti;


    public Image fill;
    public List<Transform> points;
    [HideInInspector] public GameObject endLine;

    private GangLeaderProgress _gangLeaderProgress;
    private ParticleSystem _explosion;
    private GameManager _gameManager;

    public CinemachineVirtualCamera endCam;

    private void OnEnable()
    {
        EventManager.Get<LevelCompleted>().AddListener(LevelCompleted);
    }

    private void OnDisable()
    {
        EventManager.Get<LevelCompleted>().RemoveListener(LevelCompleted);
    }

    private void Start()
    {
        _gangLeaderProgress = FindObjectOfType<GangLeaderProgress>();
        //  GameManager.Instance.splineFollower.onEndReached += OnEndReached;
        _explosion = firePivot.GetChild(0).GetComponent<ParticleSystem>();
        _gameManager = GameManager.Instance;
    }


    void OnEndReached(double f)
    {
        int i = 0;
        foreach (var point in points)
        {
            _gameManager.transform.GetChild(i).DOMove(point.position, 0.3f);
            i++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrailRenderer>())
        {
            other.transform.parent = null;
        }

        if (other.GetComponent<PlayerParent>())
        {
            // if (GangManager.Instance.transform.childCount <= 0 && _gameManager.PlayerParent.GetComponent<GangLeaderProgress>().fill.fillAmount < 0.2f)
            // {
            //     _gameManager.characterAnim.SetBool("idle", true);
            // }
            endCam.Priority = 15;
            _gameManager.splineFollower.followSpeed = 2f;
            _gameManager.characterAnim.speed = 0.5f;
            Player.Instance.isEndGame = true;
            other.GetComponent<SwipeMovement>().canMove = false;
            other.transform.DOMoveX(0f, 0.2f);
            DestroyMoneys();
            GetGangMembersToMainPath();
            Fire(other);
        }

        if (!Player.Instance.isEndGame && other.GetComponent<BlueAIController>())
        {
            endCam.Priority = 15;
            //  GameManager.Instance.splineFollower.followSpeed = 1f;
            Player.Instance.isEndGame = true;
        }
    }

    private static void GetGangMembersToMainPath()
    {
        Player.Instance.spawnPoint.DOMove(
            new Vector3(0f,
                Player.Instance.spawnPoint.position.y,
                Player.Instance.spawnPoint.position.z + 20f),
            0.2f);
    }

    private void DestroyMoneys()
    {
        Player.Instance.DestroyAllMoneys();
    }


    public void EnemySurrender()
    {
        foreach (Transform enemy in enemyGang)
        {
            if (enemy.gameObject.activeSelf)
            {
                var a = enemy.GetComponent<AIController>();
                if (!a.health.IsDead())
                {
                    GangManager.Instance.pinkAIs.Remove(enemy.gameObject);
                    enemy.GetComponent<PinkAIController>().enabled = false;
                    enemy.GetComponent<NavMeshAgent>().enabled = false;
                    enemy.GetComponent<Animator>().SetTrigger("cheer");

                   // a.health.TakeDamage(a.health.GetHealth() + 10f);
                    // a.anim.SetBool("run",false);
                    // a.anim.SetBool("death",true);
                    //
                    // a.enabled = false;
                }
            }

           
        }
    }

    // private bool isAllEnemiesDead()
    // {
    //     foreach (Transform enemy in enemyGang)
    //     {
    //         if (!enemy.GetComponent<Health>().IsDead())
    //         {
    //             return false;
    //         }
    //        
    //     }
    //     return true;
    // }

    public void Fire(Collider other)
    {
        _gangLeaderProgress.GetGun();
        StartCoroutine(FireRoutine(other));
    }

    private IEnumerator FireRoutine(Collider other)
    {
        _bulletCount = _gangLeaderProgress.ProgressLevel();

        if (_bulletCount > 0)
        {
            _gameManager.characterAnim.SetBool("PlayerSmithweesonForwardWalk", true);
        }


        int temp = _bulletCount;
        int temp2 = _bulletCount;
        for (int i = 0; i < temp; i++)
        {
            // yield return new WaitWhile(() => !GameManager.Instance.characterAnim.fireEvents);
           
            _gameManager.splineFollower.followSpeed = 0;
            yield return new WaitForSeconds(0.7f);
            _gangLeaderProgress.progressBar.transform.GetChild(temp2 - 1).gameObject.SetActive(false);
            temp2--;
            
           
            GameObject _bullet = Instantiate(bullet, firePivot.position, Quaternion.identity);
            _explosion.Stop();
            _explosion.Play();
            _bullet.transform.DOMove(target.position, 0.5f).OnComplete((() => { Destroy(_bullet); }));

            fill.fillAmount -= 0.2f;
            if (GangManager.Instance.isEnemyBossDied)
            {
                break;
            }
        }

        _gameManager.splineFollower.followSpeed = 2;
        _gameManager.characterAnim.speed = 0.5f;
        _gameManager.characterAnim.SetBool("towalk", true);
        _gangLeaderProgress.progressBar.gameObject.SetActive(false);
        _gangLeaderProgress.progressText.gameObject.SetActive(false);

        if (_bulletCount == 5)
        {
            //enemy gang boss die
            GangManager.Instance.isEnemyBossDied = true;
            LevelCompleted();
        }
        else
        {
            yield return new WaitWhile(() => (!GangManager.Instance.IsAllGangMemberDead()));

            LevelCompleted();
            Renderer rend = endLine.GetComponent<Renderer>();
            rend.enabled = true;
            rend.material.DOColor(Color.cyan, 0.2f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public bool AreTheBulletsGone()
    {
        //return _bulletCount == 0;
        return true;
    }

    private void LevelCompleted()
    {
        if (GangManager.Instance.isEnemyBossDied)
        {
            fill.gameObject.SetActive(false);
            target.GetChild(0).GetComponent<Animator>().SetBool("death", true);
            _gameManager.characterAnim.SetBool("towalk", true);
            _gameManager.splineFollower.followSpeed = 15;
            _gameManager.characterAnim.speed = 1.3f;
            EnemySurrender();
            GangManager.Instance.DeactivateAllGangMembers(points);
            
            this.InvokeAfterSeconds(1f, () => { //enemyGang.gameObject.SetActive(false);
                                                });
            UIManager.Instance.ShowPanel(Panel.Type.LevelCompleted);
            UIManager.Instance.levelCompletedPanel._GodfatherText.transform.parent.gameObject.SetActive(true);
            this.InvokeAfterSeconds(5f, () =>
            {
                UIManager.Instance.levelCompletedPanel._GodfatherText.transform.parent.gameObject.SetActive(false);
                UIManager.Instance.ShowPanel(Panel.Type.Leaderboard , true , false);
                
                UIManager.Instance.levelCompletedPanel.CreateFlyingCurrencies(CurrencyAmountHolderID.Money,
                    CurrencyAnimationType.Counter,0
                    , (holder, value) =>
                    {
                        
                    });

            });
        }
        else
        {
            _gameManager.splineFollower.follow = false;
            _gameManager.characterAnim.SetBool("idle", true);
            EnemySurrender();
            UIManager.Instance.ShowPanel(Panel.Type.LevelCompleted);
            UIManager.Instance.levelCompletedPanel._CaseFiredText.transform.parent.gameObject.SetActive(true);
            this.InvokeAfterSeconds(2f,
                () => { UIManager.Instance.levelCompletedPanel.tapToContinueButton.gameObject.SetActive(true); });
        }

        confetti.Play();
    }
}