using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip movementAudio;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float animationTime = 0.3f;
    [SerializeField] private float idleAnimationLoop = 3f;
    [SerializeField] private Transform shakeTransform;
    [SerializeField] private Transform squishTransform;

    [SerializeField] private GameObject cageTrap;
    [SerializeField] private GameObject explosionTrap;

    private GridSpace currentGridSpace;
    private GridSpace previousGridSpace;

    private Vector3 orignalSize;

    public void Init(GridSpace startGridSpace)
    {
        currentGridSpace = startGridSpace;
        currentGridSpace.SetOccupier(gameObject);
        previousGridSpace = startGridSpace;
        transform.position = currentGridSpace.GetPosition();
        
        orignalSize = transform.localScale;
        Vector3 scaleToReach = transform.localScale;
        squishTransform.localScale = Vector3.zero;

        squishTransform.DOScale(scaleToReach, 0.6f).OnComplete(() => {StartIdleAnim();});
    }

    void Update()
    {
        GetInput();
    }

    private void PlayMoveSoundPitchRandomized()
    {
        audioSource.clip = movementAudio;
        audioSource.pitch = Random.Range(0.99f, 1.1f);
        audioSource.Play();
    }

    private void StartIdleAnim()
    {
        transform.localScale = orignalSize;
        transform.DOPunchScale(Vector3.one * 0.05f, idleAnimationLoop, 1, 0.3f).OnComplete(() => {StartIdleAnim();});
    }

    private void GetInput()
    {
        if (!GameMaster.instance.CanMove()) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameMaster.instance.ReloadScene(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlaceTrap(false);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlaceTrap(true);
        }
        
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

    private void PlaceTrap(bool kill)
    {
        if(currentGridSpace.GetTrap() != null) return;
        
        if (kill)
        {
            GameObject trap = Instantiate(explosionTrap);
            currentGridSpace.SetTrap(trap.GetComponent<Trap>());
            trap.transform.position = currentGridSpace.GetPosition();
        } else
        {
            GameObject trap = Instantiate(cageTrap);
            currentGridSpace.SetTrap(trap.GetComponent<Trap>());
            trap.transform.position = currentGridSpace.GetPosition();
        }
    }

    private void TryMove(GridSpace toMoveTo)
    {
        if(toMoveTo == null)
        {
            squishTransform.DOPunchScale(Vector3.one * 0.2f, animationTime / 2f);
            GameMaster.instance.SetState(GameMaster.GameState.PlayerAnimations);
            GameMaster.instance.AddToCountDown(animationTime / 2f, GameMaster.GameState.Free);
            return;
        }
        if(toMoveTo.GetOccupier() != null)
        {
            if(toMoveTo.GetTrap() == null)
            {
                //play error sound
                squishTransform.DOPunchScale(Vector3.one * 0.2f, animationTime / 2f);
                GameMaster.instance.SetState(GameMaster.GameState.PlayerAnimations);
                GameMaster.instance.AddToCountDown(animationTime / 2f, GameMaster.GameState.Free);
                return;   
            }

            toMoveTo.GetTrap().EmptyOut(false);
        }

        previousGridSpace = currentGridSpace;
        currentGridSpace.SetOccupier(null);
        //move animation
        
        Vector3 targetPosition = toMoveTo.GetPosition();
        squishTransform.DOPunchScale(Vector3.one * 0.2f, animationTime);
        transform.DOLookAt(targetPosition, animationTime);
        transform.DOMove(targetPosition, animationTime);
        GameMaster.instance.SetState(GameMaster.GameState.PlayerAnimations);
        GameMaster.instance.AddToCountDown(animationTime, GameMaster.GameState.NPCAnimations);

        PlayMoveSoundPitchRandomized();
        toMoveTo.SetOccupier(gameObject);
        currentGridSpace = toMoveTo;
    }

    public GridSpace GetCurrentGridSpace()
    {
        return currentGridSpace;
    }

    public GridSpace GetPreviousGridSpace()
    {
        return previousGridSpace;
    }
}
