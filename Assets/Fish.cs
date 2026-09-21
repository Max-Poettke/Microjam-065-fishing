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
        
        GameMaster.instance.AddToCountDown(0.12f, GameState.NPCAnimations);
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
        
        if(fishType == FishType.LFish)
        {
            GridSpace aboveSpace = currentGridSpace.GetAboveSpace();
            GridSpace belowSpace = currentGridSpace.GetBelowSpace();
            GridSpace leftSpace = currentGridSpace.GetLeftSpace();
            GridSpace rightSpace = currentGridSpace.GetRightSpace();
            
            Fish fishToEat = null;
            if(aboveSpace != null && fishToEat == null)
            {
                fishToEat = CheckSpaceForFish(aboveSpace);
            }
            if(belowSpace != null && fishToEat == null)
            {
                fishToEat = CheckSpaceForFish(belowSpace);
            }
            if(leftSpace != null && fishToEat == null)
            {
                fishToEat = CheckSpaceForFish(leftSpace);
            }
            if(rightSpace != null && fishToEat == null)
            {
                fishToEat = CheckSpaceForFish(rightSpace);
            }

            if(fishToEat != null) {
                EatFish(fishToEat);
                return currentGridSpace;
            }
        }
        
        
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
        Vector2 distanceToPlayer = Grid.instance.GetPlayerPreviousGrid().GetListPosition() - currentGridSpace.GetListPosition();

        //Shark eats other fishes first -> should trigger fail condition

        GridSpace aboveSpace = currentGridSpace.GetAboveSpace();
        GridSpace belowSpace = currentGridSpace.GetBelowSpace();
        GridSpace leftSpace = currentGridSpace.GetLeftSpace();
        GridSpace rightSpace = currentGridSpace.GetRightSpace();

        Fish fishToEat = null;
        Player player = null;

        if(aboveSpace != null && player == null)
        {
            player = CheckSpaceForPlayer(aboveSpace);
        }
        if(belowSpace != null && player == null)
        {
            player = CheckSpaceForPlayer(belowSpace);
        }
        if(leftSpace != null && player == null)
        {
            player = CheckSpaceForPlayer(leftSpace);
        }
        if(rightSpace != null && player == null)
        {
            player = CheckSpaceForPlayer(rightSpace);
        }



        if(aboveSpace != null && fishToEat == null)
        {
            fishToEat = CheckSpaceForFish(aboveSpace);
        }
        if(belowSpace != null && fishToEat == null)
        {
            fishToEat = CheckSpaceForFish(belowSpace);
        }
        if(leftSpace != null && fishToEat == null)
        {
            fishToEat = CheckSpaceForFish(leftSpace);
        }
        if(rightSpace != null && fishToEat == null)
        {
            fishToEat = CheckSpaceForFish(rightSpace);
        }

        bool eaten = false;

        if(fishToEat != null) 
        {
            EatFish(fishToEat);
            eaten = true;
        }
        if(player != null)
        {
            EatPlayer(player);
            eaten = true;    
        } 
        if(eaten) return currentGridSpace;
        

        if(Mathf.Abs(distanceToPlayer.x) > GetSmellDistance() || Mathf.Abs(distanceToPlayer.y) > GetSmellDistance()) return currentGridSpace;
        
        bool below = false;
        bool above = false;
        bool right = false;
        bool left = false;

        
        
        if(distanceToPlayer.x > 0) below = true;
        if(distanceToPlayer.x < 0) above = true;
        if(distanceToPlayer.y > 0) right = true;
        if(distanceToPlayer.y < 0) left = true;

        

        

        //Try to move towards the player - use the previous position of the player

        if(below)
        {
            if(belowSpace != null)
            {
                if(belowSpace.GetOccupier() == null)
                {
                    return belowSpace;
                }
            }

            if(leftSpace != null)
            {
                if(leftSpace.GetOccupier() == null)
                {
                    return leftSpace;
                }
            }

            if(rightSpace != null)
            {
                if(rightSpace.GetOccupier() == null)
                {
                    return rightSpace;
                }
            }
        }

        if (above)
        {
            if(aboveSpace != null)
            {
                if(aboveSpace.GetOccupier() == null)
                {
                    return aboveSpace;
                }
            }

            if(leftSpace != null)
            {
                if(leftSpace.GetOccupier() == null)
                {
                    return leftSpace;
                }
            }

            if(rightSpace != null)
            {
                if(rightSpace.GetOccupier() == null)
                {
                    return rightSpace;
                }
            }
        }

        if (left)
        {
            if(leftSpace != null)
            {
                if(leftSpace.GetOccupier() == null)
                {
                    return leftSpace;
                }
            }

            if(aboveSpace != null)
            {
                if(aboveSpace.GetOccupier() == null)
                {
                    return aboveSpace;
                }
            }

            if(belowSpace != null)
            {
                if(belowSpace.GetOccupier() == null)
                {
                    return belowSpace;
                }
            }
        }

        if (right)
        {
            if(rightSpace != null)
            {
                if (rightSpace.GetOccupier() == null)
                {
                    return rightSpace;
                }
            }

            if(aboveSpace != null)
            {
                if(aboveSpace.GetOccupier() == null)
                {
                    return aboveSpace;
                }
            }

            if(belowSpace != null)
            {
                if(belowSpace.GetOccupier() == null)
                {
                    return belowSpace;
                }
            }
        }

        return currentGridSpace;
    }

    private Fish CheckSpaceForFish(GridSpace gridSpace)
    {
        if (gridSpace.GetOccupier() != null)
        {
            Fish fish = gridSpace.GetOccupier().GetComponent<Fish>();
            if(fish != null)
            {
                if(fishType == FishType.LFish)
                {
                    if(fish.fishType != FishType.CFish) return null;
                }
                return fish;
            }
        }
        return null;
    }

    private Player CheckSpaceForPlayer(GridSpace gridSpace)
    {
        if (gridSpace.GetOccupier() != null)
        {
            Debug.Log("Occupant found");
            Player player = gridSpace.GetOccupier().GetComponent<Player>();
            Debug.Log(player);
            return player;

        }
        return null;
    }

    private void EatFish(Fish fish)
    {
        Debug.Log("Eating fish");
        meshTransform.DOPunchScale(Vector3.one * 1.4f, 0.3f);
        audioSource.clip = chompSound;
        audioSource.Play();

        GameMaster.instance.RemoveFish(fish, true);
    }

    private void EatPlayer(Player player)
    {
        player.transform.DOPunchScale(Vector3.one * 0.7f, 0.2f).OnComplete(()=>{Destroy(player.gameObject);});

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
