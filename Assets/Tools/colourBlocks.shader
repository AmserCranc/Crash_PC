Shader "Custom/ColorBlocks"
{
    Properties
    {
        _ColourCount ("Colour Count", Int) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_COLOURS 256

            float4 _Colours[MAX_COLOURS];
            int _ColourCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float x = saturate(i.uv.x);

                float scaled = x * _ColourCount;
                int index = (int)scaled;
                index = clamp(index, 0, _ColourCount - 1);

                float localX = frac(scaled);
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float pixelWidth = 1.0 / _ScreenParams.x;
                float localPixelWidth = pixelWidth * _ColourCount;

                if (localX > 1 - localPixelWidth)
                {
                    return float4(0, 0, 0, 1);
                }

                return _Colours[index];
            }

            ENDCG
        }
    }

    FallBack "Unlit/Color"
}