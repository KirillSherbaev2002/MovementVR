using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class ContinuousMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    public XRNode inputSource;
    private Vector2 inputAxis;
    private CharacterController character;

    private XROrigin rig;
    const float EARTH_GRAVITY = -9.81f;
    private float fallingSpeed;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float additionalHeigth;
    [SerializeField] private float rayToStayRadius;

    private void OnValidate()
    {
        if(additionalHeigth <= 0)
        {
            additionalHeigth = 0.2f;
        }
        if(rayToStayRadius <= 0)
        {
            rayToStayRadius = 0.01f;
        }
    }

    private void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }

    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();
        Movement();
        FallingDown();
    }

    private void Movement()
    {
        Quaternion headYaw = Quaternion.Euler(0, rig.Camera.gameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(direction * speed * Time.fixedDeltaTime);
    }

    private void FallingDown()
    {
        bool isGrounded = CheckIfGrounded();

        if (!isGrounded) fallingSpeed = 0;
        else fallingSpeed += EARTH_GRAVITY * Time.fixedDeltaTime;
        //To allow gravity correct falling only in case of not being on the ground. If Character is staying - falling and axeleration of it = zero

        character.Move(Vector3.up * EARTH_GRAVITY * Time.fixedDeltaTime);
    }

    private bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        //Needed to provide not just one ray shooting down in the middle of the player. But multiple rays. For example if one leg is still on the ground.
        //We just not falling down because of the single ray in the middle of the body.
        float rayLength = character.center.y + rayToStayRadius;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }

    private void CapsuleFollowHeadset()
    {
        character.height = rig.CameraInOriginSpaceHeight + additionalHeigth;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.Camera.gameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2, capsuleCenter.z);
    }
}
