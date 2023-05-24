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
* Shader para efectos de particulas
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
};

//Output del Vertex Shader
struct VS_OUTPUT_DEFAULT
{
   float4 Position : POSITION0;
};


//Vertex Shader
VS_OUTPUT_DEFAULT vs_default( VS_INPUT_DEFAULT Input )
{
   VS_OUTPUT_DEFAULT Output;

   //Proyectar posicion
   Output.Position = mul( Input.Position, matWorldViewProj );
      
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
   float2 TexCoord : TEXCOORD0;
};

//Pixel Shader
float4 ps_default( PS_INPUT_DEFAULT Input ) : COLOR0
{      
	return tex2D( diffuseMap, Input.Texcoord );
}



/*
* Default default
*/
technique DefaultTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_default();
   }
}

