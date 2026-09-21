using System.Collections.Generic;
using UnityEngine;

public class RotateChildrenAround : MonoBehaviour
{
    [Header("Orbit")]
    public Transform orbitCenter;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Settings")]
    [SerializeField] private bool rotateClockwise = true;

    protected Stack<Transform> childrenStack = new Stack<Transform>();

    private float currentAngle;

    protected virtual void Awake()
    {
        CacheChildren();
    }

    protected virtual void OnEnable()
    {
        CacheChildren();

        currentAngle = 0f;

        ActivateChildren();
        ArrangeChildren();
    }

    protected virtual void Update()
    {
        RotateChildren();
    }

    protected void CacheChildren()
    {
        childrenStack.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            childrenStack.Push(child);
        }
    }

    protected void ActivateChildren()
    {
        foreach (Transform child in childrenStack)
        {
            if (child != null)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    protected void ArrangeChildren()
    {
        if (childrenStack.Count == 0)
            return;

        if (orbitCenter == null)
            return;

        float angleStep =
            360f / childrenStack.Count;

        int index = 0;

        foreach (Transform child in childrenStack)
        {
            if (child == null)
                continue;

            float angle =
                currentAngle +
                angleStep * index;

            SetChildPosition(
                child,
                angle
            );

            index++;
        }
    }

    protected void RotateChildren()
    {
        if (childrenStack.Count == 0)
            return;

        if (orbitCenter == null)
            return;

        float direction =
            rotateClockwise ? -1f : 1f;

        currentAngle +=
            rotationSpeed *
            direction *
            Time.deltaTime;

        float angleStep =
            360f / childrenStack.Count;

        int index = 0;

        foreach (Transform child in childrenStack)
        {
            if (child == null)
                continue;

            if (!child.gameObject.activeSelf)
                continue;

            float angle =
                currentAngle +
                angleStep * index;

            SetChildPosition(
                child,
                angle
            );

            index++;
        }
    }

    protected void SetChildPosition(
        Transform child,
        float angle
    )
    {
        if (child == null || orbitCenter == null)
            return;

        float radians =
            angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radians),
            Mathf.Sin(radians),
            0f
        ) * radius;

        child.position =
            orbitCenter.position + offset;
    }

    protected Transform PopFirstActiveChild()
    {
        if (childrenStack.Count == 0)
            return null;

        return childrenStack.Pop();
    }

    protected void PushChild(Transform child)
    {
        if (child == null)
            return;

        child.SetParent(transform);

        if (!childrenStack.Contains(child))
        {
            childrenStack.Push(child);
        }
    }

    protected bool HasActiveChildren()
    {
        foreach (Transform child in childrenStack)
        {
            if (child != null &&
                child.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    protected virtual void OnDisable()
    {
        foreach (Transform child in childrenStack)
        {
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    protected Transform GetOrbitCenter()
    {
        return orbitCenter;
    }

    protected float GetRadius()
    {
        return radius;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (orbitCenter == null)
            return;

        Gizmos.DrawWireSphere(
            orbitCenter.position,
            radius
        );
    }
}