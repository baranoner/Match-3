using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] int totalMoves = 20;
    [SerializeField] int scoreForBlue = 50;
    [SerializeField] int scoreForRed = 50;
    [SerializeField] int scoreForYellow = 50;
    [SerializeField] GameObject finishUI;
    private Dictionary<int, int> scoresByType = new Dictionary<int, int>();
    private CircleController myCircleController; 

    private void Awake() {
        scoresByType[0] = scoreForBlue;
        scoresByType[3] = scoreForRed;
        scoresByType[4] = scoreForYellow;

        myCircleController = FindFirstObjectByType<CircleController>();
    }
    private void OnEnable()
    {
        GameEvents.OnScoreAddedByType += AddScoreByType;
        GameEvents.OnMoveDecrease += DecreaseMove;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreAddedByType -= AddScoreByType;
        GameEvents.OnMoveDecrease -= DecreaseMove;
    }

    private void AddScoreByType(int points, int type)
    {
        if (!scoresByType.ContainsKey(type))
        {
            scoresByType[type] = 0; 
        }

        scoresByType[type] -= points;
        Debug.Log($"Score Updated - Type {type}: {scoresByType[type]}");

        if(scoresByType[type] <= 0)
        {
          scoresByType[type] = 0;

          if(scoresByType[0] == 0 && scoresByType[3] == 0 && scoresByType[4] == 0)
          {
            StartCoroutine(EndGame(true));
          }  
        }
        
    }

    public int GetScoreByType(int type)
    {
        return scoresByType.ContainsKey(type) ? scoresByType[type] : 0;
    }

    public Dictionary<int, int> GetAllScores()
    {
        return scoresByType;
    }

    public void DecreaseMove(int amount)
    {
        totalMoves -= amount;
        Debug.Log($"Moves Remaining: {totalMoves}");

        if (totalMoves <= 0)
        {
            totalMoves = 0;
            StartCoroutine(EndGame(false));
        }
    }
    private IEnumerator EndGame(bool isVictory)
    {
        finishUI.SetActive(true);

        var message = isVictory ? "You Win!" : "Out of Moves!";
        finishUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;

        yield return new WaitForSeconds(1f);

        myCircleController.gameObject.SetActive(false);
    }
    public int GetRemainingMoves()
    {
        return totalMoves;
    }
}
