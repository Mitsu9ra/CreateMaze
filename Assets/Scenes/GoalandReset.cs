using UnityEngine;

public class Goal : MonoBehaviour
{
    private bool isResetting = false; // ��d���Z�b�g�h�~

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isResetting) return;
        if (other.CompareTag("Player"))
        {
            MazeGenerator_WallExtend_Proper mazeGen = FindObjectOfType<MazeGenerator_WallExtend_Proper>();
            if (mazeGen != null)
            {
                isResetting = true;
                mazeGen.ResetMazeSafe();
                isResetting = false;
            }
        }
    }
}
