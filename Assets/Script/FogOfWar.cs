using NUnit.Framework;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
	public static FogOfWar Instance;

	public RenderTexture fogTexture;
	public Material fogMaterial;
	public float revealRadius = 5f;

	private List<FogRevealer> units = new();
	private Texture2D visibilityMap;

	void Start()
	{
		visibilityMap = new Texture2D(fogTexture.width, fogTexture.height, TextureFormat.R8, false);
		ClearFog();
	}

	void Update()
	{
		ClearFog();
		foreach (FogRevealer unit in units)
		{
			RevealArea(unit.transform.position);
		}
		ApplyFog();
	}

	void ClearFog()
	{
		Color32[] pixels = visibilityMap.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
			pixels[i] = new Color32(0, 0, 0, 255); // Full fog
		visibilityMap.SetPixels32(pixels);
	}

	void RevealArea(Vector3 worldPos)
	{
		Vector2Int texPos = WorldToTextureCoords(worldPos);
		int radius = Mathf.RoundToInt(revealRadius * fogTexture.width / 100f);

		for (int y = -radius; y <= radius; y++)
		{
			for (int x = -radius; x <= radius; x++)
			{
				int px = texPos.x + x;
				int py = texPos.y + y;
				if (px >= 0 && px < visibilityMap.width && py >= 0 && py < visibilityMap.height)
				{
					float dist = Mathf.Sqrt(x * x + y * y);
					if (dist <= radius)
						visibilityMap.SetPixel(px, py, Color.white); // Visible
				}
			}
		}
	}

	Vector2Int WorldToTextureCoords(Vector3 worldPos)
	{
		// Convert world position to texture coordinates (customize based on your map size)
		float normalizedX = Mathf.InverseLerp(-50, 50, worldPos.x);
		float normalizedY = Mathf.InverseLerp(-50, 50, worldPos.y);
		return new Vector2Int(
			Mathf.RoundToInt(normalizedX * visibilityMap.width),
			Mathf.RoundToInt(normalizedY * visibilityMap.height)
		);
	}

	void ApplyFog()
	{
		visibilityMap.Apply();
		Graphics.Blit(visibilityMap, fogTexture);
		fogMaterial.SetTexture("_FogTex", fogTexture);
	}

	public void AddUnit(FogRevealer unit)
	{
		if (!units.Contains(unit))
			units.Add(unit);
	}

	public void RemoveUnit(FogRevealer unit)
	{
		if (units.Contains(unit))
			units.Remove(unit);
	}
}
