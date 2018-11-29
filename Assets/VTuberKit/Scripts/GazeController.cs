using Live2D.Cubism.Core;
using UnityEngine;

namespace nkjzm.VTuberKit
{
    /// <summary>
    /// 目線の追従を行うクラス
    /// </summary>
    public class GazeController : MonoBehaviour
    {
        [SerializeField]
        Transform Anchor = null;
        Vector3 centerOnScreen;
        void Start()
        {
            centerOnScreen = Camera.main.WorldToScreenPoint(Anchor.position);
        }
        void LateUpdate()
        {
            var mousePos = Input.mousePosition - centerOnScreen;
            UpdateRotate(new Vector3(mousePos.x, mousePos.y, 0) * 0.2f);
        }

        Vector3 currentRotateion = Vector3.zero;
        Vector3 eulerVelocity = Vector3.zero;

        [SerializeField]
        CubismParameter HeadAngleX = null, HeadAngleY = null, HeadAngleZ = null;
        [SerializeField]
        CubismParameter EyeBallX = null, EyeBallY = null;
        [SerializeField]
        float EaseTime = 0.2f;
        [SerializeField]
        float EyeBallXRate = 0.05f;
        [SerializeField]
        float EyeBallYRate = 0.02f;
        [SerializeField]
        bool ReversedGazing = false;
        void UpdateRotate(Vector3 targetEulerAngle)
        {
            currentRotateion = Vector3.SmoothDamp(currentRotateion, targetEulerAngle, ref eulerVelocity, EaseTime);
            // 頭の角度
            SetParameter(HeadAngleX, currentRotateion.x);
            SetParameter(HeadAngleY, currentRotateion.y);
            SetParameter(HeadAngleZ, currentRotateion.z);
            // 眼球の向き
            SetParameter(EyeBallX, currentRotateion.x * EyeBallXRate * (ReversedGazing ? -1 : 1));
            SetParameter(EyeBallY, currentRotateion.y * EyeBallYRate * (ReversedGazing ? -1 : 1));
        }

        void SetParameter(CubismParameter parameter, float value)
        {
            if (parameter != null)
            {
                parameter.Value = Mathf.Clamp(value, parameter.MinimumValue, parameter.MaximumValue);
            }
        }
    }
}