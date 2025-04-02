using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    public Transform holdPosition; // Позиция, где будет удерживаться объект (например, рука персонажа)
    private GameObject heldObject = null; // Объект, который удерживается
    private bool isHolding = false; // Флаг, указывающий, удерживается ли объект
    private Vector3 offset; // Смещение объекта относительно holdPosition

    void Update()
    {
        // Проверяем, было ли касание на экране
        if (Input.touchCount > 0) // Если есть хотя бы одно касание
        {
            Touch touch = Input.GetTouch(0); // Получаем первое касание

            // Обрабатываем только начало касания (фаза Began)
            if (touch.phase == TouchPhase.Began)
            {
                // Если объект не удерживается, пытаемся поднять его
                if (!isHolding)
                {
                    TryPickUpObject(touch.position);
                }
                // Если объект удерживается, проверяем, было ли нажатие на него
                else
                {
                    TryReleaseObject(touch.position);
                }
            }
        }

        // Если объект удерживается, перемещаем его в позицию holdPosition
        if (isHolding && heldObject != null)
        {
            MoveObjectToHoldPosition();
        }
    }

    // Метод для попытки поднять объект
    void TryPickUpObject(Vector2 touchPosition)
    {
        // Создаем луч от камеры через точку касания на экране
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // Проверяем, попал ли луч в какой-либо объект
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.Log("Raycast hit: " + hit.collider.name); // Отладка
            // Проверяем, можно ли поднять этот объект (например, по тегу)
            if (hit.collider.CompareTag("Pickable"))
            {
                Debug.Log("Picking up: " + hit.collider.name); // Отладка
                PickUp(hit.collider.gameObject);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything"); // Отладка
        }
    }

    // Метод для попытки отпустить объект
    void TryReleaseObject(Vector2 touchPosition)
    {
        // Создаем луч от камеры через точку касания на экране
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // Проверяем, попал ли луч в удерживаемый объект
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == heldObject)
            {
                Debug.Log("Releasing: " + heldObject.name); // Отладка
                ReleaseObject();
            }
        }
    }

    // Метод для поднятия объекта
    void PickUp(GameObject obj)
    {
        heldObject = obj;
        isHolding = true;

        // Отключаем физику у объекта
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Делаем объект кинематическим
        }

        // Вычисляем смещение объекта относительно holdPosition
        offset = heldObject.transform.position - holdPosition.position;

        // Привязываем объект к позиции holdPosition
        heldObject.transform.position = holdPosition.position + offset;
        heldObject.transform.rotation = holdPosition.rotation;
        heldObject.transform.SetParent(holdPosition); // Делаем holdPosition родителем объекта
    }

    // Метод для отпускания объекта
    void ReleaseObject()
    {
        if (heldObject != null)
        {
            // Включаем физику у объекта
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Возвращаем физику
            }

            // Отвязываем объект от holdPosition
            heldObject.transform.SetParent(null);
            heldObject = null;
            isHolding = false;
        }
    }

    // Метод для перемещения объекта в позицию holdPosition
    void MoveObjectToHoldPosition()
    {
        // Вычисляем целевую позицию с учетом смещения
        Vector3 targetPosition = holdPosition.position + offset;

        // Проверяем, есть ли препятствие на пути
        if (!CheckForCollisions(targetPosition))
        {
            // Если коллизий нет, перемещаем объект
            heldObject.transform.position = targetPosition;
            heldObject.transform.rotation = holdPosition.rotation;
        }
    }

    // Метод для проверки коллизий на пути к целевой позиции
    bool CheckForCollisions(Vector3 targetPosition)
    {
        // Используем OverlapSphere для проверки коллизий с учетом размера объекта
        float radius = heldObject.GetComponent<Collider>().bounds.extents.magnitude; // Радиус объекта
        Collider[] colliders = Physics.OverlapSphere(targetPosition, radius);

        // Проверяем, есть ли коллизии с другими объектами
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != heldObject && !collider.isTrigger)
            {
                Debug.Log("Collision detected with: " + collider.name); // Отладка
                return true;
            }
        }
        return false;
    }
}
