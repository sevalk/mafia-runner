using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class PaintBuild : MonoBehaviour
{

    [SerializeField] private Material buildingMaterial;
    [SerializeField] private float offsetZ;


    private void OnDisable()
    {
        buildingMaterial.SetFloat("_Shift",0);
    }
    private void OnApplicationQuit()
    {
        buildingMaterial.SetFloat("_Shift",0);
    }

    private void FixedUpdate()
    {
        buildingMaterial.SetFloat("_Shift",offsetZ+transform.position.z);
    }
}
