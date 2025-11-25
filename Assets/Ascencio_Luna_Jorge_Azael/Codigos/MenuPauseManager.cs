using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class MenuPauseManager : MonoBehaviour
{
    [Header("Material del Shader")]
    public Material targetMaterial;

    [Header("Sliders de Color")]
    public Slider sliderColor1;
    public Slider sliderColor2;

    [Header("FMOD Música")]
    public Slider musicSlider;
    public StudioEventEmitter musicEmitter;
    private EventInstance musicInstance;

    [Header("Botón Exit")]
    public Button exitButton;

    // Nombres de las propiedades del shader graph
    string propColor1 = "_Color_1";
    string propColor2 = "_Color_2";


    public GameObject pausePanel;

    public void ShowPauseMenu()
    {
        Debug.Log(">>> PAUSE MENU ACTIVADO desde WristLookEvent <<<");

        pausePanel.SetActive(true);
    }

    public void HidePauseMenu()
    {
        Debug.Log(">>> PAUSE MENU DESACTIVADO <<<");

        pausePanel.SetActive(false);
    }

    void Start()
    {
        // FMOD instance
        musicInstance = musicEmitter.EventInstance;

        // Listeners para sliders
        sliderColor1.onValueChanged.AddListener(UpdateColor1);
        sliderColor2.onValueChanged.AddListener(UpdateColor2);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);

        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        // Aplicar valores iniciales
        UpdateColor1(sliderColor1.value);
        UpdateColor2(sliderColor2.value);
        SetMusicVolume(musicSlider.value);
    }

    // Cambiar Color (1)
    void UpdateColor1(float v)
    {
        // Escala de gris basada en slider
        Color c = new Color(v, v, v, 1);
        targetMaterial.SetColor(propColor1, c);
    }

    // Cambiar Color (2)
    void UpdateColor2(float v)
    {
        Color c = new Color(v, v, v, 1);
        targetMaterial.SetColor(propColor2, c);
    }

    // Control de volumen FMOD
    void SetMusicVolume(float v)
    {
        musicInstance.setParameterByName("MusicVolume", v);
    }
}
