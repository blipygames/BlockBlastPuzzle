Shader "Custom/SpriteHole"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Radius ("Hole Radius", Float) = 0.3
        _EdgeSharpness ("Edge Sharpness", Float) = 200
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _HoleCenter;
            float _Radius;
            float _EdgeSharpness;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                float2 center = float2(0.5, 0.5); 
                float dist = distance(IN.texcoord, center);
                float circle = saturate((dist - _Radius) * _EdgeSharpness);
                
                c.a *= circle;
                
                return c;
            }
            ENDCG
        }
    }
}