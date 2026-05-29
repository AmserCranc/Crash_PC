Shader "Custom/WGEO_PS1_StencilShadow"
{
    Properties
    {
        _WGEO_Atlas("Texture Atlas", 2D) = "white" {}

        _PageCount("Page Count", Float) = 1
        _PageWidth("Page Width", Float) = 512
        _PageHeight("Page Height", Float) = 128

        _VertColourIntensity("Vert colour intensity", Range(0,2)) = 1

        _ShadowDarkness("Shadow Darkness", Range(0,1)) = 0.35

        _DEBUG_pageOffset("Debug Page Offset", Int) = 0
        _DEBUG_CLUTxOffset("Debug CLUT x Offset", Int) = 0
        _DEBUG_CLUTyOffset("Debug CLUT y Offset", Int) = 0
        _DEBUG_MODE("Debug Mode", Int) = 0

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
        Tags
        {
            "RenderType"="Cutout"
            "Queue"="AlphaTest"
        }

        // ======================================================
        // MAIN PASS
        // ======================================================

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }

            Cull Back
            ZWrite On

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _WGEO_Atlas;

            float _PageCount;
            float _PageWidth;
            float _PageHeight;

            float _VertColourIntensity;
            float _ShadowDarkness;

            int _DEBUG_pageOffset;
            int _DEBUG_CLUTxOffset;
            int _DEBUG_CLUTyOffset;
            int _DEBUG_MODE;

            int _Page0_EID;
            int _Page1_EID;
            int _Page2_EID;
            int _Page3_EID;
            int _Page4_EID;
            int _Page5_EID;
            int _Page6_EID;
            int _Page7_EID;
            int _Page8_EID;

            static const float _8_5_lookup[32] =
            {
                0 ,9 ,17 ,25 ,33 ,42 ,50 ,58 ,
                66 ,75 ,83 ,91 ,99 ,107 ,116 ,124 ,
                132 ,140 ,149 ,157 ,165 ,173 ,181 ,190 ,
                198 ,206 ,214 ,223 ,231 ,239 ,247 ,255
            };

            struct appdata
            {
                float4 vertex : POSITION;

                float2 uv : TEXCOORD0;
                float4 meta : TEXCOORD1;
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

                SHADOW_COORDS(3)
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;
                o.meta = v.meta;
                o.meta1 = v.meta1;

                o.col = v.color;

                TRANSFER_SHADOW(o);

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

                float atlasY =
                    y +
                    (page + _DEBUG_pageOffset) * _PageHeight +
                    0.5;

                float2 uv;

                uv.x = atlasX / _PageWidth;
                uv.y = atlasY / (_PageHeight * _PageCount);

                float v = tex2Dlod(
                    _WGEO_Atlas,
                    float4(uv, 0, 0)
                ).r;

                return (uint)(v * 255.0);
            }

            uint ReadIndex(float2 uv, int colourMode, int page)
            {
                int2 p = int2(uv);

                int px = p.x;
                int py = p.y;

                // 4-bit indexed
                if (colourMode == 0)
                {
                    bool isHighNibble = (p.x % 2) == 0;

                    uint pxByte = ReadByte(
                        p.x / 2,
                        p.y,
                        page
                    );

                    return !isHighNibble
                        ? (pxByte >> 4) & 0x0F
                        : pxByte & 0x0F;
                }

                // 8-bit indexed
                if (colourMode == 1)
                {
                    return ReadByte(px, py, page);
                }

                // 16-bit direct
                int byteX = px << 1;

                uint lo = ReadByte(byteX, py, page);
                uint hi = ReadByte(byteX + 1, py, page);

                return lo | (hi << 8);
            }

            uint ReadCLUT(
                uint index,
                int clutX,
                int clutY,
                int page,
                int colourMode)
            {
                int baseY = clutY + _DEBUG_CLUTyOffset;

                int byteX = 0;

                if (colourMode == 1)
                {
                    byteX = ((int)index) << 1;
                }

                if (colourMode == 0)
                {
                    int subPaletteIndex = clutX * 16;

                    int paletteEntryIndex =
                        subPaletteIndex + index;

                    byteX = paletteEntryIndex * 2;
                }

                uint lo = ReadByte(byteX + 0, baseY, page);
                uint hi = ReadByte(byteX + 1, baseY, page);

                return (hi << 8) | lo;
            }

            float4 Decode5551(uint c, int blendMode)
            {
                float r = _8_5_lookup[(c >> 0) & 31] / 255.0;
                float g = _8_5_lookup[(c >> 5) & 31] / 255.0;
                float b = _8_5_lookup[(c >> 10) & 31] / 255.0;

                uint aBit = (c >> 15) & 1;

                float hasColor =
                    (r > 0.0 || g > 0.0 || b > 0.0)
                    ? 1.0
                    : 0.0;

                float a;

                switch (blendMode)
                {
                    case 0:
                        a = (aBit == 1)
                            ? (127.0 / 255.0)
                            : hasColor;
                        break;

                    default:
                        a = (aBit == 1 || hasColor > 0.0)
                            ? 1.0
                            : 0.0;
                        break;
                }

                return float4(r, g, b, a);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                int colourMode = (int)i.meta1.x;
                int eid = (int)i.meta.z;

                int page = ResolvePage(eid);

                uint value = ReadIndex(
                    i.uv,
                    colourMode,
                    page
                );

                uint colour;

                if (colourMode < 2)
                {
                    colour = ReadCLUT(
                        value,
                        (int)i.meta.x,
                        (int)i.meta.y,
                        page,
                        colourMode
                    );
                }
                else
                {
                    colour = value;
                }

                float4 finalCol =
                    Decode5551(colour, i.meta.w);

                clip(finalCol.a - 0.5);

                // ==========================================
                // STENCIL-LIKE SHADOW DARKENING
                // ==========================================

                fixed shadow = SHADOW_ATTENUATION(i);

                // Unity:
                // 1 = lit
                // 0 = shadowed

                float shadowed = 1.0 - shadow;

                // Hard edge
                shadowed =
                    shadowed > 0.001
                    ? 1.0
                    : 0.0;

                // Apply only shadow darkening
                finalCol.rgb *= lerp(
                    1.0,
                    _ShadowDarkness,
                    shadowed
                );

                // Vertex colour
                finalCol.rgb *=
                    i.col.rgb *
                    _VertColourIntensity;

                // ==========================================
                // DEBUG
                // ==========================================

                switch (_DEBUG_MODE)
                {
                    case 0:
                        return finalCol;

                    case 1:
                        return fixed4(
                            shadowed,
                            shadowed,
                            shadowed,
                            1
                        );

                    case 2:
                        return fixed4(
                            shadow,
                            shadow,
                            shadow,
                            1
                        );

                    default:
                        return fixed4(1,0,1,1);
                }
            }

            ENDCG
        }

        // ======================================================
        // SHADOW CASTER PASS
        // ======================================================

        Pass
        {
            Name "ShadowCaster"

            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            ZWrite On

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vertShadow
            #pragma fragment fragShadow

            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vertShadow(appdata_base v)
            {
                v2f o;

                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);

                return o;
            }

            float4 fragShadow(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}