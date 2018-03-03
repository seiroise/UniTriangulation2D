Shader "UniTriangulation2D/DemoShader" {
	Properties {
		_MaskPosition ("Mask Position", vector) = (0,0,0,0)
		_Distance ("Visible Distance", float) = 10.0
		_Width ("Width", Range(0.0, 1.0)) = 0.1
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Blend One OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _MaskPosition;
			float _Distance;
			float _Width;
			fixed4 _Color;

			struct vertexInput {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 wpos : TEXCOORD0;
				fixed4 color : COLOR;
			};
  
			vertexOutput vert(vertexInput i) {
				vertexOutput o; 
				o.pos =  UnityObjectToClipPos(i.vertex);
				o.wpos = mul(unity_ObjectToWorld, i.vertex);
				o.color = i.color;
				return o;
			}

			fixed4 frag(vertexOutput i) : COLOR {
				float dist = distance(i.wpos, _MaskPosition);
				float t1 = step(_Distance, dist);
				float t2 = step(dist, _Distance + _Distance * _Width);
				/*
				float2 diff = _MaskPosition.xy - i.wpos.xy;
				float rad = atan2(diff.x, diff.y);

				float div = (rad + _Time.y + 3.1415) % 0.31415;
				float r = step(div, 0.31415 * 0.5);

				return _Color * i.color * (t1 - r * t2 * t1);
				*/
				return _Color * i.color * t2;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}