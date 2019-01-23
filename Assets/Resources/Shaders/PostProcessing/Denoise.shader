Shader "Noise/Denoise"
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
            uniform float4 _Texture_TexelSize;
            
            
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = i.uv;              
                float3 center = tex2D(_Texture, uv).rgb;
                
                if(length(center) == 0)
                    return float4(0,0,0,1);
                
				float3 up = tex2D(_Texture, uv + half2(0, _Texture_TexelSize.y)).rgb;
				float3 down = tex2D(_Texture, uv - half2(0, _Texture_TexelSize.y)).rgb;
				float3 right = tex2D(_Texture, uv + half2(_Texture_TexelSize.x, 0)).rgb;
				float3 left = tex2D(_Texture, uv - half2(_Texture_TexelSize.x, 0)).rgb;
                
                if((length(up) + length(down)) == 0)
                    return float4(0,0,0,1);
                
                if((length(left) + length(right)) == 0)
                    return float4(0,0,0,1);
				
                return tex2D(_Texture, uv);
            }
        
            ENDCG
        }
    }
}
