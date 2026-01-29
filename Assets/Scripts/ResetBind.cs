using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneReloadHotkey : MonoBehaviour
{
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if ((keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed)
            && keyboard.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
