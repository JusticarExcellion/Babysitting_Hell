using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    [Header("UI Target:")]
    public Transform TargetTransform;
    private Character TargetCharacter;
    private Transform MyTransform;

    [Header("Fill Bar Object:")]
    [SerializeField] private GameObject FillBar;

    [Header("UI Position Parameters: ")]
    [SerializeField] private RectTransform BackgroundTransform;
     private RectTransform FillTransform;


    [Header("Progress Bar Parameters: ")]
    public float ProgressTime;
    public Vector2 ProgressBarOffset;//TODO: The Progress Bar Offset needs to be determined by the image size for the character
    private Vector2 BarDimensions;

    [HideInInspector]public RectTransform CanvasTransform;
    private Image FillImage;
    private float Fill = 0;
    private Camera MainCamera;
    private UserInput CameraControls;
    private ProgressBar Self;

    void Start()
    {
        MainCamera = Camera.main;
        MyTransform = this.transform;
        Self = this.GetComponent<ProgressBar>();

        FillImage = FillBar.GetComponent< Image >();
        FillTransform = FillBar.GetComponent< RectTransform >();
        CameraControls = FindObjectOfType<UserInput>();
        BarDimensions = FillTransform.sizeDelta;
        TargetCharacter = TargetTransform.GetComponent<Character>();
        FillImage.fillAmount = Fill;

        PlacingBarOnScreen();
        ApplyCameraSizeToBarScale();
    }

    void Update()
    {
        FillImage.fillAmount = TargetCharacter.InteractionTime / ProgressTime;
        if( FillImage.fillAmount <= 0.0f ) Destroy( this.gameObject );

        PlacingBarOnScreen();
        ApplyCameraSizeToBarScale();
    }

    private void
    PlacingBarOnScreen() //Placing the progress bar on the screen above the interacting character
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint( TargetTransform.position );

        float ScreenPositionX = ( (ViewportPosition.x * CanvasTransform.sizeDelta.x ) ) - ( CanvasTransform.sizeDelta.x * 0.5f ) + ProgressBarOffset.x;
        float ScreenPositionY = (ViewportPosition.y * CanvasTransform.sizeDelta.y ) - ( CanvasTransform.sizeDelta.y * 0.5f ) + ( ProgressBarOffset.y * (CameraControls.MinCameraSize / MainCamera.orthographicSize) );

        Vector2 ScreenPosition = new Vector2( ScreenPositionX , ScreenPositionY );

        BackgroundTransform.anchoredPosition = ScreenPosition;
        FillTransform.anchoredPosition = ScreenPosition;

    }

    private void
    ApplyCameraSizeToBarScale() //NOTE: Adjusting Bar Based on the current Zoom value
    {
        float ProgressBarSizeX = CameraControls.MinCameraSize / MainCamera.orthographicSize;
        float ProgressBarSizeY = CameraControls.MinCameraSize / MainCamera.orthographicSize;

        Vector2 NewBarSize = new Vector2( BarDimensions.x * ProgressBarSizeX, BarDimensions.y * ProgressBarSizeY);

        FillTransform.sizeDelta = NewBarSize;
        BackgroundTransform.sizeDelta = NewBarSize;
    }

    private void
    OnDestroy()
    {
        UIManager UIM = FindObjectOfType<UIManager>();
        UIM.DestroyProgressBar( Self );
    }
}
