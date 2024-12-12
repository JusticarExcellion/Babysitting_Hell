using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChildUI : MonoBehaviour
{
    //TODO: Make this more Dynamic, happening when the Child UI Component get's created instead of this being highly static and time consuming

    public ChildAI_test AttachedAI;
    public Image ChildHeadSprite;
    public GameObject SpeechBubble;
    public TMP_Text Text;
    public Image HealthBar;
    public Image HappinessBar;

    [Header("UI Settings")]
    [SerializeField] private float WaitTimer;
    private float CurrentTimer;

    void
    Start()
    {
        WaitTimer = 3.0f;
        CurrentTimer = 0.0f;
    }

    void
    Update()
    {

        if( SpeechBubble.activeSelf && CurrentTimer > 0.0f )
        {
            CurrentTimer -= Time.deltaTime;
        }
        else if( SpeechBubble.activeSelf && CurrentTimer <= 0.0f )
        {
            SpeechBubble.SetActive( false );
        }
    }

    public void
    ChildMessage( string Message )
    {
        SpeechBubble.SetActive( true );
        Text.SetText( Message );
        this.SetBarkTimer();
    }

    public void
    SetBarkTimer()
    {
        CurrentTimer = WaitTimer;
    }

    public void
    UpdateChildStatus( float NewHealth, float NewHappiness )
    {
        HealthBar.fillAmount = NewHealth;
        HappinessBar.fillAmount = NewHappiness;
    }

    private void
    OnDestroy()
    {
        AttachedAI.UI = null;
        AttachedAI = null;
    }

}
