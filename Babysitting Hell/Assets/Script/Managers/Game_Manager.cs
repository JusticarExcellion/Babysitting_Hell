using UnityEngine;
using UnityEngine.Assertions;

public class Game_Manager : MonoBehaviour
{
    //NOTE: Gamewide game manager that keeps the current state of the level

    public static Game_Manager instance;
    public int Score;

    void
    Awake() // NOTE: Singleton
    {
        if( instance == null )
        {
            instance = this;
        }
        else if( instance != this )
        {
            Destroy( this );
        }
        DontDestroyOnLoad( this );
        Score = 0;
    }

    void
    AddScore( int NewScore )
    {
        Score += NewScore;
    }

    void
    SubtractScore( int NewScore )
    {
        Score -= NewScore;
    }

}
