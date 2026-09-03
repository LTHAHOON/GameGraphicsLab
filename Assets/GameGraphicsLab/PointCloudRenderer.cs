using UnityEngine;
using UnityEngine.VFX;

public class PointCloudRenderer : MonoBehaviour
{
    private Texture2D _texColor;
    private Texture2D _texPosScale;
    private VisualEffect vfx;
    private uint _resolution = 2048;
    private bool _toUpdate = false;

    [SerializeField] 
    private float spacing = 0f;
    [SerializeField] 
    private int targetParticleCount = 1000;
    [SerializeField]
    private uint _particleCount = 0;
    [SerializeField]
    private MeshFilter _meshFilter;
    [SerializeField]
    private float _particleSize = 0.1f;

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
            vfx.SetUInt(Shader.PropertyToID("ParticleCount"), _particleCount);
            vfx.SetTexture(Shader.PropertyToID("TexColor"), _texColor);
            vfx.SetTexture(Shader.PropertyToID("TexPosScale"), _texPosScale);
            vfx.SetUInt(Shader.PropertyToID("Resolution"), _resolution);
            vfx.Reinit();
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


        // sourcef를 균일한 간격으로 인덱스를 구하고 result에 넣는다. 
        // offset이 0이 아니면 pos를 정규화한 방향으로 offset만큼 이동시킨다.(offset이 클수록 앞으로 멀어짐)
        for (int i = 0; i < sampleCount; i++)
        {
            int index = Mathf.FloorToInt(((float)i / sampleCount) * source.Length);
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

    //VFX에 전달할 텍스처를 생성하고, positions와 colors를 텍스처에 넣는다.
    public void SetParticles(Vector3[] positions, Color[] colors)
    {
        _texColor = new(
            positions.Length > (int)_resolution ? (int)_resolution : positions.Length,
            Mathf.Clamp(positions.Length / (int)_resolution, 1, (int)_resolution),
            TextureFormat.RGBAFloat,
            false
            );
        _texPosScale = new(
            positions.Length > (int)_resolution ? (int)_resolution : positions.Length,
            Mathf.Clamp(positions.Length / (int)_resolution, 1, (int)_resolution),
            TextureFormat.RGBAFloat,
            false
            );

        int texWidth = _texColor.width;
        int texHeight = _texColor.height;

        int index = 0;
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                //2D 텍스처의 좌표를 1D 배열의 인덱스로 변환(다음 행 넘어갈때 몇칸 뛰는지 생각하면 이해하기 쉬움)
                //int index = x + y * texWidth;
                if (index >= positions.Length)
                {
                    break;
                }

                _texColor.SetPixel(x, y, colors[index]);
                Color data = new(positions[index].x, positions[index].y, positions[index].z, _particleSize);
                _texPosScale.SetPixel(x, y, data);
                index++;
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
