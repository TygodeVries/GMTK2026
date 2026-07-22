using UnityEngine;

public class BuildingBoundingBox : MonoBehaviour
{
    public Vector2 Size;
    public float minimum_distance = 0.1f;
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

    public Vector2 Reproject(Vector2 x)
    {
        Vector4 lx = transform.worldToLocalMatrix * new Vector4(x.x, 0, x.y, 1);

        if (Mathf.Abs(lx.x) < Size.x / 2 && Mathf.Abs(lx.z) < Size.y / 2)
        {
            // Project out of the bounding box
            if (Mathf.Abs(lx.x / Size.x) > Mathf.Abs(lx.z / Size.y))
            {
                lx.x = Mathf.Sign(lx.x) * (Size.x / 2 + minimum_distance);
            }
            else
            {
                lx.z = Mathf.Sign(lx.z) * (Size.y / 2 + minimum_distance);
            }

            Vector3 tx = transform.localToWorldMatrix * lx;

            Vector2 rx = new Vector2(tx.x, tx.z);
            return rx;
        }
        return x;

    }
}
