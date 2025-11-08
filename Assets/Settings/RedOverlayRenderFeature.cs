using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class RedOverlayRenderFeature : ScriptableRendererFeature
{
	class RedOverlayPass : ScriptableRenderPass
	{
		private readonly Material redMaterial;

		public RedOverlayPass(Material material)
		{
			redMaterial = material;
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			var resourceData = frameData.Get<UniversalResourceData>();
			var cameraData = frameData.Get<UniversalCameraData>();
			var desc = cameraData.cameraTargetDescriptor;
			desc.msaaSamples = 1;
			desc.depthBufferBits = 0;

			// Create a temporary texture to hold the result
			var destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "RedOverlayTexture", false);

			// Add the blit pass
			renderGraph.AddBlitPass(
				new RenderGraphUtils.BlitMaterialParameters(
					resourceData.activeColorTexture,
					destination,
					redMaterial,
					0
				),
				"Red"
			);
		}

		private class PassData
		{
			internal TextureHandle src;
			internal TextureHandle dst;
			internal Material blitMaterial;
		}
	}

	public Material redOverlayMaterial;
	RedOverlayPass redOverlayPass;

	public override void Create()
	{
		redOverlayPass = new RedOverlayPass(redOverlayMaterial)
		{
			renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
		};
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(redOverlayPass);
	}
}
