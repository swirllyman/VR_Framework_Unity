using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleInput;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class PlayerLocomotion : MonoBehaviour
{
	public bool allowMovementInput = false;
	public Transform groundCheck;
	public LoadingSphere loadSphere;
	public LayerMask groundMask;
	public float groundDistance = .4f;
	public float jumpHeight = 3.0f;
	public float spinSpeed = 180.0f;
	public float moveSpeed = 200.0f;


	CapsuleCollider myCollider;
	PlayerPlatform playerPlatform;
	Rigidbody myBody;
	Vector2 moveAxis;
	Vector3 rotateAxis;

	float currentMoveSpeed;
	bool isGrounded = false;
	bool teleporting = false;

    #region Monos

    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        playerPlatform = GetComponent<PlayerPlatform>();
        myCollider = GetComponent<CapsuleCollider>();
        currentMoveSpeed = moveSpeed;

        SimpleOpenXRInput.onPrimaryButtonUpdate += ToggleSprint;
        SimpleOpenXRInput.onSecondaryButtonUpdate += Jump;
        SimpleOpenXRInput.onJoystickUpdate += UpdateMovement;
		loadSphere.FadeOut(.5f, .5f);
    }

    void FixedUpdate()
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		myBody.drag = isGrounded ? 10.0f : .25f;
		Vector3 direction = new Vector3(moveAxis.x, 0, moveAxis.y);

		if (direction.magnitude >= .1f)
		{
			//direction
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerPlatform.head.eulerAngles.y;

			Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
			Vector3 moveVel = moveDir * currentMoveSpeed * Time.deltaTime;
			myBody.velocity = new Vector3(moveVel.x, myBody.velocity.y, moveVel.z);
		}

		myCollider.center = new Vector3(playerPlatform.head.localPosition.x, .8f, playerPlatform.head.localPosition.z);
		moveAxis = Vector2.zero;
	}

	private void LateUpdate()
	{
		if (Mathf.Abs(rotateAxis.x) > .15f)
		{
			transform.RotateAround(playerPlatform.head.position, Vector3.up, Time.deltaTime * spinSpeed * rotateAxis.x);
		}

		rotateAxis = Vector2.zero;
	}
    #endregion

    #region Teleport
    internal void Teleport(Pose pose, float fadeInTime, float waitTime, float fadeOutTime)
    {
		if (!teleporting)
			StartCoroutine(TeleportRoutine(pose, fadeInTime, waitTime, fadeOutTime));
    }

	IEnumerator TeleportRoutine(Pose pose, float fadeInTime, float waitTime, float fadeOutTime)
    {
		teleporting = true;
		loadSphere.FadeIn(fadeInTime);
		yield return new WaitForSeconds(fadeInTime);
		transform.position = pose.position;
		transform.rotation = pose.rotation;
		loadSphere.FadeOut(waitTime, fadeOutTime);
		teleporting = false;

	}
    #endregion

    #region Movement
    private void UpdateMovement(int side, Vector2 value)
	{
		if (!allowMovementInput) return;

		if (side == 0)
		{
			moveAxis = value;
		}
		else
		{
			rotateAxis = value;
		}
	}

	void ToggleSprint(int side, bool toggle)
    {
		if (!allowMovementInput) return;
		if (side == 1)
        {
			currentMoveSpeed = toggle ? moveSpeed * 2 : moveSpeed;
        }
    }

	void Jump(int side, bool down)
	{
		if (!allowMovementInput) return;

		if (side == 1 && isGrounded && down)
		{
			myBody.drag = .25f;
			myBody.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
		}
	}
    #endregion
}
