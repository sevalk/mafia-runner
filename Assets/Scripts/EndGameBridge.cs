using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameBridge : MonoBehaviour
{
    private EndGame _endGame;

    private void Start()
    {
        _endGame = GetComponentInParent<EndGame>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerParent>())
        {
            Player.Instance.endingBridgeLevel++;
            _endGame.endLine = gameObject;
        }
    }
}
