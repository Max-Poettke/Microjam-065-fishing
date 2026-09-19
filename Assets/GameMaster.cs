using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public enum GameState {
        Free,
        Animations,
        Start,
        Stop
    }

    private bool animationsFinished;
    private GameState gameState;

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

    
}
