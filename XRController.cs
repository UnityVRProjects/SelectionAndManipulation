#if ENABLE_VR || ENABLE_AR || PACKAGE_DOCS_GENERATION
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.SpatialTracking;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    [AddComponentMenu("XR/XR Controller (Device-based)", 11)]
    [HelpURL(XRHelpURLConstants.k_XRController)]
    public class XRController : XRBaseController
    {
        [SerializeField]
        XRNode m_ControllerNode = XRNode.RightHand;

        XRNode m_InputDeviceControllerNode;

        public XRNode controllerNode
        {
            get => m_ControllerNode;
            set => m_ControllerNode = value;
        }

        [SerializeField]
        InputHelpers.Button m_SelectUsage = InputHelpers.Button.Grip;
        public InputHelpers.Button selectUsage
        {
            get => m_SelectUsage;
            set => m_SelectUsage = value;
        }

        [SerializeField]
        InputHelpers.Button m_ActivateUsage = InputHelpers.Button.Trigger;
        public InputHelpers.Button activateUsage
        {
            get => m_ActivateUsage;
            set => m_ActivateUsage = value;
        }

        [SerializeField]
        InputHelpers.Button m_UIPressUsage = InputHelpers.Button.Trigger;
        public InputHelpers.Button uiPressUsage
        {
            get => m_UIPressUsage;
            set => m_UIPressUsage = value;
        }

        [SerializeField]
        float m_AxisToPressThreshold = 0.1f;
        public float axisToPressThreshold
        {
            get => m_AxisToPressThreshold;
            set => m_AxisToPressThreshold = value;
        }

        [SerializeField]
        InputHelpers.Button m_RotateAnchorLeft = InputHelpers.Button.PrimaryAxis2DLeft;
        public InputHelpers.Button rotateObjectLeft
        {
            get => m_RotateAnchorLeft;
            set => m_RotateAnchorLeft = value;
        }

        [SerializeField]
        InputHelpers.Button m_RotateAnchorRight = InputHelpers.Button.PrimaryAxis2DRight;
        public InputHelpers.Button rotateObjectRight
        {
            get => m_RotateAnchorRight;
            set => m_RotateAnchorRight = value;
        }

        [SerializeField]
        InputHelpers.Button m_MoveObjectIn = InputHelpers.Button.PrimaryAxis2DUp;
        public InputHelpers.Button moveObjectIn
        {
            get => m_MoveObjectIn;
            set => m_MoveObjectIn = value;
        }

        [SerializeField]
        InputHelpers.Button m_MoveObjectOut = InputHelpers.Button.PrimaryAxis2DDown;
        public InputHelpers.Button moveObjectOut
        {
            get => m_MoveObjectOut;
            set => m_MoveObjectOut = value;
        }

        [SerializeField]
        InputHelpers.Axis2D m_DirectionalAnchorRotation = InputHelpers.Axis2D.PrimaryAxis2D;
        public InputHelpers.Axis2D directionalAnchorRotation
        {
            get => m_DirectionalAnchorRotation;
            set => m_DirectionalAnchorRotation = value;
        }

#if ENABLE_VR || ENABLE_AR || PACKAGE_DOCS_GENERATION
        [SerializeField]
        BasePoseProvider m_PoseProvider;
        public BasePoseProvider poseProvider
        {
            get => m_PoseProvider;
            set => m_PoseProvider = value;
        }
#endif

        InputDevice m_InputDevice;
        public InputDevice inputDevice
        {
            get
            {
                if (m_InputDeviceControllerNode != m_ControllerNode || !m_InputDevice.isValid)
                {
                    m_InputDevice = InputDevices.GetDeviceAtXRNode(m_ControllerNode);
                    m_InputDeviceControllerNode = m_ControllerNode;
                }
                return m_InputDevice;
            }
        }

        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            base.UpdateTrackingInput(controllerState);
            if (controllerState == null)
                return;

            controllerState.isTracked = inputDevice.TryGetFeatureValue(CommonUsages.isTracked, out var isTracked) && isTracked;
            controllerState.inputTrackingState = InputTrackingState.None;

#if ENABLE_VR || ENABLE_AR
            if (m_PoseProvider != null)
            {
                var retFlags = m_PoseProvider.GetPoseFromProvider(out var poseProviderPose);
                if ((retFlags & PoseDataFlags.Position) != 0)
                {
                    controllerState.position = poseProviderPose.position;
                    controllerState.inputTrackingState |= InputTrackingState.Position;
                }
                if ((retFlags & PoseDataFlags.Rotation) != 0)
                {
                    controllerState.rotation = poseProviderPose.rotation * Quaternion.Euler(90f, 0f, 0f);
                    controllerState.inputTrackingState |= InputTrackingState.Rotation;
                }
            }
            else
#endif
            {
                if (inputDevice.TryGetFeatureValue(CommonUsages.trackingState, out var trackingState))
                {
                    controllerState.inputTrackingState = trackingState;

                    if ((trackingState & InputTrackingState.Position) != 0 &&
                        inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out var devicePosition))
                    {
                        controllerState.position = devicePosition;
                    }

                    if ((trackingState & InputTrackingState.Rotation) != 0 &&
                        inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out var deviceRotation))
                    {
                        // ✅ Fix the ray direction here: rotate so forward is Z+
                        var correctedRotation = deviceRotation * Quaternion.Euler(-90f, 0f, 0f);
                        controllerState.rotation = deviceRotation * Quaternion.Euler(90f, 0f, 0f);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            base.UpdateInput(controllerState);
            if (controllerState == null)
                return;

            controllerState.ResetFrameDependentStates();
            controllerState.selectInteractionState.SetFrameState(IsPressed(m_SelectUsage), ReadValue(m_SelectUsage));
            controllerState.activateInteractionState.SetFrameState(IsPressed(m_ActivateUsage), ReadValue(m_ActivateUsage));
            controllerState.uiPressInteractionState.SetFrameState(IsPressed(m_UIPressUsage), ReadValue(m_UIPressUsage));
        }

        protected virtual bool IsPressed(InputHelpers.Button button)
        {
            inputDevice.IsPressed(button, out var pressed, m_AxisToPressThreshold);
            return pressed;
        }

        protected virtual float ReadValue(InputHelpers.Button button)
        {
            inputDevice.TryReadSingleValue(button, out var value);
            return value;
        }

        public override bool SendHapticImpulse(float amplitude, float duration)
        {
            if (inputDevice.TryGetHapticCapabilities(out var capabilities) &&
                capabilities.supportsImpulse)
            {
                return inputDevice.SendHapticImpulse(0u, amplitude, duration);
            }
            return false;
        }
    }
}
