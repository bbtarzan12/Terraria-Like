Shader "Noise/Perlin"
{
    Properties
    {
        _X ("X Seed", float) = 1
        _Y ("Y Seed", float) = 1
        _Scale ("Scale", float) = 1
        _Fractal ("Fractal", int) = 1
        _Threshold ("Threshold", range(0, 1)) = 0
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
            #include "PerlinNoise.hlsl"
            
            int _Mode;
            float _X;
            float _Y;
            float _Scale;
            float _Threshold;
            float4 _Color;
            int _Fractal;
            sampler2D _Texture;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float o = 0.5;
                float s = _Scale;       
                float w = 0.5;
        
                for (int i = 0; i < _Fractal; i++)
                {
                    float2 coord = (uv - 0.5) * s + float2(_X, _Y);

                    o += cnoise(coord) * w;
        
                    s *= 2.0;
                    w *= 0.5;
                }   
              
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
                    return float4(maxB, maxB, maxB, 1);
                    
                return float4(0,0,0,1);
            }
        
            ENDCG
        }
    }
}
