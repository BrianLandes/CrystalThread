﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Listens for if the user presses the 'Interact' button and attempts to handle it.
/// Maintains a list of one or more 'actuators' and gives the button event to the first one that uses it.
/// </summary>
public class PlayerInteractableController : MonoBehaviour {

	public string mainInteractionButtonName = "Interact";
	public string mainInteractionButtonLabel = "E: ";

	//public string secondaryInteractionButtonName = "Secondary Interact";
	//public string secondaryInteractionButtonLabel = "R: ";

	public UnitEssence myEssence;

	public Transform idealInteractionPosition;
	public float idealInteractionPositionTolerance = 0.2f;
	public float idealInteractionVelocityTolerance = 0.5f;

	public GameObject interactionUIIndicator;
	public Text interactionUILabel;

	public List<AbstractInteractableActuator> actuators = new List<AbstractInteractableActuator>();

	private bool movingIntoIdealPosition = false;
	private ActuatorTarget currentBestTarget;

	void Update () {
		if (movingIntoIdealPosition) {
			MoveIntoIdealPosition();
		}
		else {
			UpdateBestTarget();
			UpdateImmediateInteraction();

			if (Input.GetButtonDown(mainInteractionButtonName)) {
				StartInteractionMaybe();
			}
		}
	}
	
	private void StartInteractionMaybe() {
		
		// give any actuators a chance to immediately use the interaction before we look for nearby interactables
		foreach (var actuator in actuators) {
			if (actuator.UseInteractionEventImmediateMaybe()) {
				return;
			}
		}

		if (!currentBestTarget.IsNone()) {
			StartMoveIntoIdealPosition();
			//currentBestTarget.actuator.UseInteractionBestTargetEvent(currentBestTarget.target);
		}
	}

	private void UpdateBestTarget() {
		// look for nearby interactables and go with the best one (if any)
		currentBestTarget = ActuatorTarget.None();

		foreach (var actuator in actuators) {
			var bestTarget = actuator.GetBestTarget();
			if (bestTarget.IsNone()) {
				continue;
			}
			if (currentBestTarget.IsNone()) {
				currentBestTarget = bestTarget;
				continue;
			}
			if (currentBestTarget.distance > bestTarget.distance) {
				currentBestTarget = bestTarget;
				continue;
			}
		}

		if ( currentBestTarget.IsNone() ) {
			interactionUIIndicator.SetActive(false);
		} else {
			interactionUIIndicator.SetActive(true);
			interactionUILabel.text = mainInteractionButtonLabel + currentBestTarget.label;
		}
	}

	private void UpdateImmediateInteraction() {
		// check if any of the actuators are currently blocking the others
		foreach (var actuator in actuators) {
			if (actuator.IsImmediateInteraction()) {
				// this will override the best target label
				interactionUIIndicator.SetActive(true);
				interactionUILabel.text = mainInteractionButtonLabel + actuator.GetImmediateInteractionLabel();
				return;
			}
		}
	}

	private void StartMoveIntoIdealPosition() {
		movingIntoIdealPosition = true ;
		currentBestTarget.actuator.StartMoveIntoIdealPositionBestTargetEvent(currentBestTarget.target);
	}

	private void PerformInteractionOnBestTarget() {
		movingIntoIdealPosition = false;
		currentBestTarget.actuator.UseInteractionBestTargetEvent(currentBestTarget.target);
	}

	/// <summary>
	/// Take over control of the character and move them into position
	/// </summary>
	private void MoveIntoIdealPosition() {
		//bool moveIntoNextPhase = false;

		var currentVelocity = myEssence.GetCurrentVelocity();
		bool withinIdealVelocity = currentVelocity.sqrMagnitude < idealInteractionVelocityTolerance * idealInteractionVelocityTolerance;

		var vector = currentBestTarget.target.transform.position.JustXZ() - idealInteractionPosition.position.JustXZ();

		// if the distance from the target point to the pickup is less than a certain distance -> move into next phase
		float tolerance = idealInteractionPositionTolerance * idealInteractionPositionTolerance;
		bool withinIdealPosition = vector.sqrMagnitude < tolerance;
		if (withinIdealPosition) {
			myEssence.MoveVector = Vector3.zero;
		}
		else {
			myEssence.MoveVector = vector.FromXZ();
		}


		vector = currentBestTarget.target.transform.position.JustXZ() - transform.position.JustXZ();
		myEssence.TurnVector = vector.normalized;

		//// if the distance from us to the pickup is less than the distance to the ideal position -> move into next phase
		//var idealPositionRange = (idealInteractionPosition.position.JustXZ() - transform.position.JustXZ()).sqrMagnitude;
		//if (vector.sqrMagnitude < idealPositionRange) {
		//	moveIntoNextPhase = true;
		//}

		if (withinIdealVelocity && withinIdealPosition) {
			PerformInteractionOnBestTarget();
		}
	}

}

public struct ActuatorTarget {
	public MonoBehaviour target;
	public AbstractInteractableActuator actuator;
	public float distance;
	public string label;

	public ActuatorTarget(MonoBehaviour target, AbstractInteractableActuator actuator, float distance, string label) {
		this.target = target;
		this.actuator = actuator;
		this.distance = distance;
		this.label = label;
	}

	public static ActuatorTarget None() {
		return new ActuatorTarget {
			target = null,
		};
	}

	public bool IsNone() {
		return target == null;
	}
}

public abstract class AbstractInteractableActuator : MonoBehaviour {

	abstract public bool IsImmediateInteraction();

	abstract public string GetImmediateInteractionLabel();

	abstract public bool UseInteractionEventImmediateMaybe();

	abstract public ActuatorTarget GetBestTarget();

	abstract public void StartMoveIntoIdealPositionBestTargetEvent(MonoBehaviour target);

	abstract public void UseInteractionBestTargetEvent(MonoBehaviour target);
}