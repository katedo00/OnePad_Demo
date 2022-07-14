using System;
using UnityEngine;
using UnityEngine.UI;

//This script responds to the AnimationEnded event and changes the OperationDirection
[RequireComponent(typeof(RotatingBillboardScript))]
public class AnimationCycler : MonoBehaviour
{
    //The label which shows the current animation
    public Text OperationDirectionLabel;
    //A reference to the RotatingBillboardScript for changing the current animation
    private RotatingBillboardScript rotatingBillboardScript;

    void Start()
    {
        //Get a reference to the RotatingBillboardScript on the current GameObject
        rotatingBillboardScript = GetComponent<RotatingBillboardScript>();
        //Update the current animation text on screen
        UpdateAnimationLabel();
    }

    //Message receiver
    public void AnimationEnded()
    {
        //Check if the current GameObject has got a RotatingBillboardScript
        if (rotatingBillboardScript != null)
        {
            //Select the next animation int based
            rotatingBillboardScript.OperationDirection++;
            //Check if the int value still is in the enum range
            if (!Enum.IsDefined(typeof(OperationDirection), rotatingBillboardScript.OperationDirection))
            {
                //If not start from the beginning
                rotatingBillboardScript.OperationDirection = 0;
            }
            //Update the current animation text on screen
            UpdateAnimationLabel();
        }
    }

    private void UpdateAnimationLabel()
    {
        //Check if a text object is selected in the script
        if (OperationDirectionLabel != null)
        {
            //Set the description of the current animation in the label
            OperationDirectionLabel.text = GetDescription(rotatingBillboardScript.OperationDirection);
        }
    }

    private string GetDescription(OperationDirection operationDirection)
    {
        //Get a description for each enum value
        switch (operationDirection)
        {
            case OperationDirection.AllAtOnce:
                return "All at once";
            case OperationDirection.InsideOut:
                return "Inside out";
            case OperationDirection.LeftToRight:
                return "Left to right";
            case OperationDirection.OutsideIn:
                return "Outside in";
            case OperationDirection.RightToLeft:
                return "Right to left";
            //If the operationDirection isn't one of the enum values return "Unknown" 
            default:
                return "Unknown";
        }
    }
}
