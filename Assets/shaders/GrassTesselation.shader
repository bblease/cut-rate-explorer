﻿Shader "Tessellation Sample" {
	Properties{
		_Tess("Tessellation", Range(1,32)) = 4
		_MainTex("Base (RGB)", 2D) = "white" {}
	_DispTex("Disp Texture", 2D) = "gray" {}
	_NormalMap("Normalmap", 2D) = "bump" {}
	_Displacement("Displacement", Range(0, 1.0)) = 0.3
		_Color("Color", color) = (1,1,1,0)
		_SpecColor("Spec color", color) = (0.5,0.5,0.5,0.5)
	}
		SubShader{
		Tags{ "RenderType" = "TransparentCutout" }
		LOD 300

		CGPROGRAM
#pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:disp tessellate:tessFixed nolightmap
#pragma target 4.6

		struct appdata {
		float4 vertex : POSITION;
		float4 tangent : TANGENT;
		float3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
	};

	float _Tess;

	float4 tessFixed()
	{
		return _Tess;
	}

	sampler2D _DispTex;
	float _Displacement;

	void disp(inout appdata v)
	{
		float d = tex2Dlod(_DispTex, float4(v.texcoord.xy,0,0)).r * _Displacement;
		v.vertex.xyz += v.normal * d;
	}

	struct Input {
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
	sampler2D _NormalMap;
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Specular = 0.2;
		o.Gloss = 1.0;
		o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
	}
	ENDCG
	}

	SubShader
    {
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Offset -1, -1
            Fog { Mode Off }
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag


            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Length;
            float _Width;
            half4 _Color;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                half4 color : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.texcoord = v.texcoord;
                o.color = _Color;
                return o;
            }

            half4 frag (v2f IN) : COLOR
            {
                if ((IN.texcoord.x<0) || (IN.texcoord.x>_Width) || (IN.texcoord.y<0) || (IN.texcoord.y>_Length))
                {
                    half4 colorTransparent = half4(0,0,0,0) ;
                    return colorTransparent;
                }
                else
                {
                    half4 tex = tex2D(_MainTex, IN.texcoord);
                    tex.a = IN.color.a;
                    return tex;
                }
            }

            ENDCG
        }
    }

		

	
		
		FallBack "Diffuse"
}