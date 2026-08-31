using UnityEngine;
using UnityEngine.VFX;

public class PointCloudRenderer : MonoBehaviour
{
    Texture2D _texColor;
    Texture2D _texPosScale;
    VisualEffect vfx;
    uint _resolution = 2048;

    public float _particleSize = 0.1f;
    [SerializeField] private float spacing = 0f;
    [SerializeField] private int targetParticleCount = 1000;
    bool _toUpdate = false;
    [SerializeField]
    private uint _particleCount = 0;
    [SerializeField]
    private MeshFilter _meshFilter;

    private void Start()
    {
        vfx = GetComponent<VisualEffect>();

        if (_meshFilter == null || _meshFilter.sharedMesh == null)
        {
            return;
        }

        Vector3[] vertices = _meshFilter.sharedMesh.vertices;
        Vector3[] sampled = SampleVertices(vertices, targetParticleCount, spacing);
        Color[] colors = new Color[sampled.Length];

        for (int x = 0; x < sampled.Length; x++)
        {
            colors[x] = new Color(Random.value, Random.value, Random.value, 1f);
        }

        SetParticles(sampled, colors);
    }

    private void Update()
    {
        if (_toUpdate)
        {
            _toUpdate = false;
            vfx.Reinit();
            vfx.SetUInt(Shader.PropertyToID("ParticleCount"), _particleCount);
            vfx.SetTexture(Shader.PropertyToID("TexColor"), _texColor);
            vfx.SetTexture(Shader.PropertyToID("TexPosScale"), _texPosScale);
            vfx.SetUInt(Shader.PropertyToID("Resolution"), _resolution);
        }
    }

    private Vector3[] SampleVertices(Vector3[] source, int count, float offset)
    {
        if (source == null || source.Length == 0 || count <= 0)
        {
            return new Vector3[0];
        }

        int sampleCount = Mathf.Min(count, source.Length);
        Vector3[] result = new Vector3[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            int index = Mathf.FloorToInt((float)i / sampleCount * source.Length);
            Vector3 pos = source[index];
            if (offset != 0f)
            {
                Vector3 dir = pos.sqrMagnitude > 0.0001f ? pos.normalized : Vector3.one.normalized;
                pos += dir * offset;
            }

            result[i] = pos;
        }

        return result;
    }

    public void SetParticles(Vector3[] positions, Color[] colors)
    {
        _texColor = new(positions.Length > (int)_resolution ? (int)_resolution : positions.Length, Mathf.Clamp(positions.Length / (int)_resolution, 1, (int)_resolution), TextureFormat.RGBAFloat, false);
        _texPosScale = new(positions.Length > (int)_resolution ? (int)_resolution : positions.Length, Mathf.Clamp(positions.Length / (int)_resolution, 1, (int)_resolution), TextureFormat.RGBAFloat, false);
        int texWidth = _texColor.width;
        int texHeight = _texColor.height;

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int index = x + y * texWidth;
                if (index >= positions.Length)
                {
                    break;
                }

                _texColor.SetPixel(x, y, colors[index]);
                Color data = new(positions[index].x, positions[index].y, positions[index].z, _particleSize);
                _texPosScale.SetPixel(x, y, data);
            }
        }

        _texColor.Apply();
        _texPosScale.Apply();
        _particleCount = (uint)positions.Length;
        _toUpdate = true;
    }

    public void SetSpacing(float value)
    {
        spacing = value;
        if (_meshFilter == null || _meshFilter.sharedMesh == null)
        {
            return;
        }

        Vector3[] vertices = _meshFilter.sharedMesh.vertices;
        Vector3[] sampled = SampleVertices(vertices, targetParticleCount, spacing);
        Color[] colors = new Color[sampled.Length];

        for (int i = 0; i < sampled.Length; i++)
        {
            colors[i] = new Color(Random.value, Random.value, Random.value, 1f);
        }

        SetParticles(sampled, colors);
    }

    public void SetParticleCount(int count)
    {
        targetParticleCount = Mathf.Max(1, count);

        if (_meshFilter == null || _meshFilter.sharedMesh == null)
        {
            return;
        }

        Vector3[] vertices = _meshFilter.sharedMesh.vertices;
        Vector3[] sampled = SampleVertices(vertices, targetParticleCount, spacing);
        Color[] colors = new Color[sampled.Length];

        for (int i = 0; i < sampled.Length; i++)
        {
            colors[i] = new Color(Random.value, Random.value, Random.value, 1f);
        }

        SetParticles(sampled, colors);
    }
}
