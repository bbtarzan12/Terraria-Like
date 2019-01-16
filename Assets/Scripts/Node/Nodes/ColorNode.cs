﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XNode.Noise
{

	public class ColorNode : NoiseNode
	{
		[Output] public NoiseNode output;
		public Color fillColor;

		public override Texture2D GetTexture()
		{
			var texture = new Texture2D(GetGraph.mapSize.x, GetGraph.mapSize.y, TextureFormat.RGBA32, false);
			var fillColorArray = texture.GetPixels();
			for (int i = 0; i < fillColorArray.Length; i++)
				fillColorArray[i] = fillColor;
			
			texture.SetPixels(fillColorArray);
			texture.Apply();
			return texture;
		}

		public override void GenerateTexture()
		{            
			if (!Dirty)
				return;
			
			Texture = new Texture2D(GetGraph.mapSize.x, GetGraph.mapSize.y, TextureFormat.RGBA32, false);
			var fillColorArray = Texture.GetPixels();
			for (int i = 0; i < fillColorArray.Length; i++)
				fillColorArray[i] = fillColor;
			
			Texture.SetPixels(fillColorArray);
			Texture.Apply();
			GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
			Dirty = false;
		}

		public override void Update() => Dirty = true;
	}
}