
Shader "Custom/World Map 2x" {

	Properties {
		_RegionMap ("Region Map", 2D) = "white" {}
		[Toggle]
		_EnableHighlighting ("Enable Highlighting", Int) = 0
		[Toggle]
		_EnableDetailedRegionLevels ("Enable Detailed Region Levels", Int) = 0
	}

	SubShader {
		Tags {"Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM

			#pragma shader_feature __ ENABLE_HIGHLIGHTING
			#pragma vertex VertexFunction
			#pragma fragment FragmentFunction

			#include "UnityCG.cginc"

			uniform int _Coverage_Highlight = 0;
			uniform int _Coverage_Count = 0;
			uniform float4 _Coverage[144 * 5];

			uniform float _RegionLevels[20];

			bool _EnableHighlighting;
			bool _EnableDetailedRegionLevels;
			sampler2D _RegionMap;

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

				int regionId = int(tex2D(_RegionMap, f.uv).a * 256);
				float regionLevel = regionId > 0 ? _RegionLevels[regionId] : 0;
				int coverage = 0;
				bool isCoverageHighlighted = false;

				for (int k = 0; k < _Coverage_Count; k++) {
					for (int n = 0; n < 144; n++) {
						int i = k * 144 + n;
						int coverageCellX = int((_Coverage[i].y + 180) / 2);
						int coverageCellY = int((_Coverage[i].x + 90) / 2);
						half dist = distance(int2(cellX, cellY), int2(coverageCellX, coverageCellY));

						if (dist <= _Coverage[i].z) {
							coverage++;

							if (k == _Coverage_Highlight - 1) {
								isCoverageHighlighted = true;
							}

							break;
						}
					}

					if (isCoverageHighlighted) {
						break;
					}
				}

				if (coverage == 0) {
					if (regionId > 0) {
						if (_EnableHighlighting) {
							if (_EnableDetailedRegionLevels) {
								regionLevel = int(regionLevel * 4) / 5.0;
								return half4(0.3, 0.2 + regionLevel, 0.3, 1);
							} else if (regionLevel > 0) {
								return half4(0, 1, 0, 1);
							} else {
								return half4(0.3, 0.3, 0.3, 1);
							}
						} else {
							return half4(0, 1, 1, 1);
						}
					}
				} else {
					half alpha = 1;

					if (regionId == 0) {
						alpha = 0.5;
					}

					if (!_EnableHighlighting || ((isCoverageHighlighted || _Coverage_Highlight == 0) && (regionLevel > 0))) {
						return half4(0.4 + coverage * 0.15, 0, 0, alpha);
					} else if (!isCoverageHighlighted && regionLevel > 0) {
						return half4(0.2, 0, 0, alpha);
					} else if (isCoverageHighlighted && regionLevel == 0) {
						return half4(0.75, 0.75, 0.75, alpha);
					} else {
						return half4(0.5, 0.5, 0.5, alpha);
					}
				}

				return half4(0, 0, 0, 0);
			}


			ENDCG
		}
	}

}
