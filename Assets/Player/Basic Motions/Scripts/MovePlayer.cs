using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    // скрипт не дработан. Нужно скорее всего создавать ещё один для создания префаба
    [field: SerializeField] private GameObject Player_PREFAB;
    [field: SerializeField] private float _speed = 0.5f;

    private Rigidbody2D _PlayerRb;

    private void Update() {
        _PlayerRb = GetComponent<Rigidbody2D>();
    }
}
