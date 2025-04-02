using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    public Transform holdPosition; // �������, ��� ����� ������������ ������ (��������, ���� ���������)
    private GameObject heldObject = null; // ������, ������� ������������
    private bool isHolding = false; // ����, �����������, ������������ �� ������
    private Vector3 offset; // �������� ������� ������������ holdPosition

    void Update()
    {
        // ���������, ���� �� ������� �� ������
        if (Input.touchCount > 0) // ���� ���� ���� �� ���� �������
        {
            Touch touch = Input.GetTouch(0); // �������� ������ �������

            // ������������ ������ ������ ������� (���� Began)
            if (touch.phase == TouchPhase.Began)
            {
                // ���� ������ �� ������������, �������� ������� ���
                if (!isHolding)
                {
                    TryPickUpObject(touch.position);
                }
                // ���� ������ ������������, ���������, ���� �� ������� �� ����
                else
                {
                    TryReleaseObject(touch.position);
                }
            }
        }

        // ���� ������ ������������, ���������� ��� � ������� holdPosition
        if (isHolding && heldObject != null)
        {
            MoveObjectToHoldPosition();
        }
    }

    // ����� ��� ������� ������� ������
    void TryPickUpObject(Vector2 touchPosition)
    {
        // ������� ��� �� ������ ����� ����� ������� �� ������
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // ���������, ����� �� ��� � �����-���� ������
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.Log("Raycast hit: " + hit.collider.name); // �������
            // ���������, ����� �� ������� ���� ������ (��������, �� ����)
            if (hit.collider.CompareTag("Pickable"))
            {
                Debug.Log("Picking up: " + hit.collider.name); // �������
                PickUp(hit.collider.gameObject);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything"); // �������
        }
    }

    // ����� ��� ������� ��������� ������
    void TryReleaseObject(Vector2 touchPosition)
    {
        // ������� ��� �� ������ ����� ����� ������� �� ������
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // ���������, ����� �� ��� � ������������ ������
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == heldObject)
            {
                Debug.Log("Releasing: " + heldObject.name); // �������
                ReleaseObject();
            }
        }
    }

    // ����� ��� �������� �������
    void PickUp(GameObject obj)
    {
        heldObject = obj;
        isHolding = true;

        // ��������� ������ � �������
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // ������ ������ ��������������
        }

        // ��������� �������� ������� ������������ holdPosition
        offset = heldObject.transform.position - holdPosition.position;

        // ����������� ������ � ������� holdPosition
        heldObject.transform.position = holdPosition.position + offset;
        heldObject.transform.rotation = holdPosition.rotation;
        heldObject.transform.SetParent(holdPosition); // ������ holdPosition ��������� �������
    }

    // ����� ��� ���������� �������
    void ReleaseObject()
    {
        if (heldObject != null)
        {
            // �������� ������ � �������
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // ���������� ������
            }

            // ���������� ������ �� holdPosition
            heldObject.transform.SetParent(null);
            heldObject = null;
            isHolding = false;
        }
    }

    // ����� ��� ����������� ������� � ������� holdPosition
    void MoveObjectToHoldPosition()
    {
        // ��������� ������� ������� � ������ ��������
        Vector3 targetPosition = holdPosition.position + offset;

        // ���������, ���� �� ����������� �� ����
        if (!CheckForCollisions(targetPosition))
        {
            // ���� �������� ���, ���������� ������
            heldObject.transform.position = targetPosition;
            heldObject.transform.rotation = holdPosition.rotation;
        }
    }

    // ����� ��� �������� �������� �� ���� � ������� �������
    bool CheckForCollisions(Vector3 targetPosition)
    {
        // ���������� OverlapSphere ��� �������� �������� � ������ ������� �������
        float radius = heldObject.GetComponent<Collider>().bounds.extents.magnitude; // ������ �������
        Collider[] colliders = Physics.OverlapSphere(targetPosition, radius);

        // ���������, ���� �� �������� � ������� ���������
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != heldObject && !collider.isTrigger)
            {
                Debug.Log("Collision detected with: " + collider.name); // �������
                return true;
            }
        }
        return false;
    }
}
