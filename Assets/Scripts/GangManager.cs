using System;
using System.Collections;
using System.Collections.Generic;
using AI.Combat;
using AI.Control;
using AI.Core;
using DG.Tweening;
using TMPro;
using udoEventSystem;
using UnityEngine;
using UnityEngine.AI;

public class GangManager : MonoBehaviour
{
    public static GangManager Instance;
    private GameObject _paint;
    
    private int _gangMemberLevel;
    public GameObject enemyBoss;
    public TMP_Text gangLevelText;

    public GangData[] gangData; 
    public GangData[] enemyGangData;

    [HideInInspector] public List<GameObject> pinkAIs;

    [HideInInspector] public bool isEnemyBossDied = false;

    private float tempZ;
    private float tempX;
    private bool t = true;

    private GameObject painttest;
    public int GetGangLevel()
    {
        return _gangMemberLevel;
    }

    public void SetGangLevel(int level)
    {
        //bunu visual olarak nasıl vereceğiz?
        _gangMemberLevel = level;
        gangLevelText.text = "Level " + level.ToString() + " Gang";
        gangLevelText.GetComponentInChildren<ParticleSystem>().Play();

        gangLevelText.transform.DOScale(Vector3.one * 2f, 0.2f).SetLoops(2 , LoopType.Yoyo);
        
        foreach (Transform child in PoolManager.Instance.pools[0].pool.transform)
        {
           // child.GetComponent<BlueAIController>().SetLevelText(_gangMemberLevel);
            child.GetComponent<Health>().SetHealth(gangData[_gangMemberLevel - 1].health);
            child.GetComponent<Fighter>().SetWeaponDamage(gangData[_gangMemberLevel - 1].attackPower);
        }
        
        foreach (Transform child in transform)
        {
           // child.GetComponent<BlueAIController>().SetLevelText(_gangMemberLevel);
            child.GetComponent<Health>().SetHealth(gangData[_gangMemberLevel - 1].health);
            child.GetComponent<Fighter>().SetWeaponDamage(gangData[_gangMemberLevel - 1].attackPower);
        }
        
        //  GateManager.Instance.UpdateMultiplyerGatesPrice();
      EventManager.Get<GatePriceCalculated>().Execute();
        
    }

    public void SetEnemyGangLevel(PinkAIController pinkAI , int level)
    {
       // pinkAI.GetComponent<PinkAIController>().SetLevelText(level);
        level = Mathf.Clamp(level, 1, 5);
        pinkAI.GetComponent<Health>().SetHealth(enemyGangData[level - 1].health);
        pinkAI.GetComponent<Fighter>().SetWeaponDamage(enemyGangData[level - 1].attackPower);
    }

   
    [Serializable]
    public struct GangData
    {
        public string name;
        public int health;
        public int attackPower;
        
    }
    
    // private void OnEnable()
    // {
    //     EventManager.Get<AllGangMembersDied>().AddListener(SetGangManagerLevelTextInactive);
    // }
    //
    // private void OnDisable()
    // {
    //     EventManager.Get<AllGangMembersDied>().RemoveListener(SetGangManagerLevelTextInactive);   
    // }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
    }
    private void Start()
    {
        SetGangLevel(1);
        pinkAIs.Add(enemyBoss);
        _paint = Player.Instance.paint;
    }

    private void Update()
    { 
        if (t && transform.childCount > 0)
        {
            t = false;
            tempZ = 0;
            tempX = 0;
            foreach (Transform child in transform)
            {
                tempZ += child.position.z;
                tempX += child.position.x;
            }
            float ortZ = tempZ / transform.childCount;
            float ortX = tempX / transform.childCount;
            gangLevelText.transform.position = new Vector3(ortX, 5.21f, ortZ);
            gangLevelText.gameObject.SetActive(true);
            if (!painttest)
            {
                painttest =  Instantiate(_paint , _paint.transform.position , _paint.transform.rotation , _paint.transform.parent);
                painttest.SetActive(true);
            }
            
            t = true;
        }
        
    }

    public void SetGangManagerLevelTextInactive()
    {
        gangLevelText.gameObject.SetActive(false);
        if(painttest)
        {
             painttest.SetActive(false);
             painttest = null;}
    }

    public bool IsAllGangMemberDead()
    {
        return transform.childCount == 0;
    }

    
    public void DeactivateAllGangMembers(List<Transform> points)
    {
        // foreach (Transform child in this.transform)
        // {
        //     // child.GetComponent<BlueAIController>().enabled = false;
        //     // child.GetComponent<Animator>().SetBool("idle", true);
        //     this.InvokeAfterSeconds(1f, () =>
        //     {
        //         child.gameObject.SetActive(false);
        //     });
        //     
        // }
        gangLevelText.gameObject.GetComponent<TMP_Text>().enabled = false;
        int cc = transform.childCount;
        int i = 0;
        foreach (var point in points)
        {
            if (i >= cc)
            {
                break;
            }
            var child = transform.GetChild(i).GetComponent<BlueAIController>();
            child.navMeshAgent.enabled = false;
            child.enabled = false;
            child.transform.DOMove(point.position, 2f).SetEase(Ease.Linear).OnComplete((() =>
            { 
                
                child.anim.SetBool("cheer" , true);
            }));
            this.InvokeAfterSeconds(1.5f, () =>
            {
                child.transform.DORotate(new Vector3(0f, 180f, 0f), 0.5f);
            });
           
            i++;
        }
        

        for (int j = i; j < transform.childCount; j++)
        {
            transform.GetChild(j).gameObject.SetActive(false);
        }
    }
}
