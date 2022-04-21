using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandsPresence : MonoBehaviour
{
    [SerializeField] private InputDeviceCharacteristics controllerDeviceCharacteristics;
    [SerializeField] private List<GameObject> controllerPrefabs;

    [SerializeField] private bool showController = false;

    private InputDevice targetDevice;
    private GameObject spawnedController;
    public GameObject handModelPrefab;
    private GameObject spawnedHandModel;

    private Animator handAnimator;

    void Start()
    {
        FindAllControllers();
    }

    private void FindAllControllers()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerDeviceCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
                spawnedController = Instantiate(prefab, transform);
            else
                spawnedController = Instantiate(controllerPrefabs[0], transform);

            spawnedHandModel = Instantiate(handModelPrefab, transform);

            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    }

    void UpdateAnimation()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            handAnimator.SetFloat("Trigger", triggerValue);
        else
            handAnimator.SetFloat("Trigger", 0);

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            handAnimator.SetFloat("Grip", gripValue);
        else
            handAnimator.SetFloat("Grip", 0);
    }

    // Update is called once per frame
    void Update()
    {
        CheckActivityOfControllers();
    }

    void CheckActivityOfControllers()
    {
        if (!targetDevice.isValid)
        {
            FindAllControllers();
        }
        else
        {
            ShowHandsVisualization();
        }
    }

    //private void ControllerInputUpdate()
    //{
    //    if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
    //        print("Primary button pressed");

    //    if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.7f)
    //        print("Shot");

    //    if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue) && primary2DAxisValue != Vector2.zero)
    //        print("Move");
    //}

    void ShowHandsVisualization()
    {
        if (showController)
        {
            spawnedHandModel.SetActive(false);
            spawnedController.SetActive(true);
        }
        else
        {
            spawnedHandModel.SetActive(true);
            spawnedController.SetActive(false);
            UpdateAnimation();
        }
    }
}
