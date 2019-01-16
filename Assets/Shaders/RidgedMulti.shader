Shader "Noise/RidgedMulti"
{
    Properties
    {
        _X ("X Seed", float) = 1
        _Y ("Y Seed", float) = 1
        _Scale ("Scale", float) = 1
        _Gain("Grain", float) = 1
        _Lacunarity ("Lacunarity", float) = 1
        _Threshold ("Threshold", range(0,1)) = 0
        _Fractal ("Fractal", int) = 1
        _Texture ("Before Texture", 2D) = "white" {}
        _Color ("Color", color) = (1,1,1,1)
        [Enum(ADD, 0, SUB, 1, MUL, 2, DIV, 3)] _Mode ("Mode", int) = 0
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
            
            int _Mode;
            float _X;
            float _Y;
            float _Scale;
            float _Threshold;
            float _Lacunarity;
            float _Gain;
            int _Fractal;
            float4 _Color;
            sampler2D _Texture;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float2 coord = (uv - 0.5) * _Scale + float2(_X, _Y);
                float o = RidgedMF(coord, _Threshold * 0.5, _Fractal, _Lacunarity, _Gain);
                //o = (tan((uv.x + o * 100.0) * 2.0 * 3.141592 / 200.0) + 1.0) / 2.0; 
                
                float4 b = tex2D(_Texture, uv);
                
                if(o < _Threshold)
                {
                    switch(_Mode)
                    {
                        case 0: return float4((_Color + b).rgb,1);
                        case 1: return float4((_Color - b).rgb,1);
                        case 2: return float4((_Color * b).rgb,1);
                        case 3: return float4((_Color / b).rgb,1);
                        default: return float4(0,0,0,1);
                    }
                }
                
                float maxB = max(b.x,max(b.y,b.z));
                
                if(maxB > _Threshold)
                   return float4(maxB,maxB,maxB,1);
                    
                return float4(0,0,0,1);
            }
        
            ENDCG
        }
    }
}
