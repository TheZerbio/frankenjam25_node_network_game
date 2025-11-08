using UnityEngine;

public class FogOfWar : MonoBehaviour
{
	public static FogOfWar Instance;
	[SerializeField] RenderTexture fogMaskRT;     // Assign in Inspector
	[SerializeField] Material drawRevealMat;      // Simple blit material to draw circles
	static readonly int FogMaskID = Shader.PropertyToID("_FogMaskTex");

	void OnEnable()
	{
		// Bind globally so the fullscreen shader can pick it up
		Shader.SetGlobalTexture(FogMaskID, fogMaskRT);
	}

	public void RevealAtUV(Vector2 uv, float radius)
	{
		// Set parameters for your draw material
		drawRevealMat.SetVector("_CenterUV", new Vector4(uv.x, uv.y, 0, 0));
		drawRevealMat.SetFloat("_Radius", radius);

		// Draw a soft circle into the mask: black stays fogged; we draw white to reveal
		var temp = RenderTexture.GetTemporary(fogMaskRT.width, fogMaskRT.height, 0, fogMaskRT.format);
		Graphics.Blit(fogMaskRT, temp);                      // Copy current mask
		Graphics.Blit(temp, fogMaskRT, drawRevealMat);       // Composite reveal
		RenderTexture.ReleaseTemporary(temp);
	}
}
