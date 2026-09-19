using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private GridSpace currentGridSpace;
    private bool triedMovingThisFrame;
    public void Init(GridSpace startGridSpace)
    {
        currentGridSpace = startGridSpace;
        currentGridSpace.SetOccupier(gameObject);
        transform.position = currentGridSpace.GetPosition();
    }

    void Update()
    {
        GetInput();
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
        if(toMoveTo.GetOccupier() != null)
        {
            //play error sound
            return;
        }

        currentGridSpace.SetOccupier(null);
        //move

        toMoveTo.SetOccupier(gameObject);
    }

    public GridSpace GetCurrentGridSpace()
    {
        return currentGridSpace;
    }
}
