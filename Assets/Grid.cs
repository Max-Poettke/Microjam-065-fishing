using UnityEngine;
using System.Collections.Generic;


public class Grid : MonoBehaviour
{
    [Header("PlayerRef")]
    [SerializeField] private Vector2 playerStartingTile;
    [SerializeField] private Player player;

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
            }
        }

        player.Init(gridSpaces[(int)playerStartingTile.x][(int)playerStartingTile.y]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !displaying)
        {
            DisplaySenseGrid(player.GetCurrentGridSpace(), 1);
        } else if (Input.GetKeyDown(KeyCode.Space))
        {
            CloseSenseGrid();
        }
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
                if(i < 0 || j < 0)
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

    public GridSpace GetBelowSpace()
    {
        return belowSpace;
    }

    public GridSpace GetLeftSpace()
    {
        return leftSpace;
    }

    public GridSpace GetRightSpace()
    {
        return rightSpace;
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
