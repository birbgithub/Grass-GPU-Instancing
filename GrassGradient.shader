Shader "Unlit/GrassGradient"
{
    Properties
    {
        _Colour1("Colour 1", Color) = (1, 1, 1, 1)
        _Colour2("Colour 2", Color) = (0, 0, 0, 1)
        _Length("Length", Float) = 1
        _WindSpeed("Wind Speed", Float) = 1
        _Amplitude("Wind Swirliness", Float) = 1
        _Seperation("Wind Seperation", Float) = 1
        _WindStrength("Wind Strength", Float) = 1
    }
    SubShader
    {
        LOD 100
        Zwrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertexData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 colour : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            struct GrassData
            {
                float4 position;
                float2 worldUV;
            };

            fixed4 _Colour1;
            fixed4 _Colour2;
            float _Length;
            float _WindSpeed;
            float _Amplitude;
            float _Seperation;
            float _WindStrength;
            StructuredBuffer<float4> GrassDataBuffer;

            v2f vert (VertexData v, uint id : SV_INSTANCEID)
            {
                v2f o;

                float4 grassPosition = GrassDataBuffer[id];


                float3 localPosition = v.vertex; // mesh vertex positions
                localPosition.y *= v.uv.y * grassPosition.w * _Length; // scale y to vary height


                float noise = (grassPosition.w - 0.6) * _Amplitude;
                float c = 1.0 - sin(((grassPosition.x + grassPosition.z + noise) + _Time.y * _WindSpeed) * _Seperation);
                
                localPosition.xz += (c, c) * v.uv.y * _WindStrength;
                
                float3 worldPosition = grassPosition.xyz + localPosition;
                o.vertex = UnityObjectToClipPos(worldPosition);

                o.colour = lerp(_Colour2, _Colour1, v.uv.y);

                //o.colour = (c, c, c, c); // white-black wind map

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.colour;
            }
            ENDCG
        }
    }
}
