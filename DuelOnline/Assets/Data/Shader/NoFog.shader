Shader "Unlit/NoFog"
{
    Properties
    {
        _MainTex("Particle Texture", 2D) = "white" {}
    }

        SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        Fog { Mode Off }

        ZWrite Off
        Cull Off
        Blend SrcAlpha One

        Pass
        {
            SetTexture[_MainTex]
            {
                combine texture * primary
            }
        }
    }
}
