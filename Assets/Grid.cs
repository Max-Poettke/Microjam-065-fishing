using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


public class Grid : MonoBehaviour
{
    [Header("PlayerRef")]
    [SerializeField] private Vector2 playerStartingTile;
    [SerializeField] private Player player;

    [Header("Fishes")]
    [SerializeField] private List<GameObject> fishes;
    [SerializeField] private List<Vector2> fishPositions;

    [Header("Islands")]
    [SerializeField] private GameObject islandPrefab;
    [SerializeField] private List<Vector2> islandPositions;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip displaySenseSound;
    [SerializeField] private AudioClip closeSenseSound;
    [Header("Sense Visualization")]
    [SerializeField] private Color occupiedColor;
    [SerializeField] private Color senseColor;
    private bool displaying;

    [Header("Grid")]
    public GameObject TileVisualPrefab;
    public static Grid instance;
    public int tileAmountPerSide = 5;
    public float gridSize = 10f;
    private float tileSize;

    List<List<GridSpace>> gridSpaces;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        tileSize = gridSize / (float)tileAmountPerSide;
        gridSpaces = new List<List<GridSpace>>();
        Vector3 topLeft = transform.position + new Vector3(-gridSize / 2, 0, gridSize / 2);

        for(int i = 0; i < tileAmountPerSide; i++)
        {
            gridSpaces.Add(new List<GridSpace>());
            for(int j = 0; j < tileAmountPerSide; j++)
            {
                float zOffset = -(i * tileSize + tileSize / 2);
                float xOffset = j * tileSize + tileSize / 2; 
                Vector3 tilePosition = topLeft + new Vector3(xOffset, 0, zOffset);

                GameObject visualObject = Instantiate(TileVisualPrefab);
                visualObject.transform.position = tilePosition;

                GridSpace gridSpace = new GridSpace(tilePosition, visualObject, new Vector2(i, j));
                gridSpace.SetVisualColor(senseColor, occupiedColor);

                gridSpaces[i].Add(gridSpace);

                if(i > 0)
                {
                    gridSpace.SetAboveSpace(gridSpaces[i - 1][j]);
                }

                if(j > 0)
                {
                    gridSpace.SetLeftSpace(gridSpaces[i][j - 1]);
                }
            }
        }
    }

    public void InitPlayer()
    {
        player.Init(gridSpaces[(int)playerStartingTile.x][(int)playerStartingTile.y]);
    }

    public void SetIslandAndFishPositions()
    {
        //IslandPositions
        for(int i = 0; i < islandPositions.Count; i++)
        {
            GameObject newIsland = Instantiate(islandPrefab);
            Vector3 originalScale = newIsland.transform.localScale;
            newIsland.transform.position = gridSpaces[(int)islandPositions[i].x][(int)islandPositions[i].y].GetPosition();
            newIsland.transform.localScale = Vector3.zero;
            float randomRotation = Random.Range(0f,1f) * 360;
            newIsland.transform.DORotate(new Vector3(0,randomRotation,0), 0.6f);
            newIsland.transform.DOScale(originalScale, 0.6f);
            gridSpaces[(int)islandPositions[i].x][(int)islandPositions[i].y].SetOccupier(newIsland);
            
        }

        //Fishies and their positions
        for(int i = 0; i < fishPositions.Count; i++)
        {
            fishes[i].transform.position = gridSpaces[(int)fishPositions[i].x][(int)fishPositions[i].y].GetPosition();
            Vector3 originalScale = fishes[i].transform.localScale;
            fishes[i].transform.localScale = Vector3.zero;
            fishes[i].transform.DOScale(originalScale, 0.6f).OnComplete(() => {fishes[i].GetComponent<Fish>().StartIdleAnim();});
            gridSpaces[(int)fishPositions[i].x][(int)fishPositions[i].y].SetOccupier(fishes[i]);
            fishes[i].GetComponent<Fish>().SetCurrentGridSpace(gridSpaces[(int)fishPositions[i].x][(int)fishPositions[i].y]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !displaying)
        {
            DisplaySenseGrid(player.GetCurrentGridSpace(), 2);
        } else if (Input.GetKeyDown(KeyCode.Space))
        {
            CloseSenseGrid();
        }
    }

    public Vector2 GetPlayerDistanceVector(GridSpace fromSpace)
    {
        return player.GetCurrentGridSpace().GetListPosition() - fromSpace.GetListPosition();
    }

    public void DisplaySenseGrid(GridSpace centerSpace, int distance)
    {
        audioSource.Stop();
        audioSource.clip = displaySenseSound;
        audioSource.Play();
        int sideLength = 2 * distance + 1;
        Vector2 topLeftListPosition = centerSpace.GetListPosition() - Vector2.one * distance;

        for(int i = (int)topLeftListPosition.x; i < topLeftListPosition.x + sideLength; i++)
        {
            for(int j = (int)topLeftListPosition.y; j < topLeftListPosition.y + sideLength; j++)
            {
                if(i < 0 || j < 0 || i >= tileAmountPerSide || j >= tileAmountPerSide)
                {
                    continue;
                }

                gridSpaces[i][j].ActivateVisual();
                gridSpaces[i][j].SetVisualColor(senseColor, occupiedColor);
            }
        }
        displaying = true;
    }

    public void CloseSenseGrid()
    {
        audioSource.Stop();
        audioSource.clip = closeSenseSound;
        audioSource.Play();
        for(int i = 0; i < tileAmountPerSide; i++)
        {
            for(int j = 0; j < tileAmountPerSide; j++)
            {
                gridSpaces[i][j].DeactivateVisual();
            }
        }
        displaying = false;
    }

    public List<GameObject> GetFishes()
    {
        return fishes;
    }
}

