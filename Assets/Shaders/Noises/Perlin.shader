Shader "Noise/Perlin"
{
    Properties
    {
        _X ("X Seed", float) = 1
        _Y ("Y Seed", float) = 1
        _Scale ("Scale", float) = 1
        _Fractal ("Fractal", int) = 1
        _Threshold ("Threshold", range(0, 1)) = 0
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
            #include "PerlinNoise.hlsl"
            
            float _X;
            float _Y;
            float _Scale;
            float _Ratio;
            int _Fractal;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = (i.uv - 0.5) * _Scale + float2(_X, _Y);
                float o = 0.5;
                float s = 1;       
                float w = 0.5;
        
                for (int i = 0; i < _Fractal; i++)
                {
                    float2 coord = uv * s;
        
                    o += cnoise(float2(coord.x * _Ratio, coord.y)) * w;
        
                    s *= 2.0;
                    w *= 0.5;
                }   
               
                    
                return float4(o,o,o,1);
            }
        
            ENDCG
        }
    }
}
