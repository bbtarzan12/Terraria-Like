Shader "Noise/Gradient"
{
    Properties
    {
        _Pos ("Position (Start.xy, End.xy)", Vector) = (0,0,1,1)
        [Enum(Angled, 0, Radial, 1)] _Mode ("Mode", int) = 0
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
            float4 _Pos;
            
            float Angled(float2 start, float2 end, float2 uv)
            {
                  float2 od = end - start;
                  float odSq = dot(od, od);
                  float2 op = uv - start;
                  float opXod = dot(op, od);
                  
                  return saturate(opXod / odSq);
            }
            
            float Radial(float2 start, float2 end, float2 uv)
            {
                float2 od = end - start;
                float odSq = dot(od, od);
                float2 op = uv - start;
                float opSq = dot(op,op);
                
                return saturate(odSq / opSq );
            }
            
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float proj = _Mode == 0 ? Angled(_Pos.xy, _Pos.zw, uv) : Radial(_Pos.xy, _Pos.zw, uv);     
                return proj;
            }
        
            ENDCG
        }
    }
}
