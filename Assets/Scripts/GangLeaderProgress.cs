using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GangLeaderProgress : MonoBehaviour
{
    public GameObject progressBar;
    public Progress[] progress;
 
    public TMP_Text progressText;
    public Image fill;

    [Serializable]
    public struct Progress
    { 
        public string name;
        public GameObject obje; 
    }
    
    
    private GangManager _gangManager;
    private int _tempProgressLevel = 0;
    
    
    void Start()
    {
        _gangManager = FindObjectOfType<GangManager>();
        progressText.text = "thug";
    }

    public int ProgressLevel()
    {
        int moneySpent = Player.Instance.MoneySpentAmountAtCurrentLevel;
        if (moneySpent < 300)
        {
            return 0;
        }
        else if ( moneySpent < 500)
        {
            return 1;
        }
        else if (moneySpent < 700)
        {
            return 2;
        } 
        else if (moneySpent < 1000)
        {
            return 3;
        } 
        else if (moneySpent < 1500)
        {
            return 4;
        }
        else
        {
            return 5;
        }
        
    }

    public void CheckProgressAndChangeVisual()
    {
        if (_tempProgressLevel != ProgressLevel())
        {
            ChangeGangLeaderVisual();
            FillProgressBar(ProgressLevel());
            _tempProgressLevel = ProgressLevel();
            progressText.text = progress[_tempProgressLevel - 1].name;
            if (_tempProgressLevel != 1)
            {
                progressText.transform.DOScale(1.5f , 0.5f).SetLoops(2 , LoopType.Yoyo); 
            }
           
        }
    }
    
    private void ChangeGangLeaderVisual()
    {
        switch (ProgressLevel())
            {
                case 0:
                    Console.Write("progress level = " + 0);
                    break;
                case 1:
                    Console.Write("progress level = " + 1);
                    GetKnife();
                    break;
                case 2:
                    Console.Write("progress level = " + 2);
                    GetGun();
                    break;
                case 3:
                    Console.Write("progress level = " + 3);
                    GetGun();
                    WearHat();
                    break;
                case 4:
                    Console.Write("progress level = " + 4);
                    GetGun();
                    WearHat();
                    WearTopcoat();
                    break;
                case 5:
                    Console.Write("progress level = " + 5);
                    GetGun();
                    WearHat();
                    WearTopcoat();
                    GetPitbull();
                    break;
            }

    }
    private void GetKnife()
    { 
       // print("GetKnife");
        progress[0].obje.SetActive(true);
    } 
    public void GetGun()
    {
        //print("GetGun");
        progress[0].obje.SetActive(false);
        progress[1].obje.SetActive(true);
    }

    private void WearHat() 
    { 
        //print("WearHat");
        progress[2].obje.SetActive(true);
    }
    private void WearTopcoat()
    { 
        //print("WearTopcoat");
        progress[3].obje.SetActive(true);
    }
    private void GetPitbull()
    {
       // print("GetPitbull");
       var obj = Instantiate(progress[4].obje, transform.position - new Vector3(-3f, 0f, 3f),
           progress[4].obje.transform.rotation);
       
    }

    private void FillProgressBar(int index)
    {
        // for (int i = 0; i < index; i++)
        // {
        //     progressBar.transform.GetChild(i).gameObject.SetActive(true);
        // }

        fill.fillAmount = (index) * 0.2f;
    }
}
