// Copyright 2020-2022 Andreas Atteneder
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Unity.Mathematics;
using UnityEngine;

namespace GLTFast.Schema
{

    /// <summary>
    /// The material appearance of a primitive.
    /// </summary>
    [System.Serializable]
    public class Material : NamedObject
    {

        /// <summary>
        /// The material’s alpha rendering mode enumeration specifying the
        /// interpretation of the alpha value of the base color.
        /// </summary>
        public enum AlphaMode
        {
            /// <summary>
            /// The alpha value is ignored, and the rendered output is fully
            /// opaque.
            /// </summary>
            Opaque,

            /// <summary>
            /// The rendered output is either fully opaque or fully transparent
            /// depending on the alpha value and the specified alphaCutoff
            /// value
            /// </summary>
            Mask,

            /// <summary>
            /// The alpha value is used to composite the source and destination
            /// areas. The rendered output is combined with the background
            /// using the normal painting operation.
            /// </summary>
            Blend
        }

        /// <summary>
        /// The material's skybox mode enumeration specifying the type of skybox
        /// and usage of the attached texture(s)
        /// </summary>
        public enum SkyboxMode
        {
            /// <summary>
            /// In 6-sided mode, 6 textures are provided to form the cube map.
            /// </summary>
            SixSided,

            /// <summary>
            /// In cube map mode, a single equi-rectangular texture is used
            /// to create the cube map. This mode is combined with Unity's
            /// Panoramic skybox, since the texture usage is the same.
            /// </summary>
            CubeMap,

            /// <summary>
            /// Procedural skyboxes are created with code rather than textures,
            /// so the relevant properties are specified for skybox replication
            /// upon import.
            /// </summary>
            Procedural
        }

        /// <summary>
        /// Material extensions.
        /// </summary>
        public MaterialExtension extensions;


        /// <summary>
        /// A set of parameter values that are used to define the metallic-roughness
        /// material model from Physically-Based Rendering (PBR) methodology.
        /// </summary>
        public PbrMetallicRoughness pbrMetallicRoughness;

        // /// <summary>
        // /// A set of parameter values used to light flat-shaded materials
        // /// </summary>
        // public MaterialCommonConstant CommonConstant;

        /// <summary>
        /// A tangent space normal map. Each texel represents the XYZ components of a
        /// normal vector in tangent space.
        /// </summary>
        public NormalTextureInfo normalTexture;

        /// <summary>
        /// The occlusion map is a greyscale texture, with white indicating areas that
        /// should receive full indirect lighting and black indicating no indirect
        /// lighting.
        /// </summary>
        public OcclusionTextureInfo occlusionTexture;

        /// <summary>
        /// The emissive map controls the color and intensity of the light being emitted
        /// by the material. This texture contains RGB components in sRGB color space.
        /// If a fourth component (A) is present, it is ignored.
        /// </summary>
        public TextureInfo emissiveTexture;

        /// <summary>
        /// The RGB components of the emissive color of the material.
        /// If an emissiveTexture is specified, this value is multiplied with the texel
        /// values.
        /// <items>
        ///  <minimum>0.0</minimum>
        ///  <maximum>1.0</maximum>
        /// </items>
        /// <minItems>3</minItems>
        /// <maxItems>3</maxItems>
        /// </summary>
        [SerializeField]
        float[] emissiveFactor = { 0, 0, 0 };

        /// <summary>
        /// Emissive color of the material.
        /// </summary>
        public Color Emissive
        {
            get => new Color(
                emissiveFactor[0],
                emissiveFactor[1],
                emissiveFactor[2]
                );
            set => emissiveFactor = new[] { value.r, value.g, value.b };
        }

        /// <summary>
        /// The material's alpha rendering mode enumeration specifying the interpretation of the
        /// alpha value of the main factor and texture. In `OPAQUE` mode, the alpha value is
        /// ignored and the rendered output is fully opaque. In `MASK` mode, the rendered output
        /// is either fully opaque or fully transparent depending on the alpha value and the
        /// specified alpha cutoff value. In `BLEND` mode, the alpha value is used to composite
        /// the source and destination areas. The rendered output is combined with the background
        /// using the normal painting operation (i.e. the Porter and Duff over operator).
        /// </summary>
        [SerializeField]
        public string alphaMode;

        AlphaMode? m_AlphaModeEnum;

        /// <summary>
        /// <see cref="AlphaMode"/> typed and cached getter for <see cref="alphaMode"/> string.
        /// </summary>
        /// <returns>Alpha mode if it was retrieved correctly. <see cref="AlphaMode.Opaque"/> otherwise</returns>
        public AlphaMode GetAlphaMode()
        {
            if (m_AlphaModeEnum.HasValue)
            {
                return m_AlphaModeEnum.Value;
            }

            if (!string.IsNullOrEmpty(alphaMode))
            {
                m_AlphaModeEnum = (AlphaMode)System.Enum.Parse(typeof(AlphaMode), alphaMode, true);
                alphaMode = null;
                return m_AlphaModeEnum.Value;
            }

            return AlphaMode.Opaque;
        }

