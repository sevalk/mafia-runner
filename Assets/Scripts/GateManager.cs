using System;
using System.Collections;
using System.Collections.Generic;
using udoEventSystem;
using UnityEngine;

public class GateManager : MonoBehaviour
{
    public static GateManager Instance;
    public GameObject multiplyerGate;
    [HideInInspector] public List<Gate> _gates;

    private int _length;
    private int _index;

    public void GatePassed()
    {
        //_index++;
    } 
    
    private void OnEnable()
    {
        EventManager.Get<AllGangMembersDied>().AddListener(CreateMultiplyerGate);
          
    }
    
    private void OnDisable()
    {
        EventManager.Get<AllGangMembersDied>().RemoveListener(CreateMultiplyerGate);
           
    }
   
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
        foreach (var VARIABLE in FindObjectsOfType<Gate>())
        {
            _gates.Add(VARIABLE);
        }
       // _gates = FindObjectsOfType<Gate>();
        _length = _gates.Count;
        SortGatesByZAxis();
    }

    private void SortGatesByZAxis()
    {
        Gate _temp;
        for(int i=0; i<_length-1;i++){ 
            for(int j=i+1; j<_length; j++){
                if(_gates[i].transform.position.z > _gates[j].transform.position.z){
                    _temp = _gates[i]; 
                    _gates[i] = _gates[j];
                    _gates[j] = _temp; 
                }
            }
        }
 
        // for(int i=0; i<_length; i++)
        //     print(_gates[i]);
    }

    private void CreateMultiplyerGate()
    {
           _index = 0;
           for(int i=0; i<_length-1;i++) 
           {
                if(_gates[i].transform.position.z > Player.Instance.transform.position.z)
                {
                    break;
                }
                _index++;
                _index = Mathf.Clamp(_index, 0, _length - 2);
           }
           
           GameObject newGate = Instantiate(multiplyerGate.gameObject, _gates[_index + 1].transform.position , Quaternion.identity);
           newGate.SetActive(true);
           newGate.GetComponent<Gate>().CalculatePrice();
           if (_gates[_index + 1].gameObject)
           {
               _gates[_index+1].gameObject.SetActive(false);
           }
          
        }

}

