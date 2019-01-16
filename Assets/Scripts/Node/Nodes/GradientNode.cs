using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace XNode.Noise
{

	[Serializable]
	public class GradientNode : NoiseNode
	{
		public enum GradientMode { Angled, Radial }
		
		[Output] public NoiseNode output;
		
		public Color color;
		public Vector2 start;
		public Vector2 end;
		public GradientMode mode;

		protected override void Init()
		{
			base.Init();
			shader = Shader.Find("Noise/Gradient");
		}

		public override Texture2D GetTexture()
		{
			Material noiseMaterial = new Material(shader);
			noiseMaterial.SetVector("_Pos", new Vector4(start.x, start.y, end.x, end.y));
			noiseMaterial.SetColor("_Color", color);
			noiseMaterial.SetInt("_Mode", (int)mode);
			return TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
		}

		public override void GenerateTexture()
		{            
			
			if (!IsShaderInit)
				return;

			if (!Dirty)
				return;

			Material noiseMaterial = new Material(shader);
			noiseMaterial.SetVector("_Pos", new Vector4(start.x, start.y, end.x, end.y));
			noiseMaterial.SetColor("_Color", color);
			noiseMaterial.SetInt("_Mode", (int)mode);

			Texture = TextureMaker.Generate(GetGraph.mapSize, noiseMaterial);
            
			GetPort(nameof(output))?.GetConnections()?.ForEach(f => ((NoiseNode)f.node)?.SetTextureDirty());
			Dirty = false;
		}

		public override void Update() => Dirty = true;
	}
}