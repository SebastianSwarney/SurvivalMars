using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ObjectHolderEvent : UnityEvent { }

public class ObjectHolder : MonoBehaviour
{
	public ObjectHolderEvent m_onObjectPickupEvent;
	public ObjectHolderEvent m_onObjectDropEvent;

	public LayerMask m_objectPickupLayers;

	[HideInInspector]
	public PickupObject m_pickupObject;
	[HideInInspector]
	public PickupObject m_rotateObject;
	[HideInInspector]
	public bool m_holdingObject;

	public virtual void SelectObject(PickupObject p_newObject)
	{
		if (p_newObject.m_currentHolder != null)
		{
			p_newObject.m_currentHolder.DeselectObject();
		}

		p_newObject.m_rigidbody.useGravity = false;

		p_newObject.m_currentHolder = this;
		p_newObject.m_rigidbody.angularVelocity = Vector3.zero;

		m_pickupObject = p_newObject;
		m_holdingObject = true;

		m_onObjectPickupEvent.Invoke();
	}

	public virtual void DeselectObject()
	{
		m_pickupObject.m_rigidbody.useGravity = true;
		m_pickupObject.m_currentHolder = null;
		m_pickupObject = null;
		m_holdingObject = false;

		m_onObjectDropEvent.Invoke();
	}

	public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
	{
		if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void PhysicsSeekTo(Rigidbody p_targetRigidbody, Vector3 p_targetPosition)
	{
		Vector3 deltaPosition = p_targetPosition - p_targetRigidbody.transform.position;
		p_targetRigidbody.velocity = deltaPosition / Time.deltaTime;
	}

	public void PhysicsRotateTo(Rigidbody p_targetRigidbody, Quaternion p_targetRotation)
	{
		Vector3 axis;
		float angle;
		Quaternion deltaRotation = p_targetRotation * Quaternion.Inverse(p_targetRigidbody.rotation);
		deltaRotation.ToAngleAxis(out angle, out axis);
		if (angle > 180f) angle -= 360f;
		p_targetRigidbody.angularVelocity = axis * angle * Mathf.Deg2Rad / Time.deltaTime;
	}
}
