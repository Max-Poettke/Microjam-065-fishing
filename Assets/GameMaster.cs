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

    [SerializeField] private float startingCountDown = 0.3f;
    private bool starting = true;
    private bool spawnedWorld = false;
    private bool spawnedPlayer = false;

    private float countDown = 0f;
    private float currentCountDownMax = 0f;
    private int turnCounter = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timerVisualStartSize = timerVisual.transform.localScale;
        turnCounter = 0;
        AddToCountDown(startingCountDown, GameState.Start);
    }

    void Update()
    {
        if(starting)
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
                //switch to NPC animations and give them time to do something
            } else if (gameState == GameState.NPCAnimations)
            {
                //check if there is another NPC that still has to do an animation
                
                //else leave countdown to finish
            }
        }

        if(countDown <= 0f)
        {
            starting = false;
            gameState = GameState.Free;
            countDown = 0f;
            return;
        }

        countDown -= Time.deltaTime;
        timerVisual.transform.localScale = Vector3.Lerp(Vector3.zero, timerVisualStartSize, countDown / currentCountDownMax);
    }

    public void AddToCountDown(float time, GameState gameState)
    {
        this.gameState = gameState;
        countDown += time;
        currentCountDownMax = countDown;
        timerVisual.transform.localScale = timerVisualStartSize;
    }

    public bool CanMove()
    {
        return gameState == GameState.Free;
    }
}
