using System;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.MinigameBase.UI
{
    /// <summary>
    /// Used for flipping UI images on several axis.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIFlip : BaseMeshEffect
    {
        /// <summary>
        /// The configuration for how to flip the image.
        /// </summary>
        [SerializeField]
        [Tooltip("The configuration for how to flip the image.")]
        private FlipType _flipType;

        /// <summary>
        /// Modifies the vertices of the mesh to flip the image based on the flip type settings.
        /// </summary>
        /// <param name="vh">Vertex helper of the mesh this effect is on.</param>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (_flipType.HasFlag(FlipType.FLIP_Y))
                SwapVertices(vh, 0, 1, 2, 3);
            
            if (_flipType.HasFlag(FlipType.FLIP_X))
                SwapVertices(vh, 0, 3, 1, 2);
            
            if (_flipType.HasFlag(FlipType.CLOCKWISE))
            {
                UIVertex first = UIVertex.simpleVert;
                vh.PopulateUIVertex(ref first, 0);
                
                for (int i = vh.currentVertCount - 1; i >= 0; i--)
                {
                    UIVertex vert = UIVertex.simpleVert;
                    vh.PopulateUIVertex(ref vert, i);

                    Vector3 target = first.position;
                    first = vert;
                    
                    vert.position = target;
                    
                    vh.SetUIVertex(vert, i);
                }
            }
            if (_flipType.HasFlag(FlipType.COUNTER_CLOCKWISE))
            {
                UIVertex last = UIVertex.simpleVert;
                vh.PopulateUIVertex(ref last, 3);
                
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    UIVertex vert = UIVertex.simpleVert;
                    vh.PopulateUIVertex(ref vert, i);

                    Vector3 target = last.position;
                    last = vert;
                    
                    vert.position = target;
                    
                    vh.SetUIVertex(vert, i);
                }
            }
        }
        
        /// <summary>
        /// Swaps the vertices of two indices to two other indices.
        /// </summary>
        /// <param name="vh">The vertex helper for getting and setting the vertices.</param>
        /// <param name="indexA">The first index to swap.</param>
        /// <param name="indexB">The second index to swap.</param>
        /// <param name="targetIndexA">The first index to swap to.</param>
        /// <param name="targetIndexB">The second index to swap to.</param>
        private void SwapVertices(VertexHelper vh, int indexA, int indexB, int targetIndexA, int targetIndexB)
        {
            UIVertex vertA = UIVertex.simpleVert;
            UIVertex vertB = UIVertex.simpleVert;
            UIVertex temp = UIVertex.simpleVert;
                
            vh.PopulateUIVertex(ref vertA, indexA);
            vh.PopulateUIVertex(ref vertB, indexB);

            temp = vertA;
            vertA.position = vertB.position;
            vertB.position = temp.position;
                
            vh.SetUIVertex(vertA, indexA);
            vh.SetUIVertex(vertB, indexB);
                
            vh.PopulateUIVertex(ref vertA, targetIndexA);
            vh.PopulateUIVertex(ref vertB, targetIndexB);

            temp = vertA;
            vertA.position = vertB.position;
            vertB.position = temp.position;
                
            vh.SetUIVertex(vertA, targetIndexA);
            vh.SetUIVertex(vertB, targetIndexB);
        }
    }

    /// <summary>
    /// The flip type for defining the different flip settings.
    /// </summary>
    [Flags]
    public enum FlipType
    {
        /// <summary>
        /// Flips on the X axis.
        /// </summary>
        [InspectorName("Flip X")]
        FLIP_X = (1 << 0),
        
        /// <summary>
        /// Flips on the Y axis.
        /// </summary>
        [InspectorName("Flip Y")]
        FLIP_Y = (1 << 1),
        
        /// <summary>
        /// Turns the image clockwise 90 degrees.
        /// </summary>
        [InspectorName("Clockwise")]
        CLOCKWISE = (1 << 2),
        
        /// <summary>
        /// Turns the image counter-clockwise 90 degrees.
        /// </summary>
        [InspectorName("Counter Clockwise")]
        COUNTER_CLOCKWISE = (1 << 3)
    }
}
