using UnityEngine;

public class ExitScript : MonoBehaviour
{
    void Update()
    {
        //If escape (or the Android back button) is pressed
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //Exit the application
            Application.Quit();
        }
    }
}
