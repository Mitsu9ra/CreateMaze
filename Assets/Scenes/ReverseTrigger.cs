using UnityEngine;

public class ReverseTrigger : MonoBehaviour
{
    public float reverseDuration = 5f; // 通常の反転時間

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GridMovement gm = other.GetComponent<GridMovement>();

            if (gm != null)
            {
                // すでに反転中なら → 反転解除
                if (gm.isReversed)
                {
                    gm.isReversed = false;
                    Debug.Log("🔄 再度反転マスに触れたので操作が元に戻りました！");
                }
                else
                {
                    // 反転していないなら → 一定時間反転
                    gm.ReverseTemporarily(reverseDuration);
                    Debug.Log("🌀 操作が反転しました！");
                }
            }
        }
    }
}
