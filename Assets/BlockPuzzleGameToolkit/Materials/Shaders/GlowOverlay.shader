Shader "UI/GlowAura"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowSpeed ("Glow Speed", Float) = 1.0
        _GlowIntensity ("Glow Intensity", Range(0, 1)) = 0.5
        _GlowWidth ("Glow Width", Range(0, 0.1)) = 0.05
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            fixed4 _GlowColor;
            float _GlowSpeed;
            float _GlowIntensity;
            float _GlowWidth;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                // Calculate distance from edge
                float2 uvDist = abs(IN.texcoord - 0.5) * 2;
                float maxDist = max(uvDist.x, uvDist.y);
                float edge = saturate((1 - maxDist) / _GlowWidth);
                
                // Calculate glow
                float glowFactor = sin(_Time.y * _GlowSpeed) * 0.5 + 0.5;
                fixed4 glowColor = _GlowColor * glowFactor * _GlowIntensity * edge;
                
                // Blend glow with original color
                color.rgb = lerp(glowColor.rgb, color.rgb, color.a);
                color.a = max(color.a, glowColor.a);
                
                return color;
            }
            ENDCG
        }
    }
}