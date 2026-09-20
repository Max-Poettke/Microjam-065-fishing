using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public GameObject timerVisual;

    [SerializeField] private RawImage blackScreen;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip lossClip;
    [SerializeField] private AudioSource audioSource;

    public int catchCondition = 1;
    public int killCondition = 0;

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
        blackScreen.color = Color.black;
        blackScreen.DOFade(0, 2);
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

    public void RemoveFish(Fish fish, bool killed = false)
    {
        if (activeFishes.Contains(fish.gameObject))
        {
            activeFishes.Remove(fish.gameObject);
            if (killed)
            {
                if (fish.fishType.Equals(FishType.CFish) || fish.GetType().Equals(FishType.Shark))
                {
                    ReloadScene(true);
                } else
                {
                    killCondition --;
                }
            } else
            {
                if (!fish.fishType.Equals(FishType.CFish))
                {
                    ReloadScene(true);
                } else
                {
                    catchCondition --;
                }
            }

            Destroy(fish.gameObject);

            if(catchCondition == 0 && killCondition == 0)
            {
                Victory();
            }
        }
    }

    public void ReloadScene(bool failed)
    {
        gameState = GameState.Stop;
        nextState = GameState.Stop;
        countDown = 0f;

        if (failed)
        {
            audioSource.clip = lossClip;
            audioSource.Play();
        }
        blackScreen.DOFade(1, 2).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });        
    }

    public void Victory()
    {
        audioSource.clip = winClip;
        audioSource.Play();

        gameState = GameState.Stop;
        nextState = GameState.Stop;
        countDown = 0f;

        blackScreen.DOFade(1, 2).OnComplete(() =>
        {
            
        }); 
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
