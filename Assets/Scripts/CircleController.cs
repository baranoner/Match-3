using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CircleController : MonoBehaviour
{
    [Header("For Developer")]
    [SerializeField] GameBoard gameBoard;
    [SerializeField] ScoreManager scoreManager;
    private bool _isMouseInputActive = true;
    public bool IsMouseInputActive { get { return _isMouseInputActive; } set { _isMouseInputActive = value; } }    
    private GameObject _selectedCircle;

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && _isMouseInputActive) 
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main!.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                _selectedCircle = hit.collider.gameObject;
            }
        }

        if (Input.GetMouseButtonUp(0) && _selectedCircle != null) 
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main!.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject != _selectedCircle)
            {
                CircleHandler selectedHandler = _selectedCircle.GetComponent<CircleHandler>();
                CircleHandler targetHandler = hit.collider.GetComponent<CircleHandler>();
                
                if (gameBoard.ArePositionsAdjacent(selectedHandler.Row, selectedHandler.Col, targetHandler.Row, targetHandler.Col))
                {
                    StartCoroutine(SwapAndCheck(selectedHandler, targetHandler));

                    GameEvents.OnMoveDecrease?.Invoke();            
                }
            }

            _selectedCircle = null;
        }
    }
    
IEnumerator SwapAndCheck(CircleHandler handlerA, CircleHandler handlerB)
{
    Vector3 positionA = handlerA.transform.position;
    Vector3 positionB = handlerB.transform.position;
    
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

