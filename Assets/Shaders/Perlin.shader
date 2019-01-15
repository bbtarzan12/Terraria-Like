Shader "Noise/Perlin"
{
    Properties
    {
        _X ("X Seed", float) = 1
        _Y ("Y Seed", float) = 1
        _Scale ("Scale", float) = 1
        _Fractal ("Fractal", int) = 1
        _Threshold ("Threshold", float) = 1
        _Texture ("Before Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        
        
        Pass
        {
            CGPROGRAM
        
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "PerlinNoise.hlsl"
            
            float _X;
            float _Y;
            float _Scale;
            float _Threshold;
            int _Fractal;
            sampler2D _Texture;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 texUV = i.uv;
                float2 uv = i.uv + float2(_X, _Y);
                float o = 0.5;
                float s = _Scale;       
                float w = 0.5;
        
                for (int i = 0; i < _Fractal; i++)
                {
                    float2 coord = uv * s;

                    o += cnoise(coord) * w;
        
                    s *= 2.0;
                    w *= 0.5;
                }
                
                o = lerp(tex2D(_Texture, texUV), o, _Threshold);
                return float4(o, o, o, 1);
            }
        
            ENDCG
        }
    }
}
