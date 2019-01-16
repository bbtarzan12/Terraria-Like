using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XNode.Noise
{

	[Serializable]
	public class TerrainNode : NoiseNode
	{
		
		[Output] public NoiseNode output;
		public float x;
		public float height;
		public float scale;
		public int fractal;

		protected override void Init()
		{
			base.Init();
			shader = Shader.Find("Noise/Terrain");
		}

		public override Texture2D GetTexture()
		{
			Material noiseMaterial = new Material(shader);
			noiseMaterial.SetFloat("_X", x);
			noiseMaterial.SetFloat("_Height", height);
			noiseMaterial.SetFloat("_Scale", scale);
			noiseMaterial.SetInt("_Fractal", fractal);
			return TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
		}

		public override void GenerateTexture()
		{            
			
			if (!IsShaderInit)
				return;

			if (!Dirty)
				return;

			Material noiseMaterial = new Material(shader);
			noiseMaterial.SetFloat("_X", x);
			noiseMaterial.SetFloat("_Height", height);
			noiseMaterial.SetFloat("_Scale", scale);
			noiseMaterial.SetInt("_Fractal", fractal);

			Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
			GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
			Dirty = false;
		}

		public override void Update() => Dirty = true;
	}
}