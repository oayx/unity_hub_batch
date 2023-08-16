
//=======================================================
// 作者：hannibal
// 时间：2023-08-15 21:22:55
// 描述：FreeType字库结构
//
//                       xmin                     xmax
//                        |                         |
//                        |<-------- width -------->|
//                        |                         |
//              |         +-------------------------+----------------- ymax
//              |         |    ggggggggg   ggggg    |     ^        ^
//              |         |   g:::::::::ggg::::g    |     |        |
//              |         |  g:::::::::::::::::g    |     |        |
//              |         | g::::::ggggg::::::gg    |     |        |
//              |         | g:::::g     g:::::g     |     |        |
//    offsetX  -|-------->| g:::::g     g:::::g     |  offsetY     |
//              |         | g:::::g     g:::::g     |     |        |
//              |         | g::::::g    g:::::g     |     |        |
//              |         | g:::::::ggggg:::::g     |     |        |
//              |         |  g::::::::::::::::g     |     |      height
//              |         |   gg::::::::::::::g     |     |        |
//  baseline ---*---------|---- gggggggg::::::g-----*--------      |
//            / |         |             g:::::g     |              |
//     origin   |         | gggggg      g:::::g     |              |
//              |         | g:::::gg   gg:::::g     |              |
//              |         |  g::::::ggg:::::::g     |              |
//              |         |   gg:::::::::::::g      |              |
//              |         |     ggg::::::ggg        |              |
//              |         |         gggggg          |              v
//              |         +-------------------------+----------------- ymin
//              |                                   |
//              |------------- advanceX ----------->|
//=======================================================
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace YX
{
    public class HUDBatch : MonoBehaviour
    {
        [Header("名称")]
        public Font NameFont;
        public int FontSize = 40;
        public Color FontColor = Color.white;
        [SerializeField]
        private string _name;

        [Header("进度条")]
        public float ProgressWidth = 1f;
        public float ProgressHeight = 0.5f;
        public float ProgressYOffset = 1f;
        [SerializeField]
        private float _progress = 1f;

        private Mesh _mesh;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<Color> _colors = new List<Color>();
        private List<Vector2> _uvs = new List<Vector2>();
        private List<int> _triangles = new List<int>();

        private void Start()
        {
            Font.textureRebuilt += OnFontTextureRebuilt;

            var filter = gameObject.AddComponent<MeshFilter>();
            _mesh = filter.mesh;
            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = LightProbeUsage.Off;
            renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

            renderer.sharedMaterial = Resources.Load<Material>("HUDBatch");
            renderer.sharedMaterial.SetTexture("_FontTex", NameFont.material.mainTexture);

            RebuildMesh();
        }
        private void OnDestroy()
        {
            Font.textureRebuilt -= OnFontTextureRebuilt;
        }
        private void OnFontTextureRebuilt(Font changedFont)
        {
            if (changedFont != NameFont)
                return;

            RebuildMesh();
        }
        private void RebuildMesh()
        {
            NameFont.RequestCharactersInTexture(_name, FontSize);
            _mesh.Clear();
            _vertices.Clear();
            _colors.Clear();
            _uvs.Clear();
            _triangles.Clear();
            {
                InitProgress();
                InitName();
            }
            _mesh.vertices = _vertices.ToArray();
            _mesh.colors = _colors.ToArray();
            _mesh.uv = _uvs.ToArray();
            _mesh.SetTriangles(_triangles, 0);
        }
        private void InitProgress()
        {
            int indexOffset = _vertices.Count;
            Vector3 pos = Vector3.up * ProgressYOffset;

            _vertices.Add(pos + new Vector3(0, -ProgressHeight * 0.5f, 0));
            _vertices.Add(pos + new Vector3(ProgressWidth * 0.5f, -ProgressHeight * 0.5f, 0));
            _vertices.Add(pos + new Vector3(0, ProgressHeight * 0.5f, 0));
            _vertices.Add(pos + new Vector3(ProgressWidth * 0.5f, ProgressHeight * 0.5f, 0));
            _vertices.Add(_vertices[1]);
            _vertices.Add(pos + new Vector3(ProgressWidth, -ProgressHeight * 0.5f, 0));
            _vertices.Add(_vertices[3]);
            _vertices.Add(pos + new Vector3(ProgressWidth, ProgressHeight * 0.5f, 0));

            _uvs.Add(new Vector2(0f, 0f));
            _uvs.Add(new Vector2(1f, 0f));
            _uvs.Add(new Vector2(0f, 1f));
            _uvs.Add(new Vector2(1f, 1f));
            _uvs.Add(new Vector2(0f, 0f));
            _uvs.Add(new Vector2(1f, 0f));
            _uvs.Add(new Vector2(0f, 1f));
            _uvs.Add(new Vector2(1f, 1f));

            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 4; i++)
                {//通过顶点颜色的alpha识别是进度条还是文字
                    _colors.Add((j == 1 ? Color.grey : Color.red) + new Color(0,0,0,0.1f));
                }
            }

            for (int i = 0; i < 8; i = i + 4)
            {
                _triangles.Add(indexOffset + i);
                _triangles.Add(indexOffset + i + 2);
                _triangles.Add(indexOffset + i + 1);

                _triangles.Add(indexOffset + i + 1);
                _triangles.Add(indexOffset + i + 2);
                _triangles.Add(indexOffset + i + 3);
            }

            SetProgress(_progress);
        }

        private void InitName()
        {
            int indexOffset = _vertices.Count;

            float size = FontSize * 0.001f;
            Vector3 pos = Vector3.zero;
            NameFont.RequestCharactersInTexture(_name, FontSize);
            for (int i = 0; i < _name.Length; i++)
            {
                NameFont.GetCharacterInfo(_name[i], out var ch, FontSize);

                _vertices.Add(pos + new Vector3(ch.minX, ch.maxY, 0) * size);
                _vertices.Add(pos + new Vector3(ch.maxX, ch.maxY, 0) * size);
                _vertices.Add(pos + new Vector3(ch.maxX, ch.minY, 0) * size);
                _vertices.Add(pos + new Vector3(ch.minX, ch.minY, 0) * size);

                _colors.Add(Color.blue);
                _colors.Add(Color.blue);
                _colors.Add(Color.blue);
                _colors.Add(Color.blue);

                _uvs.Add(ch.uvTopLeft);
                _uvs.Add(ch.uvTopRight);
                _uvs.Add(ch.uvBottomRight);
                _uvs.Add(ch.uvBottomLeft);

                _triangles.Add(indexOffset + 4 * i + 0);
                _triangles.Add(indexOffset + 4 * i + 1);
                _triangles.Add(indexOffset + 4 * i + 2);
                _triangles.Add(indexOffset + 4 * i + 0);
                _triangles.Add(indexOffset + 4 * i + 2);
                _triangles.Add(indexOffset + 4 * i + 3);
                pos += new Vector3(ch.advance * size, 0, 0);
            }
        }
        /// <summary>
        /// 设置名称，这个接口只能在Start前执行；否则需要修改InitName的构建方式
        /// </summary>
        public void SetName(string name)
        {
            if (_mesh != null)
                throw new System.Exception("名字必须在Start前执行");
            _name = name;
        }
        /// <summary>
        /// 设置进度
        /// </summary>
        public void SetProgress(float v)
        {
            _progress = v;
            if (_mesh == null)
                return;

            Vector3 pos = Vector3.up * ProgressYOffset;
            float val = Mathf.Lerp(0, ProgressWidth, v);
            _vertices[1] = pos + new Vector3(val, -ProgressHeight * 0.5f, 0);
            _vertices[3] = pos + new Vector3(val, ProgressHeight * 0.5f, 0);
            _vertices[4] = _vertices[1];
            _vertices[6] = _vertices[3];
            _mesh.vertices = _vertices.ToArray();
        }
    }
}
namespace YX.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(HUDBatch))]
    public class HUDBatchInspector : UnityEditor.Editor
    {
        private float _progress = 1;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            _progress = EditorGUILayout.Slider(_progress, 0, 1);
            if (EditorGUI.EndChangeCheck())
            {
                (target as HUDBatch).SetProgress(_progress);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}