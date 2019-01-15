Shader "Noise/Simplex"
{
    Properties
    {
        _X ("X Seed", float) = 1
        _Y ("Y Seed", float) = 1
        _Scale ("Scale", float) = 1
        _Fractal ("Fractal", int) = 1
        _Scale ("Scale", float) = 1
        _Texture ("Before Texture", 2D) = "white" {}
        [Enum(SUB, 0, ADD, 1)] _Mode ("Mode", int) = 0
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
            #include "SimplexNoise.hlsl"
            
            int _Mode;
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
                float w = 0.25;
        
                for (int i = 0; i < _Fractal; i++)
                {
                    float2 coord = uv * s;
        
                    o += snoise(coord) * w;
        
                    s *= 2.0;
                    w *= 0.5;
                }
                
                float4 b = tex2D(_Texture, texUV);
                
                if(o < _Threshold)
                {
                    return float4(_Mode,_Mode,_Mode,1);
                }
                
                return b;
            }
        
            ENDCG
        }
    }
}
