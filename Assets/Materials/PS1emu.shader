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
        _DEBUG_MODE("Norm, Page, UVs, cMod, CLUTx, CLUTy", Int) = 0
        _DEBUG_Tinyoffset ("tiny offset", float) = 0

        //hide
        _Page0_EID("p0", Int) = 0
        _Page1_EID("p1", Int) = 0
        _Page2_EID("p2", Int) = 0
        _Page3_EID("p3", Int) = 0
        _Page4_EID("p4", Int) = 0
        _Page5_EID("p5", Int) = 0
        _Page6_EID("p6", Int) = 0
        _Page7_EID("p7", Int) = 0
        _Page8_EID("p8", Int) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Cutout" }

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
            int _DEBUG_MODE;
            int _DEBUG_Tinyoffset;

            int _Page0_EID;
            int _Page1_EID;
            int _Page2_EID; 
            int _Page3_EID;
            int _Page4_EID;
            int _Page5_EID;
            int _Page6_EID;
            int _Page7_EID;
            int _Page8_EID;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float4 meta  : TEXCOORD1;
                float4 meta1 : TEXCOORD2;

                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                noperspective float2 uv : TEXCOORD0;
                nointerpolation float4 meta : TEXCOORD1;
                nointerpolation float4 meta1 : TEXCOORD2;
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

            int ResolvePage(int eid)
            {
                if (eid == _Page0_EID) return 0;
                if (eid == _Page1_EID) return 1;
                if (eid == _Page2_EID) return 2;
                if (eid == _Page3_EID) return 3;
                if (eid == _Page4_EID) return 4;
                if (eid == _Page5_EID) return 5;
                if (eid == _Page6_EID) return 6;
                if (eid == _Page7_EID) return 7;
                if (eid == _Page8_EID) return 8;

                return 0; 
            }

            uint ReadByte(int x, int y, int page)
            {
                float atlasX = x + 0.5;
                float atlasY = y + (page + _DEBUG_pageOffset) * _PageHeight + 0.5;

                float2 uv;
                uv.x = atlasX / _PageWidth;
                uv.y = atlasY / (_PageHeight * _PageCount);

                float v = tex2Dlod(_WGEO_Atlas, float4(uv, 0, 0)).r;
                return (uint)(v * 255.0);
            }

            uint ReadIndex(float2 uv, int colourMode, int page)
            {
                int2 p = int2((uv));
                int px = p.x;
                int py = p.y;

                if (colourMode == 0)
                {
                    int byteX = px >> 1;
                    uint b = ReadByte(byteX, py, page);

                    return (px & 1)
                        ? ((b >> 4) & 0x0F)
                        : (b & 0x0F);
                }

                if (colourMode == 1)
                {
                    return ReadByte(px, py, page);
                }

                int byteX = px << 1;
                uint lo = ReadByte(byteX, py, page);
                uint hi = ReadByte(byteX + 1, py, page);

                return lo | (hi << 8);
            }

uint ReadCLUT(uint index, int clutX, int clutY, int page, int colourMode)
{
    int baseY = clutY + _DEBUG_CLUTyOffset;

    int byteX;

    if (colourMode == 1) // 8bpp
    {
        // FULL palette index, no CLUTX windowing
        byteX = ((int)index) << 1;
    }
    else // 4bpp
    {
        int baseX = clutX + _DEBUG_CLUTxOffset;
        byteX = (baseX + ((int)index)) << 1;
    }

    uint lo = ReadByte(byteX, baseY, page);
    uint hi = ReadByte(byteX + 1, baseY, page);

    return lo | (hi << 8);
}

            float4 Decode5551(uint c)
            {
                float r = ((c >> 0)  & 31) / 31.0;
                float g = ((c >> 5)  & 31) / 31.0;
                float b = ((c >> 10) & 31) / 31.0;
                float a = ((c >> 15) & 1) ? 1.0 : 0.0;

                return float4(r, g, b, a);
            }

            float4 DEBUG_ColourMode(int mode)
            {
                switch(mode)
                {
                    case 0: return fixed4(1, 0, 0, 1);
                    case 1: return fixed4(0, 1, 0, 1);
                    case 2: return fixed4(0, 0, 1, 1);
                    default: return fixed4(1, 0, 1, 1);
                }
            }

            float4 DEBUG_CLUTMode(int mode)
            {
                switch(mode)
                {
                    case 0: return fixed4(1, 0, 0, 1);
                    case 1: return fixed4(0, 1, 0, 1);
                    case 2: return fixed4(0, 0, 1, 1);
                    case 3: return fixed4(1, 0, 1, 1);
                    case 4: return fixed4(1, 1, 0, 1);
                    case 5: return fixed4(0, 1, 1, 1);
                    case 6: return fixed4(0, 0, 0, 1);
                    default: return fixed4(1, 1, 1, 1);
                }
            }

            fixed4 frag(v2f i) : SV_Target
            {
                int colourMode = (int)i.meta1.x;
                int eid = (int)i.meta.z;

                int page = ResolvePage(eid);

                uint value = ReadIndex(i.uv, colourMode, page);

                uint colour;

                if (colourMode < 2)
                {
                    colour = ReadCLUT(
                        value,
                        (int)i.meta.x,
                        (int)i.meta.y,
                        page,
                        (int)i.meta1.x
                    );
                }
                else
                {
                    colour = value;
                }

                //float4 finalCol = Decode5551(colour);
                float4 finalCol = Decode5551(colour);
                finalCol.rgb *= i.col.rgb;

                switch (_DEBUG_MODE)
                {
                    case 0: return finalCol;
                    case 1: return fixed4(ResolvePage(eid) / 8,0,0,1);
                    case 2: return fixed4(i.uv.x / _PageWidth, i.uv.y / _PageHeight, 0, 1);
                    case 3: return DEBUG_ColourMode(i.meta1.x);
                    case 4: return DEBUG_CLUTMode(i.meta.x);
                    case 5: return DEBUG_CLUTMode(i.meta.y);
                }

                return fixed4(1, 0, 1, 1);
            }

            ENDCG
        }
    }
}