using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public enum WaterState
{
    Liquid,
    Solid,
    Gas
}

public class WaterController : MonoBehaviour
{
    [Header("State Objects")]
    [SerializeField] private GameObject liquidState;
    [SerializeField] private GameObject solidState;
    [SerializeField] private GameObject gasState;

    [Header("Transition")]
    [SerializeField] private float transitionTime = 0.3f;

    private ControllerInputs controls;

    private WaterState currentState;
    private bool isTransitioning;

    private void Awake()
    {
        controls = new ControllerInputs();

        controls.Player.ChangeToLiquid.performed += _ =>
        {
            if (!isTransitioning)
                StartCoroutine(TransitionToState(WaterState.Liquid));
        };

        controls.Player.ChangeToSolid.performed += _ =>
        {
            if (!isTransitioning)
                StartCoroutine(TransitionToState(WaterState.Solid));
        };

        controls.Player.ChangeToGas.performed += _ =>
        {
            if (!isTransitioning)
                StartCoroutine(TransitionToState(WaterState.Gas));
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        ActivateOnly(liquidState);

        currentState = WaterState.Liquid;
    }

    private IEnumerator TransitionToState(WaterState targetState)
    {
        if (currentState == targetState)
            yield break;

        isTransitioning = true;

        Vector3 position = GetCurrentPosition();

        yield return new WaitForSeconds(0.2f);

        // mover SOLO EL PADRE (ROOT)
        transform.position = position;

        ActivateOnly(targetState);

        currentState = targetState;

        isTransitioning = false;
    }

    private void ActivateOnly(WaterState state)
    {
        liquidState.SetActive(state == WaterState.Liquid);
        solidState.SetActive(state == WaterState.Solid);
        gasState.SetActive(state == WaterState.Gas);
    }

    private void ActivateOnly(GameObject activeObject)
    {
        liquidState.SetActive(false);
        solidState.SetActive(false);
        gasState.SetActive(false);

        activeObject.SetActive(true);
    }

    private Vector3 GetCurrentPosition()
    {
        switch (currentState)
        {
            case WaterState.Liquid:
                return CalculateLiquidCenter();

            case WaterState.Solid:
                return transform.position;

            case WaterState.Gas:
                return transform.position;
        }

        return transform.position;
    }

    private Vector3 CalculateLiquidCenter()
    {
        Rigidbody2D[] particles =
            liquidState.GetComponentsInChildren<Rigidbody2D>();

        if (particles.Length == 0)
            return transform.position;

        Vector2 center = Vector2.zero;

        foreach (Rigidbody2D rb in particles)
        {
            center += rb.position;
        }

        center /= particles.Length;

        return center;
    }
}
