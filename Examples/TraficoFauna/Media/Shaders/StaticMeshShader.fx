/*
Copyright 2012 Educ.ar S.E.

This file is part of "Trafico de Fauna".

"Trafico de Fauna" is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

"Trafico de Fauna" is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with "Trafico de Fauna".  If not, see <http://www.gnu.org/licenses/>.
*/


/*
* Shader para Static Mesh
*/

/**************************************************************************************/
/* DEFAULT */
/**************************************************************************************/

//Variables utilizadas por el Vertex Shader
float4x4 matWorld;
float4x4 matWorldView;
float4x4 matWorldViewProj;

//Input del Vertex Shader
struct VS_INPUT_DEFAULT 
{
   float4 Position : POSITION0;
   float3 Normal :   NORMAL0;
   float4 Color : COLOR;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DEFAULT
{
   float4 Position :        POSITION0;
   float2 Texcoord :        TEXCOORD0;
   float3 Normal :          TEXCOORD3;
};


//Vertex Shader
VS_OUTPUT_DEFAULT vs_default( VS_INPUT_DEFAULT Input )
{
   VS_OUTPUT_DEFAULT Output;

   //Proyectar posicion
   Output.Position         = mul( Input.Position, matWorldViewProj );
   
   //Las Texcoord quedan igual
   Output.Texcoord         = Input.Texcoord;
   
   //Proyectar normal
   Output.Normal           = mul( Input.Normal, matWorld );
      
   return( Output );
}



//Textura utilizada por el Pixel Shader
texture diffuseMap_Tex;
sampler2D diffuseMap = sampler_state
{
   Texture = (diffuseMap_Tex);
};


//Input del Pixel Shader
struct PS_INPUT_DEFAULT 
{
   float2 Texcoord :        TEXCOORD0;
   float3 Normal :          TEXCOORD3; 
};

//Pixel Shader
float4 ps_default( PS_INPUT_DEFAULT Input ) : COLOR0
{      
	float4 fvBaseColor = tex2D( diffuseMap, Input.Texcoord );
	return fvBaseColor;
	
	//return abs(float4(normalize(Input.Normal.xyz), 1));
}



/*
* Default
*/
technique DefaultTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_default();
   }
}

/**************************************************************************************/
/* DEFAULT ONLY COLOR */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_ONLY_COLOR 
{
   float4 Position : POSITION0;
   float3 Normal :   NORMAL0;
   float4 Color : COLOR;
};

//Output del Vertex Shader
struct VS_OUTPUT_ONLY_COLOR
{
   float4 Position : POSITION0;
   float4 Color : COLOR;
};


//Vertex Shader
VS_OUTPUT_ONLY_COLOR vs_onlyColor( VS_INPUT_ONLY_COLOR Input )
{
   VS_OUTPUT_ONLY_COLOR Output;

	//Proyectar posicion
	Output.Position = mul( Input.Position, matWorldViewProj );
	Output.Color = Input.Color;
      
   return( Output );
}

//Input del Pixel Shader
struct PS_INPUT_ONLY_COLOR 
{
   float4 Color : COLOR0; 
};

//Pixel Shader
float4 ps_onlyColor( PS_INPUT_ONLY_COLOR Input ) : COLOR0
{      
	return Input.Color;
}

/*
* Default only color
*/
technique OnlyColorTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_onlyColor();
	  PixelShader = compile ps_2_0 ps_onlyColor();
   }
}

/**************************************************************************************/
/* LIGHTMAP*/
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_LIGHTMAP
{
   float4 Position : POSITION0;
   float3 Normal :   NORMAL0;
   float4 Color : COLOR;
   float2 Texcoord : TEXCOORD0;
   float2 TexcoordLightmap : TEXCOORD1;
};

//Output del Vertex Shader
struct VS_OUTPUT_LIGHTMAP
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   float2 TexcoordLightmap : TEXCOORD1;
   float3 Normal : TEXCOORD2;
};

//Vertex Shader
VS_OUTPUT_LIGHTMAP vs_lightmap( VS_INPUT_LIGHTMAP Input )
{
   VS_OUTPUT_LIGHTMAP Output;

   //Proyectar posicion
   Output.Position = mul( Input.Position, matWorldViewProj );
   
   //Las Texcoord quedan igual
   Output.Texcoord = Input.Texcoord;
   Output.TexcoordLightmap = Input.TexcoordLightmap;

   //Proyectar normal
   Output.Normal = mul( Input.Normal, matWorld );
      
   return( Output );
}

//Textura para Lightmap
texture lightMap_Tex;
sampler2D lightMap = sampler_state
{
   Texture = (lightMap_Tex);
};

//Input del Pixel Shader
struct PS_INPUT_LIGHTMAP
{
   float2 Texcoord : TEXCOORD0;
   float2 TexcoordLightmap : TEXCOORD1;
   float3 Normal : TEXCOORD2; 
};

//Pixel Shader
float4 ps_lightmap( PS_INPUT_LIGHTMAP Input ) : COLOR0
{      
	float4 baseColor = tex2D( diffuseMap, Input.Texcoord );
	float4 lightmapColor = tex2D( lightMap, Input.TexcoordLightmap );
	return baseColor * lightmapColor;
	
	//return baseColor;
}



