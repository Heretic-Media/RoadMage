Shader "Custom/StencilMask"
{
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        Pass {
            ColorMask 0
            ZWrite Off

            Stencil {
                Ref 1
                Comp always
                Pass replace
            }
        }
    } 
}