using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class FieldOfViewMesh : MonoBehaviour
{
    public float viewRadius = 8f;
    [Range(1, 360)]
    public float viewAngle = 90f;
    public int rayCount = 100;
    public LayerMask obstacleMask;

    Mesh mesh;
    Vector3 origin;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void LateUpdate()
    {
        origin = transform.position;
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        float angleStep = viewAngle / rayCount;
        float angle = -viewAngle / 2f;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(Vector3.zero);

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 dir = DirFromAngle(angle);
            Vector3 vertex;

            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, viewRadius, obstacleMask))
            {
                vertex = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                vertex = transform.InverseTransformPoint(origin + dir * viewRadius);
            }

            vertices.Add(vertex);

            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }

            angle += angleStep;
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    Vector3 DirFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
    }
}
