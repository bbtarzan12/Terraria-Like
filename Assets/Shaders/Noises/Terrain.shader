Shader "Noise/Terrain"
{
    Properties
    {
        _X ("X Seed", float) = 1
        _Height ("Height", range(0,1)) = 0.5
        _Scale ("Scale", float) = 1
        _Fractal ("Fractal", int) = 1
        _Ratio ("Ratio", float) = 1
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
            
            float _X;
            float _Height;
            float _Scale;
            float _Ratio;
            int _Fractal;

            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 texUV = i.uv;
                float o = 0.5;
                float s = 1;       
                float w = 0.5;
                float2 uv = float2(((i.uv-0.5) + float2(_X, 0)) * _Scale);

                for (int i = 0; i < _Fractal; i++)
                {
                    float2 coord = float2(uv.x * _Ratio, 0) * s;

                    o += snoise(coord) * w;
        
                    s *= 2.0;
                    w *= 0.5;
                }
                
                float t = step(uv.y + o, (_Height-0.5) * _Scale);
                
                if(t > 0)
                {
                    return float4(1,1,1,1);
                }
                
                
                return float4(0,0,0,1);
            }
        
            ENDCG
        }
    }
}
