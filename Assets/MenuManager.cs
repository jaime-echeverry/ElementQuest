using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para cargar escenas
using UnityEngine.UI;  // Si tienes botones UI, necesitar�s esta biblioteca

public class MenuManager : MonoBehaviour
{
    public string PlayScene, OptionScene;
    // M�todo para cargar la escena del juego
    public void PlayGame()
    {
        // Aqu� puedes cambiar "GameScene" por el nombre de tu escena del juego
        SceneManager.LoadScene(PlayScene);
    }

    // M�todo para cargar la escena de opciones
    public void OpenOptions()
    {
        // Si tienes una escena de opciones
        SceneManager.LoadScene(OptionScene);
    }

    // M�todo para salir del juego
    public void QuitGame()
    {
        // Si estamos en el editor de Unity, esto no cerrar� el juego, pero en una build s�
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
