Shader "Noise/Circle"
{
    Properties
    {
        _X ("X", float) = 1
        _Y ("Y", float) = 1
        _Scale ("Scale", float) = 1
        _Threshold ("Threshold", float) = 1
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
            #include "PerlinNoise.hlsl"
            
            int _Mode;
            float _X;
            float _Y;
            float _Scale;
            float _Threshold;
            sampler2D _Texture;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 texUV = i.uv;
                float2 uv = i.uv + float2(_X, _Y);
                float o = distance(uv, float2(0.5, 0.5)) / _Scale;
                
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
