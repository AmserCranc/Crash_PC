Shader "Custom/WGEO_Debug"
{
    Properties
    {
        _DebugMode ("Debug Mode", Int) = 0
        _PageSize ("Page Size", Float) = 512
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // -----------------------------
            // VRAM pages (max 8)
            // -----------------------------
            StructuredBuffer<uint> _TexData_0;
            StructuredBuffer<uint> _TexData_1;
            StructuredBuffer<uint> _TexData_2;
            StructuredBuffer<uint> _TexData_3;
            StructuredBuffer<uint> _TexData_4;
            StructuredBuffer<uint> _TexData_5;
            StructuredBuffer<uint> _TexData_6;
            StructuredBuffer<uint> _TexData_7;

            int _DebugMode;
            float _PageSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 meta   : TEXCOORD1;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
                float4 meta : TEXCOORD1;
                fixed4 col  : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.meta = v.meta;
                o.col = v.color;
                return o;
            }

            uint Fetch(uint page, uint index)
            {
                if (page == 0) return _TexData_0[index];
                if (page == 1) return _TexData_1[index];
                if (page == 2) return _TexData_2[index];
                if (page == 3) return _TexData_3[index];
                if (page == 4) return _TexData_4[index];
                if (page == 5) return _TexData_5[index];
                if (page == 6) return _TexData_6[index];
                return _TexData_7[index];
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // -----------------------------
                // MODE 0: solid white
                // -----------------------------
                if (_DebugMode == 0)
                    return fixed4(1,1,1,1);

                // -----------------------------
                // MODE 1: UV.x visualization
                // -----------------------------
                if (_DebugMode == 1)
                {
                    float v = frac(i.uv.x / _PageSize);
                    return fixed4(v, v, v, 1);
                }

                // -----------------------------
                // MODE 2: UV.y visualization
                // -----------------------------
                if (_DebugMode == 2)
                {
                    float v = frac(i.uv.y / _PageSize);
                    return fixed4(v, v, v, 1);
                }

                // -----------------------------
                // MODE 3: UV checker (optional sanity check)
                // -----------------------------
                if (_DebugMode == 3)
                {
                    float2 v = frac(i.uv / _PageSize);
                    return fixed4(v.x, v.y, 0, 1);
                }

                // -----------------------------
                // MODE 4: raw VRAM lookup
                // -----------------------------
                uint page = (uint)i.meta.z;

                uint2 uv = (uint2)round(i.uv);
                uint index = uv.y * (uint)_PageSize + uv.x;

                uint word = Fetch(page, index >> 2);
                uint shift = (index & 3) * 8;
                uint pixel = (word >> shift) & 0xFF;

                float v = pixel / 255.0;
                return fixed4(v, v, v, 1);
            }

            ENDCG
        }
    }
}