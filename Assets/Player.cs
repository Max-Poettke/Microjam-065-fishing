using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.3f;
    [SerializeField] private float idleAnimationLoop = 3f;
    [SerializeField] private Transform shakeTransform;
    [SerializeField] private Transform squishTransform;
    private GridSpace currentGridSpace;

    public void Init(GridSpace startGridSpace)
    {
        currentGridSpace = startGridSpace;
        currentGridSpace.SetOccupier(gameObject);
        transform.position = currentGridSpace.GetPosition();
        
        Vector3 scaleToReach = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(scaleToReach, 0.6f).OnComplete(() => {StartIdleAnim();});
    }

    void Update()
    {
        GetInput();
    }

    private void StartIdleAnim()
    {
        Debug.Log("Started anim");
        transform.DOPunchScale(Vector3.one * 0.05f, idleAnimationLoop, 1, 0.3f).OnComplete(() => {StartIdleAnim();});
    }

    private void GetInput()
    {
        if (!GameMaster.instance.CanMove()) return;
        
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryMove(currentGridSpace.GetAboveSpace()); 
        } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMove(currentGridSpace.GetLeftSpace());
        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryMove(currentGridSpace.GetBelowSpace());
        } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMove(currentGridSpace.GetRightSpace());
        }
    }

    private void TryMove(GridSpace toMoveTo)
    {
        if(toMoveTo == null)
        {
            squishTransform.DOPunchScale(Vector3.one * 0.2f, animationTime / 2f);
            GameMaster.instance.AddToCountDown(animationTime / 2f, GameMaster.GameState.PlayerAnimations);
            return;
        }
        if(toMoveTo.GetOccupier() != null)
        {
            //play error sound
            squishTransform.DOPunchScale(Vector3.one * 0.2f, animationTime / 2f);
            GameMaster.instance.AddToCountDown(animationTime / 2f, GameMaster.GameState.PlayerAnimations);
            return;
        }

        currentGridSpace.SetOccupier(null);
        //move animation
        
        Vector3 targetPosition = toMoveTo.GetPosition();
        squishTransform.DOPunchScale(Vector3.one * 0.2f, animationTime);
        transform.DOLookAt(targetPosition, animationTime);
        transform.DOMove(targetPosition, animationTime);
        GameMaster.instance.AddToCountDown(animationTime, GameMaster.GameState.PlayerAnimations);

        toMoveTo.SetOccupier(gameObject);
        currentGridSpace = toMoveTo;
    }

    public GridSpace GetCurrentGridSpace()
    {
        return currentGridSpace;
    }
}
