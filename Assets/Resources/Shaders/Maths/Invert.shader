Shader "Noise/Invert"
{
    Properties
    {
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
            
            sampler2D _Texture;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = i.uv;              
                float4 b = tex2D(_Texture, uv);
                return 1-b;
            }
        
            ENDCG
        }
    }
}
