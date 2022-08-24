using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Dreamteck.Splines;
using Dreamteck.Splines.Primitives;
using udoEventSystem;

public class MainPanel : Panel
{
    
      
    public SplineFollower follower;

    public GameObject path;

    private Renderer rendSpendArea;

    private GameManager _gameManager;
    
    //private SplineSample _splineSample;
    
    void Start()
    {
        //
        // _gameManager = FindObjectOfType<GameManager>();
        // follower = GameManager.Instance.splineFollower;
        // // path = follower.spline.transform.GetChild(0).gameObject;
        // rendSpendArea = path.GetComponent<Renderer>();
    }
    
    public void TapToStartClicked()
    {
     
        UIManager.Instance.HidePanel();
        GameManager.Instance.splineFollower.follow = true;
        EventManager.Get<LevelStarted>().Execute();
        GameManager.Instance.characterAnim.SetBool("towalk", true);
       // GameManager.Instance.Paint();
    //     DOTween.To(()=>rendSpendArea.material.GetFloat("Origin"),
    //         x => rendSpendArea.material.SetFloat("Origin", x), 0.5f, 160f /_gameManager.splineFollower.followSpeed );
    
    }
}