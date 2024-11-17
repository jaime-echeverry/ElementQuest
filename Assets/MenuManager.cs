using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para cargar escenas
using UnityEngine.UI;  // Si tienes botones UI, necesitarás esta biblioteca

public class MenuManager : MonoBehaviour
{
    public string PlayScene, OptionScene;
    // Método para cargar la escena del juego
    public void PlayGame()
    {
        // Aquí puedes cambiar "GameScene" por el nombre de tu escena del juego
        SceneManager.LoadScene(PlayScene);
    }

    // Método para cargar la escena de opciones
    public void OpenOptions()
    {
        // Si tienes una escena de opciones
        SceneManager.LoadScene(OptionScene);
    }

    // Método para salir del juego
    public void QuitGame()
    {
        // Si estamos en el editor de Unity, esto no cerrará el juego, pero en una build sí
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
