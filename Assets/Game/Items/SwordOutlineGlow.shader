Shader "Custom/UIRarityGlow"
{
    Properties
    {
        _MainTex        ("Sprite Texture", 2D) = "white" {}
        _OutlineColor   ("Outline Color",  Color) = (0,1,0,1)
        _OutlineWidth   ("Outline Width",  Float) = 1.5
        _OutlineAlpha   ("Outline Alpha",  Range(0,1)) = 0.0
        
        // Unity UI requires these — do NOT remove them
        _Color          ("Tint", Color) = (1,1,1,1)
        _StencilComp    ("Stencil Comparison", Float) = 8
        _Stencil        ("Stencil ID", Float) = 0
        _StencilOp      ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask  ("Stencil Read Mask",  Float) = 255
        _ColorMask      ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "IgnoreProjector"   = "True"
            "RenderType"        = "Transparent"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref        [_Stencil]
            Comp       [_StencilComp]
            Pass       [_StencilOp]
            ReadMask   [_StencilReadMask]
            WriteMask  [_StencilWriteMask]
        }

        Cull     Off
        Lighting Off
        ZWrite   Off
        ZTest    [unity_GUIZTestMode]
        Blend    SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;
            float4    _MainTex_TexelSize;
            float4    _OutlineColor;
            float     _OutlineWidth;
            float     _OutlineAlpha;
            fixed4    _Color;
            fixed4    _TextureSampleAdd;
            float4    _ClipRect;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                float4 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.worldPos = v.vertex;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.color    = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Respect Unity UI clipping (scroll views, masks, etc.)
                if (!UnityGet2DClipping(i.worldPos.xy, _ClipRect)) discard;

                fixed4 col = (tex2D(_MainTex, i.uv) + _TextureSampleAdd) * i.color;

                // Only run outline logic if there's actually a glow to show
                if (_OutlineAlpha > 0.001)
                {
                    float2 px = _MainTex_TexelSize.xy * _OutlineWidth;

                    // Sample 8 neighbours
                    float maxN = 0;
                    maxN = max(maxN, tex2D(_MainTex, i.uv + float2(    0,  px.y)).a);
                    maxN = max(maxN, tex2D(_MainTex, i.uv + float2(    0, -px.y)).a);
                    maxN = max(maxN, tex2D(_MainTex, i.uv + float2( px.x,     0)).a);
                    maxN = max(maxN, tex2D(_MainTex, i.uv + float2(-px.x,     0)).a);
                    maxN = max(maxN, tex2D(_MainTex, i.uv + float2( px.x,  px.y)).a);
                    maxN = max(maxN, tex2D(_MainTex, i.uv + float2(-px.x,  px.y)).a);
                    maxN = max(maxN, tex2D(_MainTex, i.uv + float2( px.x, -px.y)).a);
                    maxN = max(maxN, tex2D(_MainTex, i.uv + float2(-px.x, -px.y)).a);

                    // Outline = transparent pixel with an opaque neighbour
                    float outline = (1.0 - col.a) * maxN;

                    fixed4 glowPixel = fixed4(_OutlineColor.rgb, outline * _OutlineAlpha);
                    col = lerp(col, glowPixel, outline);
                }

                return col;
            }
            ENDCG
        }
    }
}