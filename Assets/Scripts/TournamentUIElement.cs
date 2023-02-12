using Database;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TournamentUIElement : MonoBehaviour
{
    [SerializeField] private Text _date;
    [SerializeField] private Text _isOver;
    [SerializeField] private Text _winnerName;
    [SerializeField] private Text _number;
    [SerializeField] private Button _elementButton;
    private int _index;

    public void InitButtonCallback(Action<int> buttonCallback)
    {
        _elementButton.onClick.RemoveAllListeners();
        _elementButton.onClick.AddListener(()=>buttonCallback?.Invoke(_index));
    }

    public void SetData(TournamentDataDescriptor tournamentData)
    {
        _index = tournamentData.TournamentID;
        _date.text = tournamentData.Date;
        _isOver.text = tournamentData.IsTournamentOver ? "Finished" : "Not Finished";
        _winnerName.text = "Winner: " + tournamentData.Winner;
        _number.text = (tournamentData.TournamentID + 1).ToString();
    }
}