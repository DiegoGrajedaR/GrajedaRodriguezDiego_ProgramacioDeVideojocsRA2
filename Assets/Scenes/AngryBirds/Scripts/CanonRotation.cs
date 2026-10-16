using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanonRotation : MonoBehaviour, InputSystem_Actions.ICanonActions
{
    private InputSystem_Actions inputAction;
    public float _maxRotation = 60;
    public float _minRotation = -60;
    private float _offset = -51.6f;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    [SerializeField] private float _multiplier = 10f;


    private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;
        inputAction = new InputSystem_Actions();
        inputAction.Canon.SetCallbacks(this);
    }

    private void OnEnable() => inputAction.Enable();
    private void OnDisable() => inputAction.Disable();


    void Update()
    {
        if (isRaising)
        {
            ProjectileSpeed = Time.deltaTime * _multiplier + ProjectileSpeed; //acotar entre dos valors (mirar variables)
            CalculateBarScale();
        }
        
        CalculateBarScale();
    }

    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRaising = true;
        }
        if (context.canceled)
        {
            var projectile = Instantiate(Bullet, ShootPoint.transform.position, Quaternion.identity); //canviar la posició on s'instancia
            Rigidbody2D rbProjectile = projectile.GetComponent<Rigidbody2D>();
            rbProjectile.linearVelocity = ShootPoint.transform.right * ProjectileSpeed;
            ProjectileSpeed = 0f;
            isRaising = false;
            Destroy(projectile, 3f);
        }
    }

    public void OnChangeMouse(InputAction.CallbackContext context)
    {
        Vector2 screenPos = context.ReadValue<Vector2>();
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(screenPos); //obtenir el valor del click del cursor (Fer amb new input system)
        _distanceBetweenMouseAndPlayer = (Vector2)mousePos - (Vector2)transform.position; //Vector2.zero; obtenir el vector distància entre el canó i el cursor

        var ang = (Mathf.Atan2(_distanceBetweenMouseAndPlayer.y, _distanceBetweenMouseAndPlayer.x)) * Mathf.Rad2Deg;   //* 180f / Mathf.PI + offset);
        //ang = Mathf.Clamp(ang, _minRotation, _maxRotation);
		transform.rotation = Quaternion.Euler(0, 0, ang); //en quin dels tres eixos va l'angle?
	}
}
