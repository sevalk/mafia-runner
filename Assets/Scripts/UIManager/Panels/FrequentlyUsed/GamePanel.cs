using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : Panel
{
    [Header("Game Panel Properties")]
    [SerializeField] public TMP_Text levelText;
    [SerializeField] private TMP_Text _moneyText;
    
    private void OnEnable()
    {
        // levelText.text = "Level" + SaveManager.Instance.state.levelCounter;
    }

    public void UpdateMoneyText(int moneyAmount)
    {
        _moneyText.text = moneyAmount.ToString();
    }
}
