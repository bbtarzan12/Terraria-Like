
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XNode.Noise.Simple
{

	public class ColorNode : NoiseNode
	{
		[Output] public NoiseNode output;
		public Color color;

		public override Texture2D GetTexture()
		{
			var texture = new Texture2D(GetGraph.mapSize.x, GetGraph.mapSize.y, TextureFormat.RGBA32, false);
			var fillColorArray = texture.GetPixels();
			for (int i = 0; i < fillColorArray.Length; i++)
				fillColorArray[i] = color;
			
			texture.SetPixels(fillColorArray);
			texture.Apply();
			return texture;
		}

		public override Texture2D GenerateTexture()
		{
			if (!Dirty)
				return HasTexture ? Texture : GenerateTexture();
			
			Texture = new Texture2D(GetGraph.mapSize.x, GetGraph.mapSize.y, TextureFormat.RGBA32, false);
			var fillColorArray = Texture.GetPixels();
			for (int i = 0; i < fillColorArray.Length; i++)
				fillColorArray[i] = color;
			
			Texture.SetPixels(fillColorArray);
			Texture.Apply();
			GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
			Dirty = false;
			
			return Texture;
		}
	}
}