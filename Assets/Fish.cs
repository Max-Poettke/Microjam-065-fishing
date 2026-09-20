using UnityEngine;
using DG.Tweening;

public enum FishType
{
    CFish,
    LFish,
    Shark
}


public class Fish : MonoBehaviour
{
    public FishType fishType = FishType.CFish;

    [SerializeField] private Transform meshTransform; //used for bounciness in animation
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioClip chompSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem movementParticles;

    private bool caught;

    private GridSpace currentGridSpace;


    public void TryMove()
    {
        if (caught)
        {
            meshTransform.DOPunchScale(Vector3.one * 0.8f, 0.1f);
            return;
        }
        GridSpace toMoveTo = GetMoveToSpace();
        if(toMoveTo == currentGridSpace) return;
        currentGridSpace.SetOccupier(null);
        
        //move animation
        Vector3 targetPosition = toMoveTo.GetPosition();
        meshTransform.DOPunchScale(Vector3.one * 0.8f, 0.1f);
        transform.DOLookAt(targetPosition, 0.1f);
        transform.DOMove(targetPosition, 0.1f);

        audioSource.clip = moveClip;
        audioSource.pitch = Random.Range(0.99f, 1.1f);
        audioSource.Play();

        movementParticles.Play();

        toMoveTo.SetOccupier(gameObject);
        currentGridSpace = toMoveTo;
    }

    private GridSpace GetMoveToSpace()
    {
        switch (fishType)
        {
            case FishType.CFish:
                return GetMoveAwaySpace();
            case FishType.LFish:
                return GetMoveAwaySpace();
            case FishType.Shark:
                return GetMoveTowardsSpace();
        }
        return currentGridSpace;
    }

    private GridSpace GetMoveAwaySpace()
    {
        Vector2 distanceToPlayer = Grid.instance.GetPlayerDistanceVector(currentGridSpace);
        if(Mathf.Abs(distanceToPlayer.x) > GetSmellDistance() || Mathf.Abs(distanceToPlayer.y) > GetSmellDistance()) return currentGridSpace;
        
        bool below = false;
        bool above = false;
        bool right = false;
        bool left = false;
        
        if(distanceToPlayer.x > 0) below = true;
        if(distanceToPlayer.x < 0) above = true;
        if(distanceToPlayer.y > 0) right = true;
        if(distanceToPlayer.y < 0) left = true;


        //paradoxically, x relates to i here so is the vertical movement
        if(distanceToPlayer.x > 0)
        {
            //player is below -> try to go up
            if(currentGridSpace.GetAboveSpace() != null && !above)
            {
                if(currentGridSpace.GetAboveSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetAboveSpace();    
                }
            } 
            
            if(currentGridSpace.GetLeftSpace() != null && !left)
            {
                if (currentGridSpace.GetLeftSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetLeftSpace();
                }    
            }
             
            if(currentGridSpace.GetRightSpace() != null && !right)
            {
                if(currentGridSpace.GetRightSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetRightSpace();
                }    
            }
            return currentGridSpace;

        } else if (distanceToPlayer.x < 0)
        {
            // player is above -> try to go down
            
            if(currentGridSpace.GetBelowSpace() != null && !below)
            {
                if(currentGridSpace.GetBelowSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetBelowSpace();   
                }
            }
             
            if(currentGridSpace.GetLeftSpace() != null && !left)
            {
                if (currentGridSpace.GetLeftSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetLeftSpace();
                }    
            }
            
            if(currentGridSpace.GetRightSpace() != null && !right)
            {
                if(currentGridSpace.GetRightSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetRightSpace();
                }    
            }

            return currentGridSpace;
        } else if (distanceToPlayer.y > 0)
        {
            //player is to the right -> try move left
            if(currentGridSpace.GetLeftSpace() != null && !left)
            {
                if (currentGridSpace.GetLeftSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetLeftSpace();
                }
            }
            
            if(currentGridSpace.GetAboveSpace()!= null && !above)
            {
                if(currentGridSpace.GetAboveSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetAboveSpace();   
                }    
            }
            
            if(currentGridSpace.GetBelowSpace() != null && !below)
            {
                if(currentGridSpace.GetBelowSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetBelowSpace();   
                }    
            }

            return currentGridSpace;
        } else if (distanceToPlayer.y < 0)
        {
            //player is to the left -> try move right
            if(currentGridSpace.GetRightSpace() != null && !right)
            {
                if (currentGridSpace.GetRightSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetRightSpace();
                }    
            }
            
            if(currentGridSpace.GetAboveSpace() != null && !above)
            {
                if(currentGridSpace.GetAboveSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetAboveSpace();   
                }    
            }
             
            if(currentGridSpace.GetBelowSpace() != null && !below)
            {
                if(currentGridSpace.GetBelowSpace().GetOccupier() == null)
                {
                    return currentGridSpace.GetBelowSpace();   
                }    
            } 
             
            return currentGridSpace;
        }

        return currentGridSpace;
    }

