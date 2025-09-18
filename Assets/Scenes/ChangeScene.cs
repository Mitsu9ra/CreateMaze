using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public static int flag = 0;
    public void change_button()
    {
        flag = 1;
        SceneManager.LoadScene("MazeScene");

    }
    public void change_button2()
    {
        flag = 2;
        SceneManager.LoadScene("MazeScene");

    }
    public void change_button3()
    {
        flag = 3;
        SceneManager.LoadScene("MazeScene");

    }
    public void change_button4()
    {
        flag = 4;
        SceneManager.LoadScene("MazeScene");

    }
    public void change_button5()
    {
        flag = 5;
        SceneManager.LoadScene("MazeScene");

    }
}
