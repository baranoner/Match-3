using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CircleController : MonoBehaviour
{
    private GameObject selectedCircle = null;
    private GameBoard gameBoard;
    private ScoreManager scoreManager;

    void Awake()
    {
        gameBoard = FindFirstObjectByType<GameBoard>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
        
    }

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                selectedCircle = hit.collider.gameObject;
            }
        }

        if (Input.GetMouseButtonUp(0) && selectedCircle != null) 
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject != selectedCircle)
            {
                CircleHandler selectedHandler = selectedCircle.GetComponent<CircleHandler>();
                CircleHandler targetHandler = hit.collider.GetComponent<CircleHandler>();

                // Check adjacency and swap if valid
                if (gameBoard.ArePositionsAdjacent(selectedHandler.Row, selectedHandler.Col, targetHandler.Row, targetHandler.Col))
                {
                    SwapCircles(selectedHandler, targetHandler);

                    GameEvents.OnMoveDecrease?.Invoke(1);            
                }
            }

            selectedCircle = null;
        }
    }

private void SwapCircles(CircleHandler handlerA, CircleHandler handlerB)
{
    StartCoroutine(SwapAndCheck(handlerA, handlerB));
}

IEnumerator SwapAndCheck(CircleHandler handlerA, CircleHandler handlerB)
{
    Vector3 positionA = handlerA.transform.position;
    Vector3 positionB = handlerB.transform.position;

    
    handlerA.transform.DOKill();
    handlerB.transform.DOKill();

    
    handlerA.transform.DOMove(positionB, 0.3f);
    handlerB.transform.DOMove(positionA, 0.3f);

    
    gameBoard.SwapCirclesBoardLogic(handlerA.Row, handlerA.Col, handlerB.Row, handlerB.Col);

    
    int tempRow = handlerA.Row;
    int tempCol = handlerA.Col;
    handlerA.UpdatePosition(handlerB.Row, handlerB.Col);
    handlerB.UpdatePosition(tempRow, tempCol);

    
    yield return new WaitForSeconds(0.35f); 

    
    bool matchFound = gameBoard.CheckForMatches();
    if (!matchFound)
    {
        
        handlerA.transform.DOMove(positionA, 0.3f);
        handlerB.transform.DOMove(positionB, 0.3f);

        
        gameBoard.SwapCirclesBoardLogic(handlerB.Row, handlerB.Col, handlerA.Row, handlerA.Col);

        
        handlerB.UpdatePosition(handlerA.Row, handlerA.Col);
        handlerA.UpdatePosition(tempRow, tempCol);


    }
}




}

