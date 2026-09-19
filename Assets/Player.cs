using UnityEngine;

public class Player : MonoBehaviour
{
    private GridSpace currentGridSpace;
    public void Init(GridSpace startGridSpace)
    {
        currentGridSpace = startGridSpace;
        currentGridSpace.SetOccupier(gameObject);
        transform.position = currentGridSpace.GetPosition();
    }

    private void TryMove(GridSpace toMoveTo)
    {
        if(toMoveTo.GetOccupier() != null)
        {
            //play error sound
            return;
        }

            
    }

    public GridSpace GetCurrentGridSpace()
    {
        return currentGridSpace;
    }
}
