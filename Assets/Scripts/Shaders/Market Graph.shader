
Shader "Custom/Market Graph" {

	SubShader {
		Tags {"Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM

			#pragma shader_feature __ ENABLE_HIGHLIGHTING
			#pragma vertex VertexFunction
			#pragma fragment FragmentFunction

			#include "UnityCG.cginc"

			uniform int _Data_Count = 0;
			uniform float _Data1[100];
			uniform float _Data2[100];
			uniform float _Data3[100];
			uniform float _Data4[100];

			struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
		    };  

		    struct FragmentData {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
		    };

			FragmentData VertexFunction(VertexData v) {
				FragmentData f;
				f.uv = v.uv;
				f.position = UnityObjectToClipPos(v.position);
				return f;
			}

			half4 FragmentFunction(FragmentData f) : COLOR {
				float rawX = f.uv.x * (_Data_Count - 1);
				int value;

				int x = int(rawX);
				int y = int(f.uv.y * 60);
				float xFrac = rawX - x;

				value = int(lerp(_Data1[x], _Data1[x + 1], xFrac));
				if (value == y) {
					return half4(1, 0, 0, 1);
				}

				value = int(lerp(_Data2[x], _Data2[x + 1], xFrac));
				if (value == y) {
					return half4(1, 1, 0, 1);
				}

				value = int(lerp(_Data3[x], _Data3[x + 1], xFrac));
				if (value == y) {
					return half4(0, 1, 1, 1);
				}

				value = int(lerp(_Data4[x], _Data4[x + 1], xFrac));
				if (value == y) {
					return half4(1, 0, 1, 1);
				}

				if (y > 40) {
					return half4(0.3, 1, 0.3, 0.2);
					//return half4(0.50, 1, 0.2, 0.2);
				} else if (y > 30) {
					return half4(0.3, 0.8, 0.3, 0.2);
					//return half4(0.75, 1, 0.2, 0.2);
				} else if (y > 20) {
					return half4(0.3, 0.6, 0.3, 0.2);
					//return half4(1, 0.75, 0.2, 0.2);
				} else {
					return half4(0.3, 0.4, 0.3, 0.2);
					//return half4(1, 0.50, 0.2, 0.2);
				}

				return half4(0, 0, 0, 0);
			}


			ENDCG
		}
	}

}
