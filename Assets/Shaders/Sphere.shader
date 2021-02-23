Shader "Custom/Sphere"
{
    Properties
    {
        _MainColor("RimColor", Color) = (0, 0.6909037, 1, 0)
        _RimPower("RimPower", Range(0, 5)) = 1
        _Speed("Speed", Range(0, 0.1)) = 0.01
        _Amplitude("Amplitude", Range(0, 1)) = 0.1
        _GlowSpeed("GlowSpeed", Range(0, 2)) = 1
        _Highlight("HighlightColor", Color) = (0.01013041, 0, 1, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert fullforwardshadows alpha:premul

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        
        inline float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)) * 46839.32);
            return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }

        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);

            for(int y=-1; y<=1; y++)
            {
                for(int x=-1; x<=1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);

                    if(d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                    }
                }
            }
        }

        inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
        {
            return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
        }

        inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
        {
            return (1.0-t)*a + (t*b);
        }


        inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);

            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
            float r3 = Unity_SimpleNoise_RandomValue_float(c3);

            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
            return t;
        }

        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
        {
            float t = 0.0;

            float freq = pow(2.0, float(0));
            float amp = pow(0.5, float(3-0));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            Out = t;
        }

        struct Input
        {
            float3 viewDir;
            float3 worldNormal;
            float3 objPos;
        };

        void vert(inout appdata_full v, out Input o){
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.objPos = v.vertex;
        }

        float4 _MainColor;
        float _RimPower;
        float _Speed;
        float _Amplitude;
        float _GlowSpeed;
        float4 _Highlight;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 color = _MainColor;
            float speed = _Speed;
            speed *= _Time.x;
            float3 pos = IN.objPos + (speed.xxx);
            float voronoi;
            Unity_Voronoi_float((pos.xy), 2, 5, voronoi);
            float gSpeed = _Time.x * _GlowSpeed;;
            float3 gPos = IN.objPos + (gSpeed.xxx);
            float noise;
            Unity_SimpleNoise_float((gPos.xy), 50, noise);
            voronoi *= noise;
            color *= voronoi.xxxx;
            float4 highlight = _Highlight * ((1-voronoi).xxxx);
            color += highlight;
            float fresnel = pow((1.0 - saturate(dot(normalize(IN.worldNormal), normalize(IN.viewDir)))), _RimPower);
            color *= (fresnel.xxxx);
            o.Albedo = color.xyz;
            o.Emission = color.xyz;
            o.Metallic = 0;
            o.Smoothness = 0.5;
            o.Alpha = fresnel;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
