using System.Collections;
using UnityEditor;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Lerp Horizontal Layout Group", 150)]
    public class LerpHorizontalLayout : HorizontalLayoutGroup
    {
        protected LerpHorizontalLayout()
        { }

        #region PublicVariables
        public float lerpTime { get { return m_LerpTime; } set { SetProperty(ref m_LerpTime, value); } }
        public float lerpSpeed { get { return m_LerpSpeed; } set { SetProperty(ref m_LerpSpeed, value); } }
        #endregion

        #region PrivateVariables
        [SerializeField] protected float m_LerpTime = 1f;
        [SerializeField] protected float m_LerpSpeed = 10f;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            lerpTime = lerpTime;
            lerpSpeed = lerpSpeed;
        }

#endif
        #endregion

        #region PublicMethod
        public override void SetLayoutHorizontal()
        {
            StopAllCoroutines();
            SetChildrenAlongAxisCustom(0, false);
        }
        #endregion

        #region PrivateMethod
        protected void SetChildrenAlongAxisCustom(int axis, bool isVertical)
        {
            float size = rectTransform.rect.size[axis];
            bool controlSize = (axis == 0 ? m_ChildControlWidth : m_ChildControlHeight);
            bool useScale = (axis == 0 ? m_ChildScaleWidth : m_ChildScaleHeight);
            bool childForceExpandSize = (axis == 0 ? m_ChildForceExpandWidth : m_ChildForceExpandHeight);
            float alignmentOnAxis = GetAlignmentOnAxis(axis);

            bool alongOtherAxis = (isVertical ^ (axis == 1));
            int startIndex = m_ReverseArrangement ? rectChildren.Count - 1 : 0;
            int endIndex = m_ReverseArrangement ? 0 : rectChildren.Count;
            int increment = m_ReverseArrangement ? -1 : 1;
            if (alongOtherAxis)
            {
                float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);

                for (int i = startIndex; m_ReverseArrangement ? i >= endIndex : i < endIndex; i += increment)
                {
                    RectTransform child = rectChildren[i];
                    float min, preferred, flexible;
                    GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);
                    float scaleFactor = useScale ? child.localScale[axis] : 1f;

                    float requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
                    float startOffset = GetStartOffset(axis, requiredSpace * scaleFactor);
                    if (controlSize)
                    {
                        SetChildAlongAxisWithScaleCustom(child, axis, startOffset, requiredSpace, scaleFactor);
                    }
                    else
                    {
                        float offsetInCell = (requiredSpace - child.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxisWithScaleCustom(child, axis, startOffset + offsetInCell, scaleFactor);
                    }
                }
            }
            else
            {
                float pos = (axis == 0 ? padding.left : padding.top);
                float itemFlexibleMultiplier = 0;
                float surplusSpace = size - GetTotalPreferredSize(axis);

                if (surplusSpace > 0)
                {
                    if (GetTotalFlexibleSize(axis) == 0)
                        pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - (axis == 0 ? padding.horizontal : padding.vertical));
                    else if (GetTotalFlexibleSize(axis) > 0)
                        itemFlexibleMultiplier = surplusSpace / GetTotalFlexibleSize(axis);
                }

                float minMaxLerp = 0;
                if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
                    minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

                for (int i = startIndex; m_ReverseArrangement ? i >= endIndex : i < endIndex; i += increment)
                {
                    RectTransform child = rectChildren[i];
                    float min, preferred, flexible;
                    GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);
                    float scaleFactor = useScale ? child.localScale[axis] : 1f;

                    float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                    childSize += flexible * itemFlexibleMultiplier;
                    if (controlSize)
                    {
                        SetChildAlongAxisWithScaleCustom(child, axis, pos, childSize, scaleFactor);
                    }
                    else
                    {
                        float offsetInCell = (childSize - child.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxisWithScaleCustom(child, axis, pos + offsetInCell, scaleFactor);
                    }
                    pos += childSize * scaleFactor + spacing;
                }
            }
        }

        private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand,
                out float min, out float preferred, out float flexible)
        {
            if (!controlSize)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }

            if (childForceExpand)
                flexible = Mathf.Max(flexible, 1);
        }

        protected void SetChildAlongAxisWithScaleCustom(RectTransform rect, int axis, float pos, float size, float scaleFactor)
        {
            if (rect == null)
                return;

            m_Tracker.Add(this, rect,
                DrivenTransformProperties.Anchors |
                (axis == 0 ?
                    (DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.SizeDeltaX) :
                    (DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.SizeDeltaY)
                )
            );

            // Inlined rect.SetInsetAndSizeFromParentEdge(...) and refactored code in order to multiply desired size by scaleFactor.
            // sizeDelta must stay the same but the size used in the calculation of the position must be scaled by the scaleFactor.

            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;

            Vector2 sizeDelta = rect.sizeDelta;
            sizeDelta[axis] = size;
            rect.sizeDelta = sizeDelta;

            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition[axis] = (axis == 0) ? (pos + size * rect.pivot[axis] * scaleFactor) : (-pos - size * (1f - rect.pivot[axis]) * scaleFactor);
            //rect.anchoredPosition = anchoredPosition;
            StartCoroutine(SetChildPositionWithLerp(rect, anchoredPosition, m_LerpTime, m_LerpSpeed));
        }

        protected void SetChildAlongAxisWithScaleCustom(RectTransform rect, int axis, float pos, float scaleFactor)
        {
            if (rect == null)
                return;

            m_Tracker.Add(this, rect,
                DrivenTransformProperties.Anchors |
                (axis == 0 ? DrivenTransformProperties.AnchoredPositionX : DrivenTransformProperties.AnchoredPositionY));

            // Inlined rect.SetInsetAndSizeFromParentEdge(...) and refactored code in order to multiply desired size by scaleFactor.
            // sizeDelta must stay the same but the size used in the calculation of the position must be scaled by the scaleFactor.

            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;

            Vector2 anchoredPosition = rect.anchoredPosition;
            anchoredPosition[axis] = (axis == 0) ? (pos + rect.sizeDelta[axis] * rect.pivot[axis] * scaleFactor) : (-pos - rect.sizeDelta[axis] * (1f - rect.pivot[axis]) * scaleFactor);
            //rect.anchoredPosition = anchoredPosition;
            StartCoroutine(SetChildPositionWithLerp(rect, anchoredPosition, m_LerpTime, m_LerpSpeed));
        }

        private IEnumerator SetChildPositionWithLerp(RectTransform rect, Vector2 targetPosition, float lerpTime, float lerpSpeed)
        {
            float pastTime = 0f;

            while (pastTime <= lerpTime)
            {
                if (rect != null) rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPosition, Time.unscaledDeltaTime * lerpSpeed);
                LayoutRebuilder.MarkLayoutForRebuild(rect);
                pastTime += Time.unscaledDeltaTime;
                yield return null;
            }

            if (rect != null) rect.anchoredPosition = targetPosition;
            LayoutRebuilder.MarkLayoutForRebuild(rect);
        }
        #endregion
    }
}
