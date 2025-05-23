// SPDX-FileCopyrightText: 2023 Unity Technologies and the glTFast authors
// SPDX-License-Identifier: Apache-2.0

using System;

namespace GLTFast.Schema
{

    /// <summary>
    /// The topology type of primitives to render
    /// </summary>
    /// <seealso href="https://www.khronos.org/registry/glTF/specs/2.0/glTF-2.0.html#_mesh_primitive_mode"/>
    public enum DrawMode
    {
        /// <summary>Points</summary>
        Points = 0,
        /// <summary>Lines</summary>
        Lines = 1,
        /// <summary>Line loop</summary>
        LineLoop = 2,
        /// <summary>Line strip</summary>
        LineStrip = 3,
        /// <summary>Triangles</summary>
        Triangles = 4,
        /// <summary>Triangle strip</summary>
        TriangleStrip = 5,
        /// <summary>Triangle fan</summary>
        TriangleFan = 6
    }

    /// <inheritdoc />
    [Serializable]
    public class MeshPrimitive : MeshPrimitiveBase<MeshPrimitiveExtensions> { }

    /// <inheritdoc />
    /// <typeparam name="TExtensions">Mesh primitive extensions type</typeparam>
    [Serializable]
    public class MeshPrimitiveBase<TExtensions> : MeshPrimitiveBase
    where TExtensions : MeshPrimitiveExtensions
    {
        /// <inheritdoc cref="Extensions"/>
        public TExtensions extensions;

        /// <inheritdoc />
        public override MeshPrimitiveExtensions Extensions => extensions;

        /// <inheritdoc />
        internal override void UnsetExtensions()
        {
            extensions = null;
        }
    }

    /// <summary>
    /// Geometry to be rendered with the given material.
    /// </summary>
    [Serializable]
    public abstract class MeshPrimitiveBase : ICloneable, IMaterialsVariantsSlot
    {

        /// <summary>
        /// A dictionary object, where each key corresponds to mesh attribute semantic
        /// and each value is the index of the accessor containing attribute's data.
        /// </summary>
        public Attributes attributes;

        /// <summary>
        /// The index of the accessor that contains mesh indices.
        /// When this is not defined, the primitives should be rendered without indices
        /// using `drawArrays()`. When defined, the accessor must contain indices:
        /// the `bufferView` referenced by the accessor must have a `target` equal
        /// to 34963 (ELEMENT_ARRAY_BUFFER); a `byteStride` that is tightly packed,
        /// i.e., 0 or the byte size of `componentType` in bytes;
        /// `componentType` must be 5121 (UNSIGNED_BYTE), 5123 (UNSIGNED_SHORT)
        /// or 5125 (UNSIGNED_INT), the latter is only allowed
        /// when `OES_element_index_uint` extension is used; `type` must be `\"SCALAR\"`.
        /// </summary>
        public int indices = -1;

        /// <summary>
        /// The index of the material to apply to this primitive when rendering.
        /// </summary>
        public int material = -1;

        /// <summary>
        /// The type of primitives to render. All valid values correspond to WebGL enums.
        /// </summary>
        public DrawMode mode = DrawMode.Triangles;

        /// <summary>
        /// An array of Morph Targets, each  Morph Target is a dictionary mapping
        /// attributes to their deviations
        /// in the Morph Target (index of the accessor containing the attribute
        /// displacements' data).
        /// </summary>
        public MorphTarget[] targets;

        /// <inheritdoc cref="MeshPrimitiveExtensions"/>
        public abstract MeshPrimitiveExtensions Extensions { get; }

        /// <inheritdoc />
        public int GetMaterialIndex(int variantIndex)
        {
            var mapping = Extensions?.KHR_materials_variants;
            if (mapping != null && mapping.TryGetMaterialIndex(variantIndex, out var materialIndex))
            {
                return materialIndex;
            }
            return material;
        }

        /// <summary>`
        /// Sets <see cref="Extensions"/> to null.
        /// </summary>
        internal abstract void UnsetExtensions();

#if DRACO_UNITY
        public bool IsDracoCompressed => Extensions!=null && Extensions.KHR_draco_mesh_compression != null;
#endif

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        // TODO: Remove upon next major release. This serves no purpose anymore except keeping the API intact.
        public override bool Equals(object obj)
        {
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return base.Equals(obj);
        }

