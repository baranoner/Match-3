using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] circlePrefabs;
    [SerializeField] Vector2 gameBoardSize;
    [SerializeField] float spacing = 1.1f;
    [SerializeField] float dropDuration = 0.5f;
    [SerializeField] float staggerDelay = 0.05f;
    [Header("For Developer")]
    [SerializeField] GameBoard gameBoard;

    void Start()
    {
        gameBoard.Initialize((int)gameBoardSize.x, (int)gameBoardSize.y, spacing, circlePrefabs);
        gameBoard.PopulateGameBoard(circlePrefabs, dropDuration, staggerDelay);
    }

    public void Retry()
    {
       int buildIndex = SceneManager.GetActiveScene().buildIndex;

       SceneManager.LoadScene(buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    
}

