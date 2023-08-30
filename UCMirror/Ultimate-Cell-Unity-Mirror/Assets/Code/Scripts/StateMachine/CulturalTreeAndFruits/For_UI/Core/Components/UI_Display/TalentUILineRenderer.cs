


/// Credit jack.sydorenko, firagon
/// Sourced from - http://forum.unity3d.com/threads/new-ui-and-line-drawing.253772/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Drawing;

namespace com.cygnusprojects.TalentTree
{
   public class TalentUILineRenderer : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
    {
        
        #region Variables
        public UnityEngine.Color colorToAdd;
        [SerializeField]
        private Sprite m_Sprite;
        public Sprite sprite { get { return m_Sprite; } set { if (SetPropertyUtility.SetClass(ref m_Sprite, value)) SetAllDirty(); } }

        public TalentTreeConnectionBase Connection;
        public TalentTreeNodeBase FromTalent;
        public TalentTreeNodeBase ToTalent;

        [NonSerialized]
        private Sprite m_OverrideSprite;
        public Sprite overrideSprite { get { return m_OverrideSprite == null ? sprite : m_OverrideSprite; } set { if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value)) SetAllDirty(); } }

        // Not serialized until we support read-enabled sprites better.
        internal float m_EventAlphaThreshold = 1;
        public float eventAlphaThreshold { get { return m_EventAlphaThreshold; } set { m_EventAlphaThreshold = value; } }

        private enum SegmentType
        {
            Start,
            Middle,
            End,
        }

        public enum JoinType
        {
            Bevel,
            Miter
        }
        public enum BezierType
        {
            None,
            Quick,
            Basic,
            Improved,
        }
        
        private const float MIN_MITER_JOIN = 15 * Mathf.Deg2Rad;

        // A bevel 'nice' join displaces the vertices of the line segment instead of simply rendering a
        // quad to connect the endpoints. This improves the look of textured and transparent lines, since
        // there is no overlapping.
        // 斜角“良好”连接会替换线段的顶点，而不是简单地渲染四边形来连接端点。
        // 由于没有重叠，这改善了纹理线和透明线的外观。
        private const float MIN_BEVEL_NICE_JOIN = 30 * Mathf.Deg2Rad;

        private static readonly Vector2 UV_TOP_LEFT = Vector2.zero;
        private static readonly Vector2 UV_BOTTOM_LEFT = new Vector2(0, 1);
        private static readonly Vector2 UV_TOP_CENTER = new Vector2(0.5f, 0);
        private static readonly Vector2 UV_BOTTOM_CENTER = new Vector2(0.5f, 1);
        private static readonly Vector2 UV_TOP_RIGHT = new Vector2(1, 0);
        private static readonly Vector2 UV_BOTTOM_RIGHT = new Vector2(1, 1);

        private static readonly Vector2[] startUvs = new[] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_CENTER, UV_TOP_CENTER };
        private static readonly Vector2[] middleUvs = new[] { UV_TOP_CENTER, UV_BOTTOM_CENTER, UV_BOTTOM_CENTER, UV_TOP_CENTER };
        private static readonly Vector2[] endUvs = new[] { UV_TOP_CENTER, UV_BOTTOM_CENTER, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };

        [SerializeField]
        private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);
        [SerializeField]
        private Vector2[] m_points;


        public float LineThickness = 2;
        public bool UseMargins;
        public Vector2 Margin;
        public bool relativeSize;

        public bool LineList = false;
        public bool LineCaps = false;
        public JoinType LineJoins = JoinType.Bevel;

        public BezierType BezierMode = BezierType.None;
        public int BezierSegmentsPerCurve = 10;
        #endregion

        #region Properties
        /// <summary>
        /// Image's texture comes from the UnityEngine.Image.
        /// 图像的纹理来自UnityEngine.Image。
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (overrideSprite == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return overrideSprite.texture;
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float spritePixelsPerUnit = 100;
                if (sprite)
                    spritePixelsPerUnit = sprite.pixelsPerUnit;

                float referencePixelsPerUnit = 100;
                if (canvas)
                    referencePixelsPerUnit = canvas.referencePixelsPerUnit;

                return spritePixelsPerUnit / referencePixelsPerUnit;
            }
        }


        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color + colorToAdd;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }


        /// <summary>
        /// UV rectangle used by the texture.
        /// 纹理使用的 UV 矩形。
        /// </summary>
        public Rect uvRect
        {
            get
            {
                return m_UVRect;
            }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Points to be drawn in the line.
        /// 线上要绘制的点。
        /// </summary>
        public Vector2[] Points
        {
            get
            {
                return m_points;
            }
            set
            {
                if (m_points == value)
                    return;
                m_points = value;
                SetAllDirty();
            }
        }

        #endregion

        #region Implementation
        Vector2 pos;
        List<Vector2> pointsTemp = new();
        public void GetAnimation()
        {
            pointsTemp = new(Points);
            for(int i =0; i < Points.Length; i++)
            {
                Points[i] = pointsTemp[0];
            }
            SetAllDirty();
        }
       
        public Tween DoAnimation(int i)
        {  
           Points[i] = pointsTemp[0];
        //    DOTween.To(() => Points[i], x => Points[i] = x, pointsTemp[i], 3f)
        //         .OnUpdate(
        //             () => {
        //                 SetAllDirty();
        //             }
        //         );
            float speed = UnityEngine.Random.Range(1.5f, 2.5f);
            return DOTween.To(() => Points[i], x => Points[i] = x, pointsTemp[i], speed).SetEase(Ease.InOutBounce)
            .OnUpdate(
                () => {
                    SetAllDirty();
                }).Pause();
        }
        public Tween UnDoAnimation(int i)
        {  
            float speed = UnityEngine.Random.Range(1.5f, 2.5f);
            return DOTween.To(() => Points[i], x => Points[i] = x, pointsTemp[Points.Length-1], speed).SetEase(Ease.InOutBounce)
            .OnUpdate(
                () => {
                    SetAllDirty();
                }).Pause();
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            
            if (m_points == null)
                return;
            Vector2[] pointsToDraw = m_points;
            // If Bezier is desired, pick the implementation
            // 如果需要贝塞尔曲线，请选择实现
            if (BezierMode != BezierType.None && m_points.Length > 3)
            {
                BezierPath bezierPath = new BezierPath();

                bezierPath.SetControlPoints(pointsToDraw);
                bezierPath.SegmentsPerCurve = BezierSegmentsPerCurve;
                List<Vector2> drawingPoints;
                switch (BezierMode)
                {
                    case BezierType.Basic:
                        drawingPoints = bezierPath.GetDrawingPoints0();
                        break;
                    case BezierType.Improved:
                        drawingPoints = bezierPath.GetDrawingPoints1();
                        break;
                    default:
                        drawingPoints = bezierPath.GetDrawingPoints2();
                        break;
                }
                pointsToDraw = drawingPoints.ToArray();
            }

            var sizeX = rectTransform.rect.width;
            var sizeY = rectTransform.rect.height;
            var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
            var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;

            // don't want to scale based on the size of the rect, so this is switchable now
            // 不想根据矩形的大小进行缩放，所以现在可以切换
            if (!relativeSize)
            {
                sizeX = 1;
                sizeY = 1;
            }

            if (UseMargins)
            {
                sizeX -= Margin.x;
                sizeY -= Margin.y;
                offsetX += Margin.x / 2f;
                offsetY += Margin.y / 2f;
            }

            vh.Clear();

            // Generate the quads that make up the wide line
            // 生成构成宽线的四边形
            var segments = new List<UIVertex[]>();
            if (LineList)
            {
                for (var i = 1; i < pointsToDraw.Length; i += 2)
                {
                    var start = pointsToDraw[i - 1];
                    var end = pointsToDraw[i];
                    start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
                    end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);

                    if (LineCaps)
                    {
                        segments.Add(CreateLineCap(start, end, SegmentType.Start));
                    }

                    segments.Add(CreateLineSegment(start, end, SegmentType.Middle));

                    if (LineCaps)
                    {
                        segments.Add(CreateLineCap(start, end, SegmentType.End));
                    }
                }
            }
            else
            {
                for (var i = 1; i < pointsToDraw.Length; i++)
                {
                    var start = pointsToDraw[i - 1];
                    var end = pointsToDraw[i];
                    start = new Vector2(start.x * sizeX + offsetX, start.y * sizeY + offsetY);
                    end = new Vector2(end.x * sizeX + offsetX, end.y * sizeY + offsetY);

                    if (LineCaps && i == 1)
                    {
                        segments.Add(CreateLineCap(start, end, SegmentType.Start));
                    }

                    segments.Add(CreateLineSegment(start, end, SegmentType.Middle));

                    if (LineCaps && i == pointsToDraw.Length - 1)
                    {
                        segments.Add(CreateLineCap(start, end, SegmentType.End));
                    }
                }
            }

            // Add the line segments to the vertex helper, creating any joins as needed
            // 将线段添加到顶点助手，根据需要创建任何连接
            for (var i = 0; i < segments.Count; i++)
            {
                if (!LineList && i < segments.Count - 1)
                {
                    var vec1 = segments[i][1].position - segments[i][2].position;
                    var vec2 = segments[i + 1][2].position - segments[i + 1][1].position;
                    var angle = Vector2.Angle(vec1, vec2) * Mathf.Deg2Rad;

                    // Positive sign means the line is turning in a 'clockwise' direction
                    // 正号表示线沿“顺时针”方向转动
                    var sign = Mathf.Sign(Vector3.Cross(vec1.normalized, vec2.normalized).z);

                    // Calculate the miter point
                    // 计算斜接点
                    var miterDistance = LineThickness / (2 * Mathf.Tan(angle / 2));
                    var miterPointA = segments[i][2].position - vec1.normalized * miterDistance * sign;
                    var miterPointB = segments[i][3].position + vec1.normalized * miterDistance * sign;

                    var joinType = LineJoins;
                    if (joinType == JoinType.Miter)
                    {
                        // Make sure we can make a miter join without too many artifacts.
                        // 确保我们可以在没有太多工件的情况下进行斜接连接。
                        if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_MITER_JOIN)
                        {
                            segments[i][2].position = miterPointA;
                            segments[i][3].position = miterPointB;
                            segments[i + 1][0].position = miterPointB;
                            segments[i + 1][1].position = miterPointA;
                        }
                        else
                        {
                            joinType = JoinType.Bevel;
                        }
                    }

                    if (joinType == JoinType.Bevel)
                    {
                        if (miterDistance < vec1.magnitude / 2 && miterDistance < vec2.magnitude / 2 && angle > MIN_BEVEL_NICE_JOIN)
                        {
                            if (sign < 0)
                            {
                                segments[i][2].position = miterPointA;
                                segments[i + 1][1].position = miterPointA;
                            }
                            else
                            {
                                segments[i][3].position = miterPointB;
                                segments[i + 1][0].position = miterPointB;
                            }
                        }

                        var join = new UIVertex[] { segments[i][2], segments[i][3], segments[i + 1][0], segments[i + 1][1] };
                        vh.AddUIVertexQuad(join);
                    }
                }
                vh.AddUIVertexQuad(segments[i]);
            }
        }

        private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, SegmentType type)
        {
            
            if (type == SegmentType.Start)
            {
                var capStart = start - ((end - start).normalized * LineThickness  / 2);
                return CreateLineSegment(capStart, start, SegmentType.Start);
            }
            else if (type == SegmentType.End)
            {
                var capEnd = end + ((end - start).normalized * LineThickness / 2);
                return CreateLineSegment(end, capEnd, SegmentType.End);
            }

            Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
            return null;
        }

        private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, SegmentType type)
        {
           
           

            var uvs = middleUvs;
            if (type == SegmentType.Start)
                uvs = startUvs;
            else if (type == SegmentType.End)
                uvs = endUvs;

            Vector2 offset = new Vector2(start.y - end.y, end.x - start.x).normalized * LineThickness / 2;
            var v1 = start - offset;
            var v2 = start + offset;
            var v3 = end + offset;
            var v4 = end - offset;
            return SetVbo(new[] { v1, v2, v3, v4 }, uvs);
        }

        #region ILayoutElement Interface

        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }

        public virtual float minWidth { get { return 0; } }

        public virtual float preferredWidth
        {
            get
            {
                if (overrideSprite == null)
                    return 0;
                return overrideSprite.rect.size.x / pixelsPerUnit;
            }
        }

        public virtual float flexibleWidth { get { return -1; } }

        public virtual float minHeight { get { return 0; } }

        public virtual float preferredHeight
        {
            get
            {
                if (overrideSprite == null)
                    return 0;
                return overrideSprite.rect.size.y / pixelsPerUnit;
            }
        }

        public virtual float flexibleHeight { get { return -1; } }

        public virtual int layoutPriority { get { return 0; } }

        #endregion

        #region ICanvasRaycastFilter Interface
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (m_EventAlphaThreshold >= 1)
                return true;

            Sprite sprite = overrideSprite;
            if (sprite == null)
                return true;

            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);

            Rect rect = GetPixelAdjustedRect();

            // Convert to have lower left corner as reference point.
            // 转换为以左下角为参考点。
            local.x += rectTransform.pivot.x * rect.width;
            local.y += rectTransform.pivot.y * rect.height;

            local = MapCoordinate(local, rect);

            // Normalize local coordinates.
            // 标准化局部坐标。
            Rect spriteRect = sprite.textureRect;
            Vector2 normalized = new Vector2(local.x / spriteRect.width, local.y / spriteRect.height);

            // Convert to texture space.
            // 转换为纹理空间。
            float x = Mathf.Lerp(spriteRect.x, spriteRect.xMax, normalized.x) / sprite.texture.width;
            float y = Mathf.Lerp(spriteRect.y, spriteRect.yMax, normalized.y) / sprite.texture.height;

            try
            {
                return sprite.texture.GetPixelBilinear(x, y).a >= m_EventAlphaThreshold;
            }
            catch (UnityException e)
            {
                Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + e.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        /// <summary>
        /// Return image adjusted position
        /// 返回图像调整位置
        /// **Copied from Unity's Image component for now and simplified for UI Extensions primatives
        /// **目前从 Unity Image 组件复制并针对 UI 扩展原语进行了简化
        /// </summary>
        /// <param name="local"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect spriteRect = sprite.rect;
            //if (type == Type.Simple || type == Type.Filled)
            return new Vector2(local.x * spriteRect.width / rect.width, local.y * spriteRect.height / rect.height);

            //Vector4 border = sprite.border;
            //Vector4 adjustedBorder = GetAdjustedBorders(border / pixelsPerUnit, rect);

            //for (int i = 0; i < 2; i++)
            //{
            //    if (local[i] <= adjustedBorder[i])
            //        continue;

            //    if (rect.size[i] - local[i] <= adjustedBorder[i + 2])
            //    {
            //        local[i] -= (rect.size[i] - spriteRect.size[i]);
            //        continue;
            //    }

            //    if (type == Type.Sliced)
            //    {
            //        float lerp = Mathf.InverseLerp(adjustedBorder[i], rect.size[i] - adjustedBorder[i + 2], local[i]);
            //        local[i] = Mathf.Lerp(border[i], spriteRect.size[i] - border[i + 2], lerp);
            //        continue;
            //    }
            //    else
            //    {
            //        local[i] -= adjustedBorder[i];
            //        local[i] = Mathf.Repeat(local[i], spriteRect.size[i] - border[i] - border[i + 2]);
            //        local[i] += border[i];
            //        continue;
            //    }
            //}

            //return local;
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // 如果矩形小于组合边框，则没有足够的空间容纳正常大小的边框。
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                // 为了避免边框重叠的伪影，我们缩小边框以适应。
                float combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    float borderScaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }

        #endregion

        #endregion

    }
}