        /// <summary>
        /// Default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        // TODO: Remove upon next major release. This serves no purpose anymore except keeping the API intact.
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns>Member-wise clone</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        internal void GltfSerialize(JsonWriter writer)
        {
            writer.AddObject();
            if (attributes != null)
            {
                writer.AddProperty("attributes");
                attributes.GltfSerialize(writer);
            }
            if (indices >= 0)
            {
                writer.AddProperty("indices", indices);
            }
            if (material >= 0)
            {
                writer.AddProperty("material", material);
            }
            if (mode != DrawMode.Triangles)
            {
                writer.AddProperty("mode", (int)mode);
            }
            if (targets != null)
            {
                writer.AddArray("targets");
                foreach (var target in targets)
                {
                    target.GltfSerialize(writer);
                }
                writer.CloseArray();
            }
            if (Extensions != null)
            {
                writer.AddProperty("extensions");
                Extensions.GltfSerialize(writer);
            }
            writer.Close();
        }
    }

    /// <summary>
    /// Mesh vertex attribute collection. Each property value is the index of
    /// the accessor containing attribute’s data.
    /// </summary>
    [Serializable]
    public class Attributes
    {

        // Names are identical to glTF specified property names, that's why
        // inconsistent names are ignored.
        // ReSharper disable InconsistentNaming

        /// <summary>Vertex position accessor index.</summary>
        public int POSITION = -1;
        /// <summary>Vertex normals accessor index.</summary>
        public int NORMAL = -1;
        /// <summary>Vertex tangents accessor index.</summary>
        public int TANGENT = -1;
        /// <summary>Texture coordinates accessor index.</summary>
        public int TEXCOORD_0 = -1;
        /// <summary>Texture coordinates accessor index (second UV set).</summary>
        public int TEXCOORD_1 = -1;
        /// <summary>Texture coordinates accessor index (third UV set).</summary>
        public int TEXCOORD_2 = -1;
        /// <summary>Texture coordinates accessor index (fourth UV set).</summary>
        public int TEXCOORD_3 = -1;
        /// <summary>Texture coordinates accessor index (fifth UV set).</summary>
        public int TEXCOORD_4 = -1;
        /// <summary>Texture coordinates accessor index (sixth UV set).</summary>
        public int TEXCOORD_5 = -1;
        /// <summary>Texture coordinates accessor index (seventh UV set).</summary>
        public int TEXCOORD_6 = -1;
        /// <summary>Texture coordinates accessor index (eighth UV set).</summary>
        public int TEXCOORD_7 = -1;
        /// <summary>Texture coordinates accessor index (ninth UV set).</summary>
        public int TEXCOORD_8 = -1;
        /// <summary>Vertex color accessor index.</summary>
        public int COLOR_0 = -1;
        /// <summary>Bone joints accessor index.</summary>
        public int JOINTS_0 = -1;
        /// <summary>Bone weights accessor index.</summary>
        public int WEIGHTS_0 = -1;

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        // TODO: Remove upon next major release. This serves no purpose anymore except keeping the API intact.
        public override bool Equals(object obj)
        {
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return base.Equals(obj);
        }

        /// <summary>
        /// Default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        // TODO: Remove upon next major release. This serves no purpose anymore except keeping the API intact.
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        /// <summary>
        /// Calculates the texture coordinate set quantity.
        /// </summary>
        /// <returns>Texture coordinate set quantity.</returns>
        public int GetTexCoordsCount()
        {
            if (TEXCOORD_0 < 0) return 0;
            if (TEXCOORD_1 < 0) return 1;
            if (TEXCOORD_2 < 0) return 2;
            if (TEXCOORD_3 < 0) return 3;
            if (TEXCOORD_4 < 0) return 4;
            if (TEXCOORD_5 < 0) return 5;
            if (TEXCOORD_6 < 0) return 6;
            if (TEXCOORD_7 < 0) return 7;
            return TEXCOORD_8 < 0 ? 8 : 9;
        }

        /// <summary>
        /// Tries to consolidate all `TEXCOORD_*` accessor fields into a single array.
        /// </summary>
        /// <param name="uvAccessors">Resulting array of accessor indices.</param>
        /// <param name="limitExceeded">If true, the glTF has more UV sets than glTFast supports.</param>
        /// <returns>True if there's one or more UV sets and the result is valid. False otherwise.</returns>
        public bool TryGetAllUVAccessors(out int[] uvAccessors, out bool limitExceeded)
        {
            if (TEXCOORD_0 >= 0)
            {
                var uvCount = GetTexCoordsCount();
                uvAccessors = new int[uvCount];
                uvAccessors[0] = TEXCOORD_0;
                if (TEXCOORD_1 >= 0)
                {
                    uvAccessors[1] = TEXCOORD_1;
                }
                if (TEXCOORD_2 >= 0)
                {
                    uvAccessors[2] = TEXCOORD_2;
                }
                if (TEXCOORD_3 >= 0)
                {
                    uvAccessors[3] = TEXCOORD_3;
                }
                if (TEXCOORD_4 >= 0)
                {
                    uvAccessors[4] = TEXCOORD_4;
                }
                if (TEXCOORD_5 >= 0)
                {
                    uvAccessors[5] = TEXCOORD_5;
                }
                if (TEXCOORD_6 >= 0)
                {
                    uvAccessors[6] = TEXCOORD_6;
                }
                if (TEXCOORD_7 >= 0)
                {
                    uvAccessors[7] = TEXCOORD_7;
                }
                limitExceeded = TEXCOORD_8 >= 0;
                return true;
            }

            uvAccessors = null;
            limitExceeded = false;
            return false;
        }

