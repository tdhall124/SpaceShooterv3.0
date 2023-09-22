using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovement
{
    // MOVEMENT
    [SerializeField] private float _speed = 3.5f;
    //[SerializeField] private float _speedBoost = 2f;
    [SerializeField] private float _thrusterBoost = 1.5f;

    private bool _isSpeedBoostActive = false;
    public bool IsSpeedBoostActive
    {
        get { return _isSpeedBoostActive; }
        set { _isSpeedBoostActive = value; }
    }

    private int _thrusterLevelNormal = 0;
    private int _thrusterLevelMedium = 1;
    private int _thrusterLevelHigh = 2;

    void Update()
    {
        CalculateMovement();
    }

    public void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSpeedBoostActive)
        {
            _speed *= _thrusterBoost;
            UIManager.Instance.UpdateThrusterLevel(_thrusterLevelMedium);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed /= _thrusterBoost;
            UIManager.Instance.UpdateThrusterLevel(_thrusterLevelNormal);
        }

        transform.Translate(_speed * Time.deltaTime * direction);
        // this position adjustment does nothing until the object reaches -4.66f
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.66f, 0), 0);

        if(transform.position.y < -3.7f)
            UIManager.Instance.HideResourcePanel();
        else
            UIManager.Instance.ShowResourcesPanel(); // what a shame

        if (transform.position.x > 9f)
        {
            transform.position = new Vector3(-9f, transform.position.y, 0);
        }
        else if (transform.position.x < -9f)
        {
            transform.position = new Vector3(9f, transform.position.y, 0);
        }
    }

    public void SetBoost(float boost)
    {
        _speed *= boost;
        _isSpeedBoostActive = true;
    }

    public void RelieveBoost(float boost)
    {
        _speed /= boost;
        _isSpeedBoostActive = false;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