        /// <summary>
        /// <see cref="AlphaMode"/> typed setter for <see cref="alphaMode"/> string.
        /// </summary>
        /// <param name="mode">Alpha mode</param>
        public void SetAlphaMode(AlphaMode mode)
        {
            m_AlphaModeEnum = mode;
            if (mode != AlphaMode.Opaque)
            {
                alphaMode = mode.ToString().ToUpper();
            }
        }

        [System.Serializable]
        public struct Extras
        {
            public SkyboxData skyboxData;
            public ParticlesUnlitData particlesUnlitData;

            public override string ToString()
            {
                string json = "{";
                if (skyboxData.isSkybox)
                {
                    json += $"\"skyboxData\":{skyboxData.ToString()},";
                }
                if (particlesUnlitData.isParticlesUnlit)
                {
                    json += $"\"particlesUnlitData\":{particlesUnlitData.ToString()}";
                }
                json = json.Trim(',') + "}";
                return json;
            }
        }

        /// <summary>
        /// Structure to store skybox material properties
        /// </summary>
        [System.Serializable]
        public struct ParticlesUnlitData
        {
            // TODO: handle textures for bumpMap and emissionMap
            public bool isParticlesUnlit;
            public Color color;
            public float cutoff;
            public float bumpScale;
            public Color emissionColor;
            public float distortionStrength;
            public float distortionBlend;
            public float softParticlesNearFadeDistance;
            public float softParticlesFarFadeDistance;
            public float cameraNearFadeDistance;
            public float cameraFarFadeDistance;

            public float mode;
            public float colorMode;
            public float flipbookMode;
            public float lightingEnabled;
            public float distortionEnabled;
            public float emissionEnabled;
            public float blendOp;
            public float srcBlend;
            public float dstBlend;
            public float zWrite;
            public float cull;
            public float softParticlesEnabled;
            public float cameraFadingEnabled;
            public Vector4 softParticleFadeParams;
            public Vector4 cameraFadeParams;
            public Vector4 colorAddSubDiff;
            public float distortionStrengthScaled;

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        /// <summary>
        /// Structure to store skybox material properties
        /// </summary>
        [System.Serializable]
        public struct SkyboxData
        {
            public bool isSkybox;
            public SkyboxMode skyboxMode;

            // Cube map and Panoramic properties
            public Color skyTint;
            public float exposure;
            public float rotation;

            // Procedural skybox properties
            public float sunSize;
            public float sunSizeConvergence;
            public float atmosphereThickness;
            public Color ground;

            // 6-sided cube map textures
            public int frontTex;
            public int backTex;
            public int leftTex;
            public int rightTex;
            public int upTex;
            public int downTex;

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        };

        public Extras extras;

        /// <summary>
        /// Specifies the cutoff threshold when in `MASK` mode. If the alpha value is greater than
        /// or equal to this value then it is rendered as fully opaque, otherwise, it is rendered
        /// as fully transparent. This value is ignored for other modes.
        /// </summary>
        public float alphaCutoff = 0.5f;

        /// <summary>
        /// Specifies whether the material is double sided. When this value is false, back-face
        /// culling is enabled. When this value is true, back-face culling is disabled and double
        /// sided lighting is enabled. The back-face must have its normals reversed before the
        /// lighting equation is evaluated.
        /// </summary>
        public bool doubleSided;

        /// <summary>
        /// True if the material requires the mesh to have normals.
        /// </summary>
        public bool RequiresNormals => extensions?.KHR_materials_unlit == null;

        /// <summary>
        /// True if the material requires the mesh to have tangents.
        /// </summary>
        public bool RequiresTangents => normalTexture != null && normalTexture.index >= 0;

        internal void GltfSerialize(JsonWriter writer)
        {
            writer.AddObject();
            GltfSerializeRoot(writer);
            if (pbrMetallicRoughness != null)
            {
                writer.AddProperty("pbrMetallicRoughness");
                pbrMetallicRoughness.GltfSerialize(writer);
            }
            if (normalTexture != null)
            {
                writer.AddProperty("normalTexture");
                normalTexture.GltfSerialize(writer);
            }
            if (occlusionTexture != null)
            {
                writer.AddProperty("occlusionTexture");
                occlusionTexture.GltfSerialize(writer);
            }
            if (emissiveTexture != null)
            {
                writer.AddProperty("emissiveTexture");
                emissiveTexture.GltfSerialize(writer);
            }
            if (emissiveFactor != null
                && (
                    emissiveFactor[0] > Constants.epsilon
                    || emissiveFactor[1] > Constants.epsilon
                    || emissiveFactor[2] > Constants.epsilon)
                )
            {
                writer.AddArrayProperty("emissiveFactor", emissiveFactor);
            }
            if (!string.IsNullOrEmpty(alphaMode))
            {
                writer.AddProperty("alphaMode", alphaMode);
            }
            if (math.abs(alphaCutoff - .5f) > Constants.epsilon)
            {
                writer.AddProperty("alphaCutoff", alphaCutoff);
            }
            if (doubleSided)
            {
                writer.AddProperty("doubleSided", doubleSided);
            }
            if (extras.skyboxData.isSkybox || extras.particlesUnlitData.isParticlesUnlit)
            {
                writer.AddProperty("extras", extras);
            }
            if (extensions != null)
            {
                writer.AddProperty("extensions");
                extensions.GltfSerialize(writer);
            }
            writer.Close();
        }
    }
}
