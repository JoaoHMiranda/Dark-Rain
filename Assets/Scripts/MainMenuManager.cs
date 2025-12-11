using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuração")]
    public string gameSceneName = "SampleScene"; // O nome da sua cena de jogo
    
    [Header("Telas")]
    public GameObject tutorialPanel; // O painel preto do tutorial
    public GameObject menuPrincipal; // O grupo com os botões JOGAR/SAIR

    public void PlayGame()
    {
        // GARANTE QUE O TEMPO ESTÁ RODANDO ANTES DE ENTRAR
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenTutorial()
    {
        // Troca as telas: Esconde o Menu, Mostra o Tutorial
        menuPrincipal.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        // Destroca: Mostra o Menu, Esconde o Tutorial
        menuPrincipal.SetActive(true);
        tutorialPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do Jogo...");
        Application.Quit(); // Fecha o jogo real (.exe)

        // Esse código abaixo faz o Editor da Unity parar de rodar (O botão Play desliga)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}