using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public enum GameState {
        Free,
        Animations,
        Start,
        Stop
    }

    private bool animationsFinished;
    private GameState gameState;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameState = GameState.Start;
    }

    void Update()
    {
        if(animationsFinished && gameState != GameState.Free)
        {
            gameState = GameState.Free;
        }
    }

    public bool CanMove()
    {
        return gameState == GameState.Free;
    }
}
