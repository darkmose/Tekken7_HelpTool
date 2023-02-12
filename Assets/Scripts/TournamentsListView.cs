using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentsListView : MonoBehaviour
{
    [SerializeField] private GameObject _windowRootObject;
    [SerializeField] private Transform _contentRoot; 
    [SerializeField] private Text _navigationStatus;
    [SerializeField] private Button _navigationNext;
    [SerializeField] private Button _navigationPrev;
    private Action<int> _tournamentElementCallback;

    public void InitButtonsCallbacks(Action nextButton, Action prevButton, Action<int> tournamentElementCallback)
    {
        _navigationNext.onClick.RemoveAllListeners();
        _navigationPrev.onClick.RemoveAllListeners();
        _navigationNext.onClick.AddListener(()=>nextButton?.Invoke());
        _navigationPrev.onClick.AddListener(()=>prevButton?.Invoke());
        _tournamentElementCallback = tournamentElementCallback;
    }

    public void PrepareList(List<TournamentDataDescriptor> tournamentDataDescriptors)
    {
        ClearTournamentList();
        foreach (var tournamentData in tournamentDataDescriptors)
        {
            var uiElement = ObjectPooler.GetPooledGameObject("TournamentUI");
            uiElement.transform.SetParent(_contentRoot);
            uiElement.transform.localScale = Vector3.one;
            if (uiElement.TryGetComponent(out TournamentUIElement tournamentUIElement))
            {
                tournamentUIElement.InitButtonCallback(OnTournamentButtonClick);
                tournamentUIElement.SetData(tournamentData);
            }
        }
    }

    private void OnTournamentButtonClick(int tournamentID)
    {
        _tournamentElementCallback?.Invoke(tournamentID);
    }

    private void ClearTournamentList()
    {
        int listCount = _contentRoot.childCount;

        for (int i = 0; i < listCount; i++)
        {
            var contentElement = _contentRoot.GetChild(0);
            contentElement.gameObject.SetActive(false);
            var poolerRoot = ObjectPooler.PooledObjectsRoot;
            contentElement.SetParent(poolerRoot);
        }
    }

    public void RefreshNavigationPanel(int startIndex, int lastIndex, bool canMoveNext, bool canMovePrev)
    {
        _navigationPrev.interactable = startIndex > 0;
        _navigationNext.interactable = canMoveNext;
        _navigationStatus.text = $"{startIndex+1}..{lastIndex+1}";
    }

    public void Show()
    {
        _windowRootObject.SetActive(true);
    }

    public void Hide()
    {
        ClearTournamentList();
        _windowRootObject.SetActive(false);
    }
}
