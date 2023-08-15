
//=======================================================
// 作者：hannibal
// 时间：2023-08-15 21:22:55
// 描述：
//=======================================================
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class CustomShowFont : MonoBehaviour
    {
        public Font font;
        public string str = "Hello World";
        public int fontSize = 40;
        public Color fontColor = Color.white;
        Mesh mesh;

        private void Start()
        {
            Font.textureRebuilt += OnFontTextureRebuilt;

            mesh = new Mesh();
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = font.material;

            RebuildMesh();
        }
        private void OnDestroy()
        {
            Font.textureRebuilt -= OnFontTextureRebuilt;
        }
        private void OnFontTextureRebuilt(Font changedFont)
        {
            if (changedFont != font)
                return;

            RebuildMesh();
        }
        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            RebuildMesh();
        }

        private void RebuildMesh()
        {
            if (mesh == null) return;

            font.RequestCharactersInTexture(str, fontSize);
            mesh.Clear();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uv = new List<Vector2>();
            List<Color> color = new List<Color>();

            DrawText(vertices, triangles, uv, color, fontColor, Vector3.zero, 0);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.colors = color.ToArray();
        }

        private void DrawText(List<Vector3> vertices, List<int> triangles, List<Vector2> uv, List<Color> colorList, Color color, Vector3 offset, int index)
        {

            Vector3 pos = Vector3.zero - offset;
            font.RequestCharactersInTexture(str, fontSize);
            for (int i = 0; i < str.Length; i++)
            {
                CharacterInfo ch;
                font.GetCharacterInfo(str[i], out ch, fontSize);

                vertices.Add(pos + new Vector3(ch.minX, ch.maxY, 0) * 0.1f);
                vertices.Add(pos + new Vector3(ch.maxX, ch.maxY, 0) * 0.1f);
                vertices.Add(pos + new Vector3(ch.maxX, ch.minY, 0) * 0.1f);
                vertices.Add(pos + new Vector3(ch.minX, ch.minY, 0) * 0.1f);

                colorList.Add(color);
                colorList.Add(color);
                colorList.Add(color);
                colorList.Add(color);

                uv.Add(ch.uvTopLeft);
                uv.Add(ch.uvTopRight);
                uv.Add(ch.uvBottomRight);
                uv.Add(ch.uvBottomLeft);

                triangles.Add(4 * (i + index * str.Length) + 0);
                triangles.Add(4 * (i + index * str.Length) + 1);
                triangles.Add(4 * (i + index * str.Length) + 2);

                triangles.Add(4 * (i + index * str.Length) + 0);
                triangles.Add(4 * (i + index * str.Length) + 2);
                triangles.Add(4 * (i + index * str.Length) + 3);

                pos += new Vector3(ch.advance * 0.1f, 0, 0);
            }
        }
    }
}
