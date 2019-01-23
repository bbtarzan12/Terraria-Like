Shader "Noise/RidgedMulti"
{
    Properties
    {
        _X ("X Seed", float) = 1
        _Y ("Y Seed", float) = 1
        _RidgedOffset("RidgedOffset", float) = 0.5
        _Scale ("Scale", float) = 1
        _Gain("Grain", float) = 1
        _Lacunarity ("Lacunarity", float) = 1
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
            #include "RidgedMulti.hlsl"
            
            float _X;
            float _Y;
            float _Scale;
            float _RidgedOffset;
            float _Lacunarity;
            float _Gain;
            float _Ratio;
            int _Fractal;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = (i.uv - 0.5) * _Scale + float2(_X, _Y);
                float o = RidgedMF(float2(uv.x * _Ratio, uv.y), _RidgedOffset, _Fractal, _Lacunarity, _Gain);
                return float4(o,o,o,1);
            }
        
            ENDCG
        }
    }
}
