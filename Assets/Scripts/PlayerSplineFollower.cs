using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using udoEventSystem;
using UnityEngine;

public class PlayerSplineFollower : MonoBehaviour
{
    private Transform _gangText;
    private SplineFollower _splineFollower;

    private bool _control = true;

    private Tween _increaseSpeed;
    private Tween _decreaseSpeed;

   
    
    private void Start()
    {
        _gangText = GangManager.Instance.gangLevelText.transform;
        _splineFollower = GetComponent<SplineFollower>();
    }

   
    private void KillTweens()
    {
        DOTween.Kill(_decreaseSpeed);
        DOTween.Kill(_increaseSpeed);
        _decreaseSpeed = null;
        _increaseSpeed = null;
    }

   
}
