Shader "Noise/Step"
{
    Properties
    {
        _Threshold ("Threshold", range(0, 1)) = 0
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
            
            float _Threshold;
            sampler2D _Texture;
            
            fixed4 frag (v2f_img i) : SV_Target
            {   
                float4 o = tex2D(_Texture, i.uv);
                
                if(o.r > _Threshold)
                    return float4(1,1,1,1);
                else
                    return float4(0,0,0,1);
            }
        
            ENDCG
        }
    }
}
