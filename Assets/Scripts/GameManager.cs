using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SplineFollower splineFollower;
    [HideInInspector]public PlayerParent PlayerParent;
    [HideInInspector] public Animator characterAnim;

    public GameObject path1;
    public GameObject path2;

    private GangLeaderProgress _gangLeaderProgress;
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
       LevelManager.Instance.SetLevelText(); 
       UIManager.Instance.ShowPanel(Panel.Type.Main);
       
       PlayerParent = FindObjectOfType<PlayerParent>();
       _gangLeaderProgress = PlayerParent.GetComponent<GangLeaderProgress>();
       
       characterAnim = PlayerParent.transform.GetChild(0).GetChild(0).GetComponent<Animator>();

       splineFollower.onEndReached += OnEndReached;
       
       
    }

    public void UpdateFollower()
    {
        // SaveManager.Instance.SetTotalFollowerAmount(SaveManager.Instance.GetTotalFollowerAmount()
        //                                             + Mathf.CeilToInt(UIManager.Instance.gamePanel.fameSlider.value * 10000f));
    }

    void OnEndReached(double f)
    {
        characterAnim.transform.DORotate(new Vector3(0f, 180f, 0f), 0.5f);
        characterAnim.SetBool("sit" , true);
        _gangLeaderProgress.progress[4].obje.GetComponent<NavMeshAgent>().enabled = false;
        _gangLeaderProgress.progress[4].obje.GetComponent<Animator>().SetBool("isRunning" , false);
    }

    
    public void Paint()
    {
        var rend1 = path1.GetComponent<Renderer>();
        var rend2 = path2.GetComponent<Renderer>();
        DOTween.To(()=>rend1.material.GetFloat("Origin"),
            x => rend1.material.SetFloat("Origin", x), 0.5f, 160f /splineFollower.followSpeed ).SetEase(Ease.Linear);
        DOTween.To(()=>rend2.material.GetFloat("Origin"),
            x => rend2.material.SetFloat("Origin", x), 0.5f, 160f /splineFollower.followSpeed ).SetEase(Ease.Linear);
        print(200f /splineFollower.followSpeed);
    }
  

}
