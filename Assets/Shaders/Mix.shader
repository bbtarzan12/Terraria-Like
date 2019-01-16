Shader "Noise/Mix"
{
    Properties 
    {
        _Texture1 ("Texture 1", 2D) = "" 
        _Texture2 ("Texture 2", 2D) = ""
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

            int _Mode;
            sampler2D _Texture1;
            sampler2D _Texture2;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                
                float4 tex1Color = tex2D(_Texture1, uv);
                float4 tex2Color = tex2D(_Texture2, uv);
                
                switch(_Mode)
                {
                    case 0: return float4((tex1Color + tex2Color).rgb,1);
                    case 1: return float4((tex1Color - tex2Color).rgb,1);
                    case 2: return float4((tex1Color * tex2Color).rgb,1);
                    case 3: return float4((tex1Color / tex2Color).rgb,1);
                    default: return float4(0,0,0,1);
                }
            }
        
            ENDCG
        }
    }
}