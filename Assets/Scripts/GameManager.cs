using UnityEngine;


//this scripe is a Singleton, which makes only one instance of it alive during the course of the game

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The Awake
    /// </summary>
    internal void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Defines the instance
    /// </summary>
    private static GameManager instance;

    /// <summary>
    /// Gets the Instance
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    protected GameManager()
    {
        GameState = GameState.Start;
    }

    public GameState GameState { get; set; }

    public void Die()
    {
        UIManager.Instance.SetStatus(Constants.StatusDeadTapToStart);
        this.GameState = GameState.Dead;
    }



}
