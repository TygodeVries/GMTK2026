using UnityEngine;

public class BuildingBoundingBox : MonoBehaviour
{
    public Vector2 Size;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.Translate(transform.position) * Matrix4x4.Rotate(transform.rotation);
        Gizmos.DrawWireCube(new Vector3(), new Vector3(Size.x, 0, Size.y));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.1f);
        Gizmos.matrix = Matrix4x4.Translate(transform.position) * Matrix4x4.Rotate(transform.rotation);
        Gizmos.DrawCube(new Vector3(), new Vector3(Size.x, 0, Size.y));
    }
}
