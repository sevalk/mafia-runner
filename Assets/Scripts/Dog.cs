using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dog : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Player _player;
    private Animator _anim;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        
        _player = Player.Instance;
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_agent.enabled)
        {
            _agent.SetDestination(_player.transform.position);

            //todo seval

            _anim.SetFloat("Blend", _agent.velocity.magnitude/ 16f);


        }
       
    }
}
