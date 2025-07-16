using UnityEngine;

public class Turn : MonoBehaviour
{
    // 色と半径を設定
    public Color gizmoColor = Color.red;
    public float radius = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