    private GridSpace GetMoveTowardsSpace()
    {
        Vector2 distanceToPlayer = Grid.instance.GetPlayerDistanceVector(currentGridSpace);
        if(Mathf.Abs(distanceToPlayer.x) > GetSmellDistance() || Mathf.Abs(distanceToPlayer.y) > GetSmellDistance()) return currentGridSpace;
        
        bool below = false;
        bool above = false;
        bool right = false;
        bool left = false;

        GridSpace aboveSpace = currentGridSpace.GetAboveSpace();
        GridSpace belowSpace = currentGridSpace.GetBelowSpace();
        GridSpace leftSpace = currentGridSpace.GetLeftSpace();
        GridSpace rightSpace = currentGridSpace.GetRightSpace();
        
        if(distanceToPlayer.x > 0) below = true;
        if(distanceToPlayer.x < 0) above = true;
        if(distanceToPlayer.y > 0) right = true;
        if(distanceToPlayer.y < 0) left = true;

        //Shark eats other fishes first -> should trigger fail condition

        Fish fishToEat = null;
        Player player = null;

        if(aboveSpace != null)
        {
            fishToEat = CheckSpaceForFish(aboveSpace);
            player = CheckSpaceForPlayer(aboveSpace);
        }
        if(belowSpace != null)
        {
            fishToEat = CheckSpaceForFish(belowSpace);
            player = CheckSpaceForPlayer(belowSpace);
        }
        if(leftSpace != null)
        {
            fishToEat = CheckSpaceForFish(leftSpace);
            player = CheckSpaceForPlayer(leftSpace);
        }
        if(rightSpace != null)
        {
            fishToEat = CheckSpaceForFish(rightSpace);
            player = CheckSpaceForPlayer(rightSpace);
        }

        Debug.Log("Player: " + player);

        if(fishToEat != null) EatFish(fishToEat);
        if(player != null) EatPlayer(player);

        //Try to move towards the player - use the previous position of the player




        return currentGridSpace;
    }

    private Fish CheckSpaceForFish(GridSpace gridSpace)
    {
        if (gridSpace.GetOccupier() != null)
        {
            Fish fish = gridSpace.GetOccupier().GetComponent<Fish>();
            if(fish != null)
            {
                return fish;
            }
        }
        return null;
    }

    private Player CheckSpaceForPlayer(GridSpace gridSpace)
    {
        if (gridSpace.GetOccupier() != null)
        {
            Player player = gridSpace.GetOccupier().GetComponent<Player>();
            if(player != null)
            {
                return player;
            }
        }
        return null;
    }

    private void EatFish(Fish fish)
    {
        meshTransform.DOPunchScale(Vector3.one * 1.4f, 0.3f);
        audioSource.clip = chompSound;
        audioSource.Play();

        GameMaster.instance.RemoveFish(fish, true);
    }

    private void EatPlayer(Player player)
    {
        player.transform.DOPunchScale(Vector3.one * 0.7f, 0.3f);

        meshTransform.DOPunchScale(Vector3.one * 1.4f, 0.3f);
        audioSource.clip = chompSound;
        audioSource.Play();

        GameMaster.instance.ReloadScene(true);
    }

    public int GetSmellDistance()
    {
        switch (fishType)
        {
            case FishType.CFish:
                return 2;
            case FishType.LFish:
                return 1;
            case FishType.Shark:
                return 2;
            default:
                return 0;
        }
    }

    public void SetCaught(bool caught)
    {
        this.caught = caught;
    }

    public bool IsCaught()
    {
        return caught;
    }

    public void StartIdleAnim()
    {
        Debug.Log("Started anim");
        transform.DOPunchScale(Vector3.one * 0.05f, 1, 1, 0.3f).OnComplete(() => {StartIdleAnim();});
    }

    public GridSpace GetCurrentGridSpace()
    {
        return currentGridSpace;
    }

    public void SetCurrentGridSpace(GridSpace gridSpace)
    {
        this.currentGridSpace = gridSpace;
    }
}