technique LightmapTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_lightmap();
	  PixelShader = compile ps_2_0 ps_lightmap();
   }
}


/**************************************************************************************/
/* RESALTADO */
/**************************************************************************************/

float4 color_resaltado = float4(0, 0, 0, 1);
float scaleFactor = 1;

//Pixel Shader
float4 ps_resaltado( PS_INPUT_DEFAULT Input ) : COLOR0
{      
	float4 fvBaseColor = tex2D( diffuseMap, Input.Texcoord );
	//float4 resaltado = float4(color_resaltado.rgb * scaleFactor, 0);
	return fvBaseColor + color_resaltado * scaleFactor;
}

/*
* Resaltado
*/
technique ResaltadoTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_resaltado();
   }
}


/**************************************************************************************/
/* PHONG SHADING */
/**************************************************************************************/

//Output del Vertex Shader
struct VS_OUTPUT_PHONG 
{
   float4 Position :        POSITION0;
   float2 Texcoord :        TEXCOORD0;
   float3 ViewDirection :   TEXCOORD1;
   float3 LightDirection :  TEXCOORD2;
   float3 Normal :          TEXCOORD3;
};

//Variables del vs Phong
float3 fvLightPosition = float3( -100.00, 100.00, -100.00 );
float3 fvEyePosition = float3( 0.00, 0.00, -100.00 );

//Vertex Shader
VS_OUTPUT_PHONG vs_phong( VS_INPUT_DEFAULT Input )
{
   VS_OUTPUT_PHONG Output;

   //Proyectar posicion
   Output.Position         = mul( Input.Position, matWorldViewProj );
   
   //Las Texcoord quedan igual
   Output.Texcoord         = Input.Texcoord;
   
   //Obtener direccion a la que mira la camara y direccion de la luz
   float3 fvObjectPosition = mul( Input.Position, matWorld );
   Output.ViewDirection    = fvEyePosition - fvObjectPosition;
   Output.LightDirection   = fvLightPosition - fvObjectPosition;
   
   //Proyectar normal
   Output.Normal           = mul( Input.Normal, matWorld );
      
   return( Output );
}

//Variables utilizadas por el Pixel Shader
float4 fvAmbient = float4( 0.37, 0.38, 0.37, 1.00 );
float4 fvDiffuse = float4( 0.89, 0.89, 0.33, 1.00 );
float4 fvSpecular = float4( 0.49, 1.00, 0.49, 1.00 );
float fSpecularPower = float( 16.84 );


//Input del Pixel Shader
struct PS_INPUT_PHONG 
{
   float2 Texcoord :        TEXCOORD0;
   float3 ViewDirection :   TEXCOORD1;
   float3 LightDirection:   TEXCOORD2;
   float3 Normal :          TEXCOORD3;
   
};

//Pixel Shader
float4 ps_phong( PS_INPUT_PHONG Input ) : COLOR0
{      
	//Aplicar algoritmo de Diffuse Light
   float3 fvLightDirection = normalize( Input.LightDirection );
   float3 fvNormal         = normalize( Input.Normal );
   float  fNDotL           = dot( fvNormal, fvLightDirection ); 
   
   //Aplicar algoritmo de Specular Light
   float3 fvReflection     = normalize( ( ( 2.0f * fvNormal ) * ( fNDotL ) ) - fvLightDirection ); 
   float3 fvViewDirection  = normalize( Input.ViewDirection );
   float  fRDotV           = max( 0.0f, dot( fvReflection, fvViewDirection ) );
   
   //Obtener el texel de textura
   float4 fvBaseColor      = tex2D( diffuseMap, Input.Texcoord );
   
   //Sumar Ambient + Diffuse + Specular
   float4 fvTotalAmbient   = fvAmbient * fvBaseColor; 
   float4 fvTotalDiffuse   = fvDiffuse * fNDotL * fvBaseColor; 
   float4 fvTotalSpecular  = fvSpecular * pow( fRDotV, fSpecularPower );
   
   //Saturar para no pasarse del máximo color
   return( saturate( fvTotalAmbient + fvTotalDiffuse + fvTotalSpecular ) );
}



/*
* Technique Phong
*/
technique PhongTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_phong();
	  PixelShader = compile ps_2_0 ps_phong();
   }
}



/**************************************************************************************/
/* ANIMATE_UV */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_ANIMATE_UV 
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_ANIMATE_UV
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
};


//Vertex Shader
VS_OUTPUT_ANIMATE_UV vs_animate_uv( VS_INPUT_ANIMATE_UV Input )
{
   VS_OUTPUT_ANIMATE_UV Output;

   //Proyectar posicion
   Output.Position = mul( Input.Position, matWorldViewProj );
   
   //Las Texcoord quedan igual
   Output.Texcoord = Input.Texcoord;

   return Output;
}


float2 animateUV = float2(0, 0);

//Pixel Shader
float4 ps_animate_uv(float2 Texcoord: TEXCOORD0) : COLOR 
{
	float2 uv = Texcoord + animateUV;
	//uv.u = uv.u % 1;
	//uv.v = uv.v % 1;
	return tex2D( diffuseMap, uv );
}

technique AnimateUVTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_animate_uv();
	  PixelShader = compile ps_2_0 ps_animate_uv();
   }
}


