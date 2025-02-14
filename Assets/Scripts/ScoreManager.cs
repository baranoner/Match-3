using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] int totalMoves = 20;
    [SerializeField] int scoreOfBlue;
    [SerializeField] int scoreOfLightBlue;
    [SerializeField] int scoreOfPurple;
    [SerializeField] int scoreOfRed;
    [SerializeField] int scoreOfYellow;
    [Header("For Developer")]
    [SerializeField] GameObject finishUI;
    [SerializeField] CircleController circleController;
    private Dictionary<int, int> _scoresByType = new Dictionary<int, int>();
    private bool _isVictory;

    private void Awake() {
        _scoresByType[0] = scoreOfBlue;
        _scoresByType[1] = scoreOfLightBlue;
        _scoresByType[2] = scoreOfPurple;
        _scoresByType[3] = scoreOfRed;
        _scoresByType[4] = scoreOfYellow;
    }
    private void OnEnable()
    {
        GameEvents.OnScoreDecreaseByType += DecreaseScoreByType;
        GameEvents.OnMoveDecrease += DecreaseMove;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreDecreaseByType -= DecreaseScoreByType;
        GameEvents.OnMoveDecrease -= DecreaseMove;
    }

    private void DecreaseScoreByType(int type)
    {
        if (!_scoresByType.ContainsKey(type))
        {
            _scoresByType[type] = 0; 
        }

        _scoresByType[type]--;

        if(_scoresByType[type] <= 0)
        {
          _scoresByType[type] = 0;

          int isAllTypesZeroCount = 0;
          for (int i = 0; i < _scoresByType.Count; i++)
          {
              if (_scoresByType[i] == 0)
              {
                  isAllTypesZeroCount++;
              }

              if (isAllTypesZeroCount == _scoresByType.Count)
              {
                _isVictory = true;  
                StartCoroutine(EndGame());
              }
          }
        }
        
    }

    public int GetScoreByType(int type)
    {
        return _scoresByType.ContainsKey(type) ? _scoresByType[type] : 0;
    }

    public void DecreaseMove()
    {
        totalMoves --;

        if (totalMoves <= 0)
        {
            totalMoves = 0;
            StartCoroutine(EndGame());
        }
    }
    private IEnumerator EndGame()
    {
        circleController.IsMouseInputActive = false;
        
        yield return new WaitForSeconds(1f);
        
        finishUI.SetActive(true);

        string message = _isVictory ? "You Win!" : "Out of Moves!";
        finishUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;
        
    }
    public int GetRemainingMoves()
    {
        return totalMoves;
    }
}
