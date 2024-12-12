using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Tooltip("Number of Minutes in the level, you can give decimals for finer tuning")]
    [HideInInspector] public float SecondsLeft;
    private TMP_Text Text;
    private LevelManager LM;

    public void
    SetTimer( float MinutesInLevel )
    {
        SecondsLeft = MinutesInLevel * 60;
        Text = this.GetComponent<TMP_Text>();
        Text.SetText("");
        LM = FindObjectOfType<LevelManager>();
    }

    void
    Update()
    {
        if( SecondsLeft > 0f )
        {
            SecondsLeft -= Time.deltaTime;
        }
        else if( SecondsLeft < 0f )
        {
            SecondsLeft = 0.0f;
            LM.EndLevel();
        }
    }

    void
    FixedUpdate()
    {

        int CleanedMinutesLeft = (int)(SecondsLeft / 60);
        int CleanedSecondsLeft = (int)SecondsLeft % 60;
        string CurrentTimer = CleanedSecondsLeft < 10 ? CleanedMinutesLeft + ":0" + CleanedSecondsLeft : CleanedMinutesLeft + ":" + CleanedSecondsLeft;

        Text.SetText(  CurrentTimer );
    }
}
