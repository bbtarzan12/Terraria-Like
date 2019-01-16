Shader "Noise/BasicTerrain"
{
    Properties
    {
        _Height ("Height", range(0,1)) = 0.5
        _Scale ("Scale", float) = 1
        _Fractal ("Fractal", int) = 1
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
            
            float _Height;
            int _Mode;
            float _Scale;
            float _Threshold;
            int _Fractal;
            sampler2D _Texture;

            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 texUV = i.uv;
                float o = 0.5;
                float s = 1;       
                float w = 0.5;
                float2 uv = float2((i.uv-0.5) * _Scale);

                for (int i = 0; i < _Fractal; i++)
                {
                    float2 coord = float2(uv.x, 0) * s;

                    o += snoise(coord) * w;
        
                    s *= 2.0;
                    w *= 0.5;
                }
                
                float t = step(uv.y + o, (_Height-0.5) * _Scale);
                
                float4 b = tex2D(_Texture, texUV);
                
                if(t > 0)
                {
                    return float4(_Mode,_Mode,_Mode,1);
                }
                
                
                return 1-b;
            }
        
            ENDCG
        }
    }
}
