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

    // Properties del Shader
    string propColor1 = "_Color_1";
    string propColor2 = "_Color_2";

    public GameObject pausePanel;

    void Start()
    {
        // FMOD instance
        musicInstance = musicEmitter.EventInstance;

        // Aplicar valores iniciales SOLO si los sliders existen
        if (sliderColor1 != null)
            UpdateColor1(sliderColor1.value);

        if (sliderColor2 != null)
            UpdateColor2(sliderColor2.value);

        if (musicSlider != null)
            SetMusicVolume(musicSlider.value);
    }

    // ------------------------------
    // FUNCIONES PARA EL INSPECTOR
    // ------------------------------

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

    public void ExitGame()
    {
        Debug.Log(">>> EXIT PRESIONADO <<<");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void UpdateColor1(float v)
    {
        Color c = new Color(v, v, v, 1);
        targetMaterial.SetColor(propColor1, c);
    }

    public void UpdateColor2(float v)
    {
        Color c = new Color(v, v, v, 1);
        targetMaterial.SetColor(propColor2, c);
    }

    public void SetMusicVolume(float v)
    {
        musicInstance.setParameterByName("MusicVolume", v);
    }
}
