Shader "Noise/Flip"
{
    Properties
    {
        _X ("X (0 = X, 1 = Flip)", int) = 0
        _Y ("Y (0 = X, 1 = Flip)", int) = 0
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
            
            int _X;
            int _Y;
            sampler2D _Texture;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = i.uv;          
                float2 newUV = float2(abs(uv.x - _X), abs(uv.y - _Y));    
                float4 b = tex2D(_Texture, newUV);
                return b;
            }
        
            ENDCG
        }
    }
}
