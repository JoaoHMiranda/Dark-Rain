using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Interface")]
    public GameObject pausePanel;
    public string menuSceneName = "MainMenu"; 

    // Referência ao outro painel para não abrir por cima
    public GameObject levelUpPanel; 

    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // --- PROTEÇÃO ---
            // Se o Painel de Level Up estiver aberto, IGNORA o botão ESC.
            if (levelUpPanel != null && levelUpPanel.activeSelf) 
            {
                return; 
            }
            // ----------------

            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true); 
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false); 
        Time.timeScale = 1f; 
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(menuSceneName);
    }
    
    public void QuitGame()
    {
        Application.Quit(); 
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}