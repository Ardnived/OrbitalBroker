
Shader "Custom/World Map" {

	Properties {
		_CellMap ("Cell Map", 2D) = "white" {}
		[Toggle]
		_EnableOrbits ("Enable Orbits", Int) = 0
	}

	SubShader {
		Tags {"Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM

			//#pragma shader_feature __ ORBITS
			#pragma vertex VertexFunction
			#pragma fragment FragmentFunction

			#include "UnityCG.cginc"

			bool _EnableOrbits;
			sampler2D _CellMap;

			uniform bool _Orbit_Defined = false;
			uniform float4 _Orbit[144];

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
				float x = f.uv.x * 180;
				float y = f.uv.y * 91;

				int cellX = int(x);
				int cellY = int(y);

				float cellFractionX = x - cellX;
				float cellFractionY = y - cellY;

				if (cellFractionX <= 0.1 || cellFractionX >= 0.9 || cellFractionY <= 0.1 || cellFractionY >= 0.9) {
					return half4(0, 0, 0, 0);
				}

				float level = tex2D(_CellMap, f.uv).a;

				bool inOrbitalPath = false;

				if (_Orbit_Defined) {
					for (int i = 0; i < 144; i++) {
						int orbitCellX = int((_Orbit[i].y + 180) / 2);
						int orbitCellY = int((_Orbit[i].x + 90) / 2);
						half dist = distance(int2(cellX, cellY), int2(orbitCellX, orbitCellY));

						if (dist <= _Orbit[i].z) {
							inOrbitalPath = true;
							break;
						}
					}
				}

				if (_EnableOrbits) {
					if (level > 0) {
						if (inOrbitalPath) {
							return half4(0, 1, 0, 1);
						} else {
							return half4(0, 1, 1, 1);
						}
					} else if (inOrbitalPath) {
						return half4(0, 1, 0, 0.5);
					}
				} else if (level > 0) {
					return half4(0.4, 0.5 + level/2, 0.7 - level/2, 1);
				}

				return half4(0, 0, 0, 0);
			}
			ENDCG
		}
	}

}
