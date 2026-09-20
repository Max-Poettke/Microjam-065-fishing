using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public GameObject timerVisual;
    private Vector3 timerVisualStartSize;
    public enum GameState {
        Free,
        PlayerAnimations,
        NPCAnimations,
        Start,
        Stop
    }
    private GameState gameState;
    private GameState nextState;

    [SerializeField] private float startingCountDown = 0.3f;
    private bool spawnedWorld = false;
    private bool spawnedPlayer = false;

    private float countDown = 0f;
    private float currentCountDownMax = 0f;

    private List<GameObject> activeFishes;
    private int fishesMoved = 0;
    private int turnCounter = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timerVisualStartSize = timerVisual.transform.localScale;
        turnCounter = 0;
        activeFishes = Grid.instance.GetFishes();
        gameState = GameState.Start;
        AddToCountDown(startingCountDown, GameState.Free);
    }

    void Update()
    {
        if(gameState == GameState.Start)
        {
            if (!spawnedWorld)
            {
                spawnedWorld = true;
                Grid.instance.SetIslandAndFishPositions();
            }

            if (!spawnedPlayer)
            {
                if(countDown < startingCountDown / 2f)
                {
                    Grid.instance.InitPlayer();
                }    
            }
        }

        if(countDown <= 0.1f)
        {
            if(gameState == GameState.PlayerAnimations)
            {
            } else if (gameState == GameState.NPCAnimations && nextState == GameState.NPCAnimations)
            {
                Debug.Log(activeFishes.Count + " " + fishesMoved);
                activeFishes[fishesMoved].GetComponent<Fish>().TryMove();
                fishesMoved ++;

                if(fishesMoved < activeFishes.Count)
                {
                    AddToCountDown(0.14f, GameState.NPCAnimations);     
                } else
                {
                    AddToCountDown(0.14f, GameState.Free);
                }

            } else if (gameState == GameState.NPCAnimations && nextState == GameState.Free)
            {
                turnCounter ++;
            }
        }

        if(countDown <= 0f)
        {
            if(nextState == GameState.NPCAnimations)
            {
                fishesMoved = 0;
            }
            gameState = nextState;
            countDown = 0f;
            return;
        }

        countDown -= Time.deltaTime;
        timerVisual.transform.localScale = Vector3.Lerp(Vector3.zero, timerVisualStartSize, countDown / currentCountDownMax);
    }

    public void AddToCountDown(float time, GameState nextState)
    {
        this.nextState = nextState;
        countDown += time;
        currentCountDownMax = countDown;
        timerVisual.transform.localScale = timerVisualStartSize;
    }

    public void SetState(GameState state)
    {
        gameState = state;
    }

    public bool CanMove()
    {
        return gameState == GameState.Free;
    }
}
