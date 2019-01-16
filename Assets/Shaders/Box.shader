Shader "Noise/Box"
{
    Properties
    {
        _X ("X", float) = 1
        _Y ("Y", float) = 1
        _TRX ("Top Right X", float) = 1
        _TRY ("Top Right Y", float) = 1
        _BLX ("Bottom Left X", float) = 0
        _BLY ("Bottom Left Y", float) = 0
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
            
            int _Mode;
            float _X;
            float _Y;
            float _TRX;
            float _TRY;
            float _BLX;
            float _BLY;
            float _Threshold;
            sampler2D _Texture;
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 texUV = i.uv;
                float2 uv = i.uv + float2(_X, _Y);
                
                float2 bl = step(float2(_BLX, _BLY), uv);
                float2 tr = step(float2(1-_TRX, 1-_TRY), 1-uv);
                float o = bl.x * bl.y * tr.x * tr.y;
                
                float4 b = tex2D(_Texture, texUV);
                
                if(o > 0)
                {
                    return float4(_Mode,_Mode,_Mode,1);
                }
                
                return b;
            }
        
            ENDCG
        }
    }
}
