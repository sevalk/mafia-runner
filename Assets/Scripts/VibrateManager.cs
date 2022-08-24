using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class VibrateManager : MonoBehaviour
{
    public static VibrateManager Instance;
    public GameObject HapticStopButton;
    public GameObject HapticStartButton;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    


    public void Vibrate( HapticTypes type)
    {
        MMVibrationManager.Haptic (type);
    }

    public void StopAllHaptics()
    {
        MMVibrationManager.StopAllHaptics(true);
    }
    
    public void TurnOnAllHaptics()
    {
        // turns all haptics on
        MMVibrationManager.SetHapticsActive(true);
        HapticStopButton.SetActive(true);
        HapticStartButton.SetActive(false);
        SaveManager.Instance.SetHapticState(true);
    }
    public void TurnOffHaptics()
    {
        // turns all haptics off
        MMVibrationManager.SetHapticsActive(false);
        HapticStopButton.SetActive(false);
        HapticStartButton.SetActive(true);
        SaveManager.Instance.SetHapticState(true);
    }

    public void ContiniusHaptic()
    {
        // MMVibrationManager.ContinuousHaptic(0.3f, 0.7f, 5f, 
        //     HapticTypes.None, this, true);
        
        MMVibrationManager.ContinuousHaptic(0.5f, 0.9f, 0.7f, 
            HapticTypes.MediumImpact, this, true);

    }
    public void TransientHaptic()
    {
      
      MMVibrationManager.TransientHaptic(0.7f, 5f);
       
    }
    
    public void TeleportHaptic()
    {
        MMVibrationManager.AdvancedHapticPattern("NVTeleport", 
            null, null, -1, null, 
            null, null, -1, 
            HapticTypes.LightImpact); 

    }

    
    public void StopContinuousHaptics()
    {
        MMVibrationManager.StopContinuousHaptic();
    }
   


}