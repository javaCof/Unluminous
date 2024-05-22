Shader "Custom/PortalIcon"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "White" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
            "RenderType" = "Overlay"
        }

        Pass
        {
            Lighting Off
            ZWrite Off
            ZTest Always
            Cull Back
            Fog
            {
                Mode Off
            }

            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture[_MainTex]
            {
            
            Combine texture
            }
        }
    }
    FallBack "Unlit/Texture"
}
