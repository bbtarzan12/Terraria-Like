using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XNode.Noise.Simple
{

	[Serializable]
	public class GradientNode : NoiseNode
	{
		public enum GradientMode { Angled, Radial }
		
		[Output] public NoiseNode output;
		public Vector2 start = new Vector2(0, 1.0f);
		public Vector2 end = new Vector2(0, 0);
		public GradientMode mode;

		protected override void Init()
		{
			shader = Shader.Find("Noise/Gradient");
			base.Init();
		}

		public override Texture2D GetTexture()
		{
			shader = Shader.Find("Noise/Gradient");
			Material noiseMaterial = new Material(shader);
			noiseMaterial.SetVector("_Pos", new Vector4(start.x, start.y, end.x, end.y));
			noiseMaterial.SetInt("_Mode", (int)mode);
			return TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
		}

		public override Texture2D GenerateTexture()
		{
			if (!IsShaderInit)
				return Texture2D.whiteTexture;

			if (!Dirty)
				return HasTexture ? Texture : GenerateTexture();

			Material noiseMaterial = new Material(shader);
			noiseMaterial.SetVector("_Pos", new Vector4(start.x, start.y, end.x, end.y));
			noiseMaterial.SetInt("_Mode", (int)mode);

			Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
			GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
			Dirty = false;
			
			return Texture;
		}
	}
}