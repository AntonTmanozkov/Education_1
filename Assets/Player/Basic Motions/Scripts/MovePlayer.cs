using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovePlayer : MonoBehaviour
{
    [field: SerializeField] private float _speed = 0.5f;
    [field: SerializeField] private float _jumpForce = 300f;

    private Rigidbody _playerRb;
    private bool _isGrounded;

    private void Awake() 
    {
        _playerRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() 
    {
        MovementLogic();
        JumpLogic();
    }

    private void MovementLogic()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        _playerRb.AddForce(movement * _speed);
    }

    private void JumpLogic()
    {
        if (Input.GetAxis("Jump") > 0)
        {
            if (_isGrounded)
            {
                _playerRb.AddForce(Vector3.up * _jumpForce);
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        IsGroundedUpdate(other, true);
    }

    private void OnCollisionExit(Collision other) {
        IsGroundedUpdate(other, false);
    }

    private void IsGroundedUpdate(Collision collision, bool value)
    {
        if (collision.gameObject.tag == "Ground") 
        {
            _isGrounded = value;
        } 
        
    }

    // public void GetPlayer(GameObject Player) => _player = Player;
}
