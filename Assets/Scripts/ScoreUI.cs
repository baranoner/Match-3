using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreTextType0;
    [SerializeField] TextMeshProUGUI scoreTextType1;
    [SerializeField] TextMeshProUGUI scoreTextType2;
    [SerializeField] TextMeshProUGUI scoreTextType3;
    [SerializeField] TextMeshProUGUI scoreTextType4;
    [Header("For Developer")]
    [SerializeField] TextMeshProUGUI movesText;
    [SerializeField] ScoreManager scoreManager;
    
    private void Start() {
        UpdateScoreUI(-1);
        UpdateMovesUI();
    }

    private void OnEnable()
    {
        GameEvents.OnScoreDecreaseByType += UpdateScoreUI;
        GameEvents.OnMoveDecrease += UpdateMovesUI;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreDecreaseByType -= UpdateScoreUI;
        GameEvents.OnMoveDecrease -= UpdateMovesUI;
    }

    private void UpdateScoreUI(int type)
    {
        switch (type)
        {
            case -1:
                scoreTextType0!.text = scoreManager.GetScoreByType(0).ToString();
                scoreTextType1!.text = scoreManager.GetScoreByType(1).ToString();
                scoreTextType2!.text = scoreManager.GetScoreByType(2).ToString();
                scoreTextType3!.text = scoreManager.GetScoreByType(3).ToString();
                scoreTextType4!.text = scoreManager.GetScoreByType(4).ToString();
                break;
            case 0:
                scoreTextType0!.text = scoreManager.GetScoreByType(0).ToString();
                break;
            case 1:
                scoreTextType1!.text = scoreManager.GetScoreByType(1).ToString();
                break;
            case 2:
                scoreTextType2!.text = scoreManager.GetScoreByType(2).ToString();
                break;
            case 3:
                scoreTextType3!.text = scoreManager.GetScoreByType(3).ToString();
                break;
            case 4:
                scoreTextType4!.text = scoreManager.GetScoreByType(4).ToString();
                break;
        }
    }
    private void UpdateMovesUI()
    {
        int moves = scoreManager.GetRemainingMoves();
        movesText.text = $"Moves: {moves}";
    }
}