        internal void GltfSerialize(JsonWriter writer)
        {
            writer.AddObject();
            if (POSITION >= 0) writer.AddProperty("POSITION", POSITION);
            if (NORMAL >= 0) writer.AddProperty("NORMAL", NORMAL);
            if (TANGENT >= 0) writer.AddProperty("TANGENT", TANGENT);
            if (TEXCOORD_0 >= 0) writer.AddProperty("TEXCOORD_0", TEXCOORD_0);
            if (TEXCOORD_1 >= 0) writer.AddProperty("TEXCOORD_1", TEXCOORD_1);
            if (TEXCOORD_2 >= 0) writer.AddProperty("TEXCOORD_2", TEXCOORD_2);
            if (TEXCOORD_3 >= 0) writer.AddProperty("TEXCOORD_3", TEXCOORD_3);
            if (TEXCOORD_4 >= 0) writer.AddProperty("TEXCOORD_4", TEXCOORD_4);
            if (TEXCOORD_5 >= 0) writer.AddProperty("TEXCOORD_5", TEXCOORD_5);
            if (TEXCOORD_6 >= 0) writer.AddProperty("TEXCOORD_6", TEXCOORD_6);
            if (TEXCOORD_7 >= 0) writer.AddProperty("TEXCOORD_7", TEXCOORD_7);
            if (COLOR_0 >= 0) writer.AddProperty("COLOR_0", COLOR_0);
            if (JOINTS_0 >= 0) writer.AddProperty("JOINTS_0", JOINTS_0);
            if (WEIGHTS_0 >= 0) writer.AddProperty("WEIGHTS_0", WEIGHTS_0);
            writer.Close();
        }
    }

    /// <summary>
    /// Mesh primitive extensions
    /// </summary>
    [Serializable]
    public class MeshPrimitiveExtensions
    {
#if DRACO_UNITY
        // ReSharper disable once InconsistentNaming
        public MeshPrimitiveDracoExtension KHR_draco_mesh_compression;
#endif

        /// <inheritdoc cref="MaterialsVariantsMeshPrimitiveExtension"/>
        // ReSharper disable once InconsistentNaming
        public MaterialsVariantsMeshPrimitiveExtension KHR_materials_variants;

        internal void GltfSerialize(JsonWriter writer)
        {
            writer.AddObject();
#if DRACO_UNITY
            if (KHR_draco_mesh_compression != null) {
                writer.AddProperty("KHR_draco_mesh_compression");
                KHR_draco_mesh_compression.GltfSerialize(writer);
            }
#endif
            if (KHR_materials_variants != null)
            {
                writer.AddProperty("KHR_materials_variants");
                KHR_materials_variants.GltfSerialize(writer);
            }
            writer.Close();
        }
    }

#if DRACO_UNITY
    [Serializable]
    public class MeshPrimitiveDracoExtension {
        public int bufferView;
        public Attributes attributes;

        internal void GltfSerialize(JsonWriter writer) {
            writer.AddObject();
            writer.AddProperty("bufferView", bufferView);
            writer.AddProperty("attributes");
            attributes.GltfSerialize(writer);
            writer.Close();
        }
    }
#endif

    /// <summary>
    /// Morph target (blend shape)
    /// </summary>
    [Serializable]
    public class MorphTarget
    {
        // Names are identical to glTF specified property names, that's why
        // inconsistent names are ignored.
        // ReSharper disable InconsistentNaming

        /// <summary>Vertex position deviation accessor index.</summary>
        public int POSITION = -1;
        /// <summary>Vertex normal deviation accessor index.</summary>
        public int NORMAL = -1;
        /// <summary>Vertex tangent deviation accessor index.</summary>
        public int TANGENT = -1;

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        // TODO: Remove upon next major release. This serves no purpose anymore except keeping the API intact.
        public override bool Equals(object obj)
        {
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return base.Equals(obj);
        }

        /// <summary>
        /// Default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        // TODO: Remove upon next major release. This serves no purpose anymore except keeping the API intact.
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        internal void GltfSerialize(JsonWriter writer)
        {
            if (POSITION >= 0) writer.AddProperty("POSITION", POSITION);
            if (NORMAL >= 0) writer.AddProperty("NORMAL", NORMAL);
            if (TANGENT >= 0) writer.AddProperty("TANGENT", TANGENT);
        }
    }
}
