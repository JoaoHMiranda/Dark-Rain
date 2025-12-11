using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para reiniciar ou ir pro menu

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel; // Arraste o Panel_GameOver aqui

    [Header("Cenas")]
    public string menuSceneName = "MainMenu"; // Nome da cena do menu

    public void ShowGameOver()
    {
        // 1. Mostra o painel
        gameOverPanel.SetActive(true);

        // 2. PAUSA O JOGO (Congela o tempo)
        Time.timeScale = 0f;
        
        // (Opcional) Destrava o mouse se estiver escondido
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        // Despausa o tempo antes de reiniciar (MUITO IMPORTANTE)
        Time.timeScale = 1f;
        
        // Recarrega a cena atual (SampleScene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f; // Despausa
        SceneManager.LoadScene(menuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saindo...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}