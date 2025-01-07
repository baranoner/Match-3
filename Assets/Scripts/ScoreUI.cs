using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI blueScoreText;
    [SerializeField] TextMeshProUGUI redScoreText;
    [SerializeField] TextMeshProUGUI yellowScoreText;
    [SerializeField] TextMeshProUGUI movesText;

    private ScoreManager myScoreManager;

    void Awake() 
    {
    myScoreManager = FindFirstObjectByType<ScoreManager>();
    }
    private void Start() {
        blueScoreText.text = myScoreManager.GetScoreByType(0).ToString();
        redScoreText.text =  myScoreManager.GetScoreByType(3).ToString();
        yellowScoreText.text = myScoreManager.GetScoreByType(4).ToString();

        UpdateMovesUI(myScoreManager.GetRemainingMoves());
    }

    private void OnEnable()
    {
        GameEvents.OnScoreAddedByType += UpdateScoreUI;
        GameEvents.OnMoveDecrease += UpdateMovesUI;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreAddedByType -= UpdateScoreUI;
        GameEvents.OnMoveDecrease -= UpdateMovesUI;
    }

    private void UpdateScoreUI(int points, int type)
    {
        switch (type)
        {
            case 0: // Blue
                blueScoreText.text = myScoreManager.GetScoreByType(0).ToString();
                break;
            case 3: // Red
                redScoreText.text =  myScoreManager.GetScoreByType(3).ToString();
                break;
            case 4: // Yellow
                yellowScoreText.text = myScoreManager.GetScoreByType(4).ToString();
                break;
        }
    }
    private void UpdateMovesUI(int moves)
    {
        moves = myScoreManager.GetRemainingMoves();
        movesText.text = $"Moves: {moves}";
    }
}

