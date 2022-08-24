using System.Collections;
using System.Collections.Generic;
using udoEventSystem;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public delegate void OpenPanelDelegate(Panel.Type panelType);

    public OpenPanelDelegate OpenedPanel;

    public Panel lockUIPanel, emptyPanel;
    public GamePanel gamePanel;
    public LevelFailedPanel failPanel;
    public LevelCompletedPanel levelCompletedPanel;
    public CurrencyPanelTest currencyTestPanel;
    public MainPanel mainPanel;
    public LeaderBoardController leaderboard;

    Panel topPanelOrPopup;
    Stack<Panel> previousPanels;
    Panel currentPanel;
    Panel lastShownPanel;

    public Panel.Type panelTypeForShowPanelTest;


    private void OnEnable()
    {
        EventManager.Get<LevelFailed>().AddListener(LevelFailed);
        EventManager.Get<LevelStarted>().AddListener(LevelStarted);
    }

    private void OnDisable()
    {
        EventManager.Get<LevelFailed>().RemoveListener(LevelFailed);
        EventManager.Get<LevelStarted>().RemoveListener(LevelStarted);
    }


  

    private void LevelStarted()
    {
      
    }
    
    private void LevelFailed()
    {
        ShowPanel(Panel.Type.FailedLevel);
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

        previousPanels = new Stack<Panel>();
    }

    public Panel.Type GetTopPanelOrPopupType()
    {
        if (topPanelOrPopup != null)
        {
            return topPanelOrPopup.panelType;
        }
        else
        {
            return Panel.Type.None;
        }
    }

    public Panel.Type GetCurrentPanelType()
    {
        if (currentPanel != null)
        {
            return currentPanel.panelType;
        }
        else
        {
            return Panel.Type.None;
        }
    }

    public void HidePanel()
    {
        currentPanel.isCurrentlyOpen = false;
        currentPanel.gameObject.SetActive(false);
    }

    public Panel.Type GetLastPanelType()
    {
        if (lastShownPanel != null)
        {
            return lastShownPanel.panelType;
        }
        else
        {
            return Panel.Type.None;
        }
    }

    public Panel.Type GetFirstPreviousPanelType()
    {
        if (previousPanels.Count > 0)
        {
            return previousPanels.Peek().panelType;
        }
        else
        {
            return Panel.Type.None;
        }
    }

    Panel tempPanel;

    public void ShowPanel(Panel.Type panelType, bool isPopup = false, bool removePreviousPanels = false)
    {
        if (removePreviousPanels)
        {
            previousPanels.Clear();
        }

        tempPanel = SelectPanel(panelType);
        if (tempPanel.isAlwaysPopup)
        {
            tempPanel.isCurrentlyPopup = true;
        }
        else
        {
            tempPanel.isCurrentlyPopup = isPopup;
        }

        if (currentPanel == null)
        {
            tempPanel.isCurrentlyPopup = false;
        }

        if (!tempPanel.isCurrentlyPopup)
        {
            currentPanel = tempPanel;
        }

        if (topPanelOrPopup != null)
        {
            if (topPanelOrPopup.isCurrentlyPopup || !tempPanel.isCurrentlyPopup)
            {
                topPanelOrPopup.isCurrentlyOpen = false;
                topPanelOrPopup.gameObject.SetActive(false);
            }


            if (!removePreviousPanels)
            {
                previousPanels.Push(topPanelOrPopup);
            }

            if (topPanelOrPopup.filterImage != null)
            {
                topPanelOrPopup.filterImage.SetActive(false);
            }
        }

        lastShownPanel = topPanelOrPopup;
        topPanelOrPopup = tempPanel;
        topPanelOrPopup.gameObject.SetActive(true);
        topPanelOrPopup.isCurrentlyOpen = true;

        if (topPanelOrPopup.filterImage != null)
        {
            topPanelOrPopup.filterImage.SetActive(true);
        }

        if (OpenedPanel != null)
        {
            OpenedPanel(topPanelOrPopup.panelType);
        }
    }

    public void AddToPreviousPanels(Panel.Type panelType, bool isPopup = false, bool removePreviousPanels = false)
    {
        if (removePreviousPanels)
        {
            previousPanels.Clear();
        }

        tempPanel = SelectPanel(panelType);
        if (tempPanel.isAlwaysPopup)
        {
            tempPanel.isCurrentlyPopup = true;
        }
        else
        {
            tempPanel.isCurrentlyPopup = isPopup;
        }

        if (previousPanels.Count == 0)
        {
            tempPanel.isCurrentlyPopup = false;
        }

        previousPanels.Push(tempPanel);
    }

    //public void AddAsFirstPreviousPanel(Panel.Type panelType, bool isPopup = false)
    //{
    //}

    //public void RemoveAllPreviousPanels()
    //{
    //    previousPanels.Clear();
    //    if (topPanelOrPopup != null && topPanelOrPopup.isCurrentlyPopup)
    //    {
    //        previousPanels.Push(currentPanel);
    //    }

    //    //Debug.Log("RemoveAllPreviousPanels prevPanelsCurrentCount: " + previousPanels.Count);
    //}

    Stack<Panel> tempStack;

    public void RemoveFromPreviousPanels(Panel.Type panelTypeToRemove)
    {
        if (tempStack == null)
        {
            tempStack = new Stack<Panel>();
        }
        else
        {
            tempStack.Clear();
        }

        while (previousPanels.Count > 0)
        {
            tempPanel = previousPanels.Pop();
            if (tempPanel.panelType != panelTypeToRemove)
            {
                tempStack.Push(tempPanel);
            }
        }

        while (tempStack.Count > 0)
        {
            previousPanels.Push(tempStack.Pop());
        }

        //Debug.Log("RemoveFromPreviousPanels " + panelTypeToRemove);
    }

    public Panel.Type ReturnToPreviousPanel()
    {
        if (previousPanels.Count > 0)
        {
            if (topPanelOrPopup.filterImage != null)
            {
                topPanelOrPopup.filterImage.SetActive(false);
            }

            lastShownPanel = topPanelOrPopup;
            topPanelOrPopup.gameObject.SetActive(false);
            topPanelOrPopup = previousPanels.Pop();
            topPanelOrPopup.gameObject.SetActive(true);
            if (!topPanelOrPopup.isCurrentlyPopup)
            {
                currentPanel = topPanelOrPopup;
            }

            if (topPanelOrPopup.filterImage != null)
            {
                topPanelOrPopup.filterImage.SetActive(true);
            }

            if (OpenedPanel != null)
            {
                OpenedPanel(topPanelOrPopup.panelType);
            }

            //Debug.Log("ReturnToPreviousPanel " + topPanelOrPopup.panelType + " " + topPanelOrPopup.isCurrentlyPopup);

            return topPanelOrPopup.panelType;
        }
        else
        {
            //Debug.Log("ReturnToPreviousPanel " + Panel.Type.None);

            return Panel.Type.None;
        }
    }

    public Panel.Type ReturnToPreviousPanelWithDelay(float delay)
    {
        StartCoroutine(ReturnToPreviousPanelCoroutine(delay));
        if (previousPanels.Count > 0)
        {
            return previousPanels.Peek().panelType;
        }
        else
        {
            return Panel.Type.None;
        }
    }

    IEnumerator ReturnToPreviousPanelCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPreviousPanel();
    }

    private Panel SelectPanel(Panel.Type panelType)
    {
        switch (panelType)
        {
            case Panel.Type.LockUI:
                tempPanel = lockUIPanel;
                break;
            case Panel.Type.Empty:
                tempPanel = emptyPanel;
                break;
            case Panel.Type.Game:
                tempPanel = gamePanel;
                break;
            case Panel.Type.FailedLevel:
                tempPanel = failPanel;
                break;
            case Panel.Type.LevelCompleted:
                tempPanel = levelCompletedPanel;
                break;
            case Panel.Type.Test:
                tempPanel = currencyTestPanel;
                break;
            case Panel.Type.Main:
                tempPanel = mainPanel;
                break;
            case Panel.Type.Leaderboard:
                tempPanel = leaderboard;
                break;
        }

        return tempPanel;
    }

    [ContextMenu("Show Panel Test")]
    private void ShowPanelTest()
    {
        ShowPanel(panelTypeForShowPanelTest);
    }

    //public void ToggleNoVideoLeftBanner(bool toggle)
    //{
    //    if(!noVideoLeftPanel.gameObject.activeInHierarchy)
    //        noVideoLeftPanel.gameObject.SetActive(toggle);
    //}
}