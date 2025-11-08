using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.RenderGraphModule.Util.RenderGraphUtils;

public class FogOfWarRenderFeature : ScriptableRendererFeature
{
	class FogOfWarPass : ScriptableRenderPass
	{
		private readonly Material fogMaterial;

		public FogOfWarPass(Material material)
		{
			fogMaterial = material;
			renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			Debug.Log("Recording FogOfWarPass");
			UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
			TextureHandle colorTarget = resourceData.activeColorTexture;

			renderGraph.AddBlitPass(new BlitMaterialParameters(colorTarget, colorTarget, fogMaterial, 0), "FogOfWarPass");
		}
	}

	public Material fogMaterial;
	FogOfWarPass fogPass;

	public override void Create()
	{
		fogPass = new FogOfWarPass(fogMaterial);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(fogPass);
	}
}
