Shader "Noise/Terrain"
{
    Properties
    {
        _SX ("Simplex X Seed", float) = 1
        _SY ("Simplex Y Seed", float) = 1
        _PX ("Perlin X Seed", float) = 1
        _PY ("Perlin Y Seed", float) = 1
        _SScale ("Simplex Scale", float) = 1
        _SFractal ("Simplex Fractal", int) = 1
        _PScale ("Perlin Scale", float) = 1
        _PFractal ("Perlin Fractal", int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata
            {    
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            v2f vert (appdata v)
            {
                v2f o;               
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f o) : SV_Target
            {
                float2 uv = o.uv;

                if(uv.y > 0.7 || (uv.y < 0.2 && uv.y > 0.05))
                    return float4(0,0,0,1);
                
                if(uv.y >= -abs(1.5*uv.x-0.75)+1.35)
                    return float4(0,0,0,1);
                
                return float4(1,1,1,1);
            }
            
            ENDCG
        }
        
        Pass
        {
            Blend DstColor Zero 
            CGPROGRAM
        
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "SimplexNoise.hlsl"
            
            float _SX;
            float _SY;
            float _SScale;
            int _SFractal;
            
            struct appdata
            {    
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            v2f vert (appdata v)
            {
                v2f o;               
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f o) : SV_Target
            {
                float2 uv = o.uv + float2(_SX, _SY);
                float r = 0.5;
                float s = _SScale;       
                float w = 0.25;
        
                for (int i = 0; i < _SFractal; i++)
                {
                    float2 coord = uv * s;
        
                    r += snoise(coord) * w;
        
                    s *= 2.0;
                    w *= 0.5;
                }
                
                return float4(r, r, r, 1);
            }
        
            ENDCG
        }
        
        Pass
        {
            Blend DstColor Zero 
            CGPROGRAM
        
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "PerlinNoise.hlsl"
            
            float _PX;
            float _PY;
            float _PScale;
            int _PFractal;
            
            struct appdata
            {    
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            v2f vert (appdata v)
            {
                v2f o;               
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f o) : SV_Target
            {
                float2 uv = o.uv + float2(_PX, _PY);
                float r = 0.5;
                float s = _PScale;       
                float w = 0.5;
        
                for (int i = 0; i < _PFractal; i++)
                {
                    float2 coord = uv * s;

                    r += cnoise(coord) * w;
        
                    s *= 2.0;
                    w *= 0.5;
                }
            
            return float4(r, r, r, 1);
            }
        
            ENDCG
        }
        
        pass
        {
            Blend DstColor Zero 
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata
            {    
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            v2f vert (appdata v)
            {
                v2f o;               
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f o) : SV_Target
            {
                float r = step(0.2, o.color.r);
                
                return float4(r,r,r,1);
            }
            
            ENDCG
        }
    }
}