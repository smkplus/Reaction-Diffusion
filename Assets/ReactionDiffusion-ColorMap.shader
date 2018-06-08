// adapted from http://bl.ocks.org/robinhouston/ed597847175cf692ecce

Shader "Custom/ReactionDiffusion-ColorMap" {
        Properties {
         _MainTex ("_MainTex", 2D) = "white" {}
    }

    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;

            fixed4 frag(v2f_img i) : SV_Target {     
            	float2 diff = tex2D(_MainTex, i.uv).rg;
            	float COLOR_MIN = 0.2;
            	float COLOR_MAX = 0.4;
            	float v = (COLOR_MAX - diff.g) / (COLOR_MAX - COLOR_MIN);
				return float4(v,v,v,1);
            }

            ENDCG
        }
    }
 }