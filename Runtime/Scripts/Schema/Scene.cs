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

using UnityEngine;

namespace GLTFast.Schema
{

    /// <summary>
    /// Scene, the top level hierarchy object.
    /// </summary>
    [System.Serializable]
    public class Scene : NamedObject
    {

        /// <summary>
        /// The indices of all root nodes
        /// </summary>
        public uint[] nodes;

        public Extras extras;

        [System.Serializable]
        public struct Extras
        {
            public LightSettings lightSettings;
            public UnityMaterials unityMaterials;

            public override string ToString()
            {
                if (unityMaterials.uri == null || unityMaterials.uri.Length == 0)
                {
                    return $"{{\"lightSettings\":{JsonUtility.ToJson(lightSettings)}}}";
                }
                return JsonUtility.ToJson(this);
            }
        }

        [System.Serializable]
        public struct LightSettings
        {
            public int ambientMode;
            public float intensityMultiplier;
            public Vector4 skyColour;
            public Vector4 equatorColour;
            public Vector4 groundColour;
            public Vector4 ambientColour;
        }

        [System.Serializable]
        public struct UnityMaterials
        {
            public string uri;
            public int bufferView;
        }

        internal void GltfSerialize(JsonWriter writer)
        {
            writer.AddObject();
            GltfSerializeRoot(writer);
            writer.AddArrayProperty("nodes", nodes);
            writer.AddProperty("extras", extras);

            writer.Close();
        }
    }
}
