﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XNode.Noise.Procedural
{

	[Serializable]
	public class TerrainNode : NoiseNode
	{
		
		[Output] public NoiseNode output;
		public float x;
		public float height = 0.5f;
		public float scale = 10;
		[Range(1, 10)] public int fractal = 1;
		public float smooth = 1;

		protected override void Init()
		{
			shader = Shader.Find("Noise/Terrain");
			base.Init();
		}

		public override Texture2D GetTexture()
		{
			shader = Shader.Find("Noise/Terrain");
			Material noiseMaterial = new Material(shader);
			noiseMaterial.SetFloat("_X", x);
			noiseMaterial.SetFloat("_Height", height);
			noiseMaterial.SetFloat("_Scale", scale);
			noiseMaterial.SetInt("_Fractal", fractal);
			noiseMaterial.SetFloat("_Ratio", GetGraph.Ratio);
			noiseMaterial.SetFloat("_Smooth", smooth);
			return TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
		}

		public override Texture2D GenerateTexture()
		{
			if (!IsShaderInit)
				return Texture2D.whiteTexture;

			if (!Dirty)
				return HasTexture ? Texture : GenerateTexture();

			Material noiseMaterial = new Material(shader);
			noiseMaterial.SetFloat("_X", x);
			noiseMaterial.SetFloat("_Height", height);
			noiseMaterial.SetFloat("_Scale", scale);
			noiseMaterial.SetInt("_Fractal", fractal);
			noiseMaterial.SetFloat("_Ratio", GetGraph.Ratio);
			noiseMaterial.SetFloat("_Smooth", smooth);

			Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
			GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
			Dirty = false;

			return Texture;
		}
	}
}