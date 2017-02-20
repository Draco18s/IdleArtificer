Shader "custom/Invert" {
	Properties
	{
		_Color ("Tint Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags { "Queue"="Transparent" }

		Pass
		{
			ZWrite On
			ColorMask 0
		}
		Pass
		{
			Blend OneMinusDstColor Zero

	}
	}
}