public class GridSpace
{
    Vector3 position;
    Vector2 listPosition;
    GameObject occupier;
    GameObject visualObject;
    GridSpace aboveSpace;
    GridSpace belowSpace;
    GridSpace leftSpace;
    GridSpace rightSpace;

    public GridSpace(Vector3 position, GameObject visualObject, Vector2 listPosition)
    {
        this.position = position;
        this.visualObject = visualObject;
        this.listPosition = listPosition;
        DeactivateVisual();
    }

    public GameObject GetOccupier()
    {
        return occupier;
    }

    public void SetOccupier(GameObject occupier)
    {
        this.occupier = occupier; 
    }

    public Vector2 GetListPosition()
    {
        return listPosition;
    }

    public GameObject GetVisualObject()
    {
        return visualObject;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public GridSpace GetAboveSpace()
    {
        return aboveSpace;
    }

    public void SetAboveSpace(GridSpace aboveSpace)
    {
        this.aboveSpace = aboveSpace;
        aboveSpace.SetBelowSpace(this);
    }

    public GridSpace GetBelowSpace()
    {
        return belowSpace;
    }

    public void SetBelowSpace(GridSpace belowSpace)
    {
        this.belowSpace = belowSpace;
    }

    public GridSpace GetLeftSpace()
    {
        return leftSpace;
    }

    public void SetLeftSpace(GridSpace leftSpace)
    {
        this.leftSpace = leftSpace;
        leftSpace.SetRightSpace(this);
    }

    public GridSpace GetRightSpace()
    {
        return rightSpace;
    }

    public void SetRightSpace(GridSpace rightSpace)
    {
        this.rightSpace = rightSpace;
    }

    public void SetVisualColor(Color color, Color occupiedColor)
    {
        if(occupier != null)
        {
            visualObject.GetComponent<MeshRenderer>().material.color = occupiedColor;
            return;   
        }
        visualObject.GetComponent<MeshRenderer>().material.color = color;
    }

    public void ActivateVisual()
    {
        visualObject.GetComponent<MeshRenderer>().enabled = true;
    }

    public void DeactivateVisual()
    {
        visualObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
