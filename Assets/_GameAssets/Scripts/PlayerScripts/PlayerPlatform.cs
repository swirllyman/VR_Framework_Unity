using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatform : MonoBehaviour
{
	public static PlayerPlatform singleton;
	public static int trackingFrames = 5;

	public Transform playerRigTransform;
	public Transform head, leftHand, rightHand;
	public MyHand[] vrHands;
	internal PlayerLocomotion locomotion;
	internal PlayerInteractionUI uiInteraction;
	internal bool useFixedTime = true;

	void Awake()
	{
		singleton = this;
		locomotion = GetComponent<PlayerLocomotion>();
		uiInteraction = GetComponent<PlayerInteractionUI>();
		uiInteraction.enabled = false;
	}

	bool slowed = false;
	[ContextMenu("Toggle Time Scale")]
	void TestTimeScale()
	{
		slowed = !slowed;
		Time.timeScale = slowed ? .1f : 1.0f;
	}

	public void SetupVelocityTracking(int frameCount)
	{
		trackingFrames = frameCount;
		foreach (MyHand h in vrHands)
		{
			h.SetupHand(trackingFrames);
		}
	}

	private void Update()
	{
		if (!useFixedTime || Time.timeScale != 1.0f)
		{
			foreach (MyHand h in vrHands)
			{
				h.UpdateHand(Time.unscaledDeltaTime);
			}
		}
	}

	private void FixedUpdate()
	{
		if (useFixedTime && Time.timeScale == 1.0f)
		{
			foreach (MyHand h in vrHands)
			{
				h.UpdateHand(Time.fixedUnscaledDeltaTime);
			}
		}
	}

	public static void LoadSphere(float fadeTime, bool fadeIn)
	{
		if (fadeIn)
		{
			singleton.locomotion.loadSphere.FadeIn(fadeTime);
		}
		else
		{
			singleton.locomotion.loadSphere.FadeOut(0.0f, fadeTime);
		}
	}

	public static void Teleport(Pose pose, float fadeInTime = .25f, float waitTime = .5f, float fadeOutTime = .25f)
	{
		singleton.locomotion.Teleport(pose, fadeInTime, waitTime, fadeOutTime);
	}

	public static void ToggleUiInteraction(bool toggle)
	{
		singleton.uiInteraction.enabled = toggle;
	}
}

[System.Serializable]
public struct Pose
{
	public Vector3 position;
	public Quaternion rotation;
}

[System.Serializable]
public class MyHand
{
	public Transform transform;

	//Player Velocity Settings
	int currentFrame = 0;
	Vector3 velocityThisFrame;
	Vector3[] velocitySet;
	Vector3 prevPosFixed;

	public void SetupHand(int frameCount)
	{
		velocitySet = new Vector3[frameCount];
		currentFrame = 0;
	}

	/// <summary>
	/// Updates Hand based on Delta Time if being called from Update or Fixed Delta Time if called from FixedUpdate
	/// </summary>
	/// <param name="timeType"></param>
	public void UpdateHand(float timeType)
	{
		velocityThisFrame = (transform.position - prevPosFixed) / timeType;
		velocitySet[currentFrame] = velocityThisFrame;
		currentFrame = (currentFrame + 1) % velocitySet.Length;
		prevPosFixed = transform.position;
	}

	public Vector3 GetWorldVelocity()
	{
		Vector3 avgVel = Vector3.zero;
		foreach (Vector3 v in velocitySet)
		{
			avgVel += v;
		}
		return avgVel / velocitySet.Length;
	}
}