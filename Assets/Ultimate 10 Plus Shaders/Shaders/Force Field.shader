Shader "Ultimate 10+ Shaders/Force Field Pulsating"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1,1,1,1)

        _FresnelPower("Fresnel Power", Range(0, 10)) = 3
        _ScrollDirection ("Scroll Direction", float) = (0, 0, 0, 0)
        
        // Параметры пульсации
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.0
        _PulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.3
        _PulseMinBrightness ("Min Brightness", Range(0, 1)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Cull Back
        Lighting Off
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #ifndef SHADER_API_D3D11
                #pragma target 3.0
            #else
                #pragma target 4.0
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float rim : TEXCOORD1;
                float4 position : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            half _FresnelPower;
            half2 _ScrollDirection;
            
            // Параметры пульсации
            float _PulseSpeed;
            float _PulseIntensity;
            float _PulseMinBrightness;

            v2f vert (appdata vert)
            {
                v2f output;

                output.position = UnityObjectToClipPos(vert.vertex);
                output.uv = TRANSFORM_TEX(vert.uv, _MainTex);

                float3 worldNormal = UnityObjectToWorldNormal(vert.normal);
                float3 worldPos = mul(unity_ObjectToWorld, vert.vertex).xyz;
                float3 viewDirWorld = normalize(_WorldSpaceCameraPos - worldPos);

                output.rim = 1.0 - saturate(dot(viewDirWorld, worldNormal));
                output.rim = saturate(output.rim + _PulseMinBrightness); // Гарантированная минимальная видимость

                output.uv += _ScrollDirection * _Time.y;

                return output;
            }

            fixed4 frag (v2f input) : SV_Target
            {
                // Эффект пульсации (синусоидальная волна)
                float pulse = sin(_Time.y * _PulseSpeed) * _PulseIntensity + 1.0;
                
                // Комбинируем текстуру, цвет и эффект Френеля
                fixed4 pixel = tex2D(_MainTex, input.uv) * _Color;
                
                // Применяем пульсацию к цвету и альфа-каналу
                pixel.rgb *= pulse;
                pixel.a *= input.rim * pow(_FresnelPower, input.rim);
                
                return pixel;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}