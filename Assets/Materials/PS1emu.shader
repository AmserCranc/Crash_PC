Shader "Custom/WGEO_PS1"
{
    Properties
    {
        _WGEO_Atlas("Texture Atlas", 2D) = "white" {}

        _PageCount("Page Count", Float) = 1
        _PageWidth("Page Width", Float) = 512
        _PageHeight("Page Height", Float) = 128

        _DEBUG_pageOffset("Debug Page Offset", Int) = 0
        _DEBUG_CLUTxOffset("Debug CLUT x Offset", Int) = 0
        _DEBUG_CLUTyOffset("Debug CLUT y Offset", Int) = 0

    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Cull Off
            ZWrite On

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _WGEO_Atlas;

            float _PageCount;
            float _PageWidth;
            float _PageHeight;

            int _DEBUG_pageOffset;
            int _DEBUG_CLUTxOffset;
            int _DEBUG_CLUTyOffset;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;

                // x = clutX (VRAM x)
                // y = clutY (VRAM y)
                // z = page
                // w = blend
                float4 meta : TEXCOORD1;

                // x = colourMode
                float4 meta1 : TEXCOORD2;

                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float4 meta : TEXCOORD1;
                float4 meta1 : TEXCOORD2;
                fixed4 col : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.meta = v.meta;
                o.meta1 = v.meta1;
                o.col = v.color;
                return o;
            }


            uint ReadByte(int x, int y, int page)
            {
                x = clamp(x, 0, 511);
                y = clamp(y, 0, 127);

                float2 uv;

                uv.x = (x + 0.5) / _PageWidth;

                uv.y =
                    ((y + 0.5) / _PageHeight) / _PageCount
                    + ((float)(page + _DEBUG_pageOffset) / _PageCount);

                return (uint)(tex2D(_WGEO_Atlas, uv).r * 255.0 + 0.5);
            }

            uint ReadIndex(float2 uv, int colourMode, int page) //UVs in are pagespace 512/128
            {
                colourMode = 1;
                int px;
                int py = uv.y;

                // -----------------------------
                // 4BPP
                if (colourMode == 0)
                {
                    px = uv.x;

                    int byteX = px >> 1;
                    uint b = ReadByte(byteX, py, page);

                    return (px & 1)
                        ? ((b >> 4) & 0x0F)
                        : (b & 0x0F);
                }

                // -----------------------------
                // 8BPP
                if (colourMode == 1)
                {
                    px = uv.x;
                    return ReadByte(px, py, page);
                }

                // -----------------------------
                // 16BPP DIRECT
                px = uv.x;

                int byteX = px * 2;

                uint lo = ReadByte(byteX + 0, py, page);
                uint hi = ReadByte(byteX + 1, py, page);

                return lo | (hi << 8);
            }

            uint ReadCLUT(
                uint index,
                int clutX,
                int clutY,
                int page
            )
            {
                int entryX = clutX + _DEBUG_CLUTxOffset + (int)index;

                int byteX = entryX * 2;
                int byteY = clutY + _DEBUG_CLUTyOffset;

                uint lo = ReadByte(byteX + 0, byteY, page);
                uint hi = ReadByte(byteX + 1, byteY, page);

                return lo | (hi << 8);
            }

            float4 Decode5551(uint c)
            {
                
                float a = ((c >> 15) & 31);
                float r = ((c >> 10) & 31) / 31.0;
                float g = ((c >> 5)  & 31) / 31.0;
                float b = ((c >> 0)  & 31) / 31.0;


                return float4(r, g, b, a);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                int colourMode = (int)i.meta1.x;

                int page = (int)i.meta.z;

                uint value = ReadIndex(i.uv, colourMode, page);

                uint colour;

                if (colourMode < 2)
                {
                    colour = ReadCLUT(
                        value,
                        (int)i.meta.x,
                        (int)i.meta.y,
                        page
                    );
                }
                else
                {
                    colour = value;
                }

                float4 finalCol = Decode5551(colour);

                return finalCol * i.col;
                //return fixed4(i.meta.x, i.meta.y, 0, 1);
            }

            // fixed4 frag(v2f i) : SV_Target 
            // { 
            //     float pageOffset = (i.meta.z + _DEBUG_pageOffset) / _PageCount; 
            //     float2 uvs = float2(i.uv.x, (i.uv.y / _PageCount) + pageOffset); 
            //     fixed4 colour = tex2D(_WGEO_Atlas, uvs); 
            //     return tex2D(_WGEO_Atlas, uvs); 
            // }

            ENDCG
        }
    }
}