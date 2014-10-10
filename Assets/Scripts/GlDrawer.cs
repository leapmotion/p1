using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace P1
{
  public class Line
  {
    public Vector3 startPoint { get; set; }

    public Vector3 endPoint { get; set; }

    public Color color { get; set; }

    public Line (Vector3 startPoint, Vector3 endPoint, Color color)
    {
      this.startPoint = startPoint;
      this.endPoint = endPoint;
      this.color = color;
    }
  }

  public class Circle
  {
    public Vector3 center { get; set; }

    public Vector3 normal { get; set; }

    public float radius { get; set; }

    public Color color { get; set; }

    public Circle (Vector3 center, Vector3 normal, float radius, Color color)
    {
      this.center = center;
      this.normal = normal;
      this.radius = radius;
      this.color = color;
    }
  }

  public class GlDrawer : MonoBehaviour
  {

    public static GlDrawer instance = null;
    private List<Line> lines = new List<Line> ();
    private List<Circle> circles = new List<Circle> ();
    private Color default_color = new Color (0.1f, 0.1f, 0.1f, 0.1f);

    public void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color = default(Color))
    {
      if (color.a == 0.0f) {
        color = default_color;
      }
      circles.Add(new Circle (center, normal, radius, color));
    }

    public void DrawLine(Vector3 a, Vector3 b, Color color = default(Color))
    {
      if (color.a == 0.0f) {
        color = default_color;
      }
      lines.Add(new Line (a, b, color));
    }

    public void DrawWireCube(Vector3 position, Quaternion rotation, Vector3 size, Color color = default(Color))
    {
      // Generate transformed vertices.
      Vector3[] verts = new Vector3[8];
      for (int i = 0; i < 8; i++) {
        verts [i] = Vector3.Scale(0.5f * size, new Vector3 ((i / 4) * 2 - 1, (i / 2 % 2) * 2 - 1, (i % 2) * 2 - 1));
        verts [i] = position + rotation * verts [i];
      }
    
      // Draw lines
      for (int i = 0; i < 4; i++) {
        DrawLine(verts [i], verts [i + 4], color);
        DrawLine(verts [2 * i], verts [2 * i + 1], color);
        DrawLine(verts [i + (i / 2 * 2)], verts [i + (i / 2 * 2) + 2], color);
      }
    }
  
    Material lineMaterial = null;
  
    Material CreateLineMaterial()
    {
      //Matrial material = new Material("LeapInteract/Materials/Held");
    
      Material material = new Material ("Shader \"Liens/Colored Blended\" {" +
        "SubShader { Pass { " +
        "    Blend SrcAlpha OneMinusSrcAlpha " +
        "    ZWrite Off Cull Off Fog { Mode Off } " +
        "    BindChannels {" +
        "      Bind \"vertex\", vertex Bind \"color\", color }" +
        "} } }");
      material.hideFlags = HideFlags.HideAndDontSave;
      material.shader.hideFlags = HideFlags.HideAndDontSave;
      return material;
    }
  
#region loop 

    // Use this for initialization
    void Start()
    {
      instance = this;
      lineMaterial = CreateLineMaterial();
    }
  
    // Update is called once per frame
    void Update()
    {
    }

    void OnPostRender()
    {
      lineMaterial.SetPass(0);
      GL.PushMatrix();
      GL.Begin(GL.LINES);
      lines.Reverse();
      foreach (Line line in lines) {
        GL.Color(line.color);
        GL.Vertex3(line.startPoint.x, line.startPoint.y, line.startPoint.z);
        GL.Vertex3(line.endPoint.x, line.endPoint.y, line.endPoint.z);
      }
      GL.End();

      foreach (Circle circle in circles) {
        // Create a cross of points
        Vector3 vertex_1 = Vector3.zero;
        if (circle.normal.normalized != Vector3.up)
          vertex_1 = Vector3.Cross(circle.normal, Vector3.up).normalized;
        else
          vertex_1 = Vector3.Cross(circle.normal, Vector3.right).normalized;
        Vector3 vertex_3 = vertex_1 * -1;
        Vector3 vertex_2 = Vector3.Cross(circle.normal, vertex_1).normalized;
        Vector3 vertex_4 = vertex_2 * -1;

        List<Vector3> points = new List<Vector3> ();
        points.Add(circle.center + vertex_1 * circle.radius);
        points.Add(circle.center + vertex_2 * circle.radius);
        points.Add(circle.center + vertex_3 * circle.radius);
        points.Add(circle.center + vertex_4 * circle.radius);
        int iterations = 4;
        for (int curr_iteration = 0; curr_iteration < iterations; ++curr_iteration) {
          for (int i = 1; i < points.Count; i += 2) { // We skip the item we insert at the end
            // Get the vertex between two points. Get the Vector from center to the point normalized then multiplied by radius
            Vector3 new_point = ((points [i - 1] + points [i]) / 2 - circle.center).normalized * circle.radius + circle.center;
            points.Insert(i, new_point);
          }
          Vector3 last_point = ((points [0] + points [points.Count - 1]) / 2 - circle.center).normalized * circle.radius + circle.center;
          points.Add(last_point);
        }

        GL.Begin(GL.LINES);
        GL.Color(circle.color);
        for (int i = 0; i < points.Count - 1; ++i) {
          GL.Vertex3(points [i + 0].x, points [i + 0].y, points [i + 0].z);
          GL.Vertex3(points [i + 1].x, points [i + 1].y, points [i + 1].z);
        }
        GL.Vertex3(points [points.Count - 1].x, points [points.Count - 1].y, points [points.Count - 1].z);
        GL.Vertex3(points [0].x, points [0].y, points [0].z);
        GL.End();
      }

      GL.PopMatrix();
      lines.Clear();
      circles.Clear();
    }
  #endregion
  }
}
