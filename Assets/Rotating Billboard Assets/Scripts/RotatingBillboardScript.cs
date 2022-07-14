using System.Collections;
using System.Linq;
using UnityEngine;

public class RotatingBillboardScript : MonoBehaviour
{
    //The turn effect of the billboard parts
    public OperationDirection OperationDirection;
    //The speed at wich the billboard parts should turn
    public float Turnspeed = 70;
    //The time in seconds a advertisement is shown before turning to the next one
    public float WaitBeforeTurnTime = 5;
    //The time in seconds between parts when not using OperationDirection.AllAtOnce
    public float WaitTimeBetweenParts = 0.1f;
    //Texture of the first advertisement
    public Texture Advertisement1;
    //Texture of the second advertisement
    public Texture Advertisement2;
    //Texture of the third advertisement
    public Texture Advertisement3;
    //The transforms of the advertisement parts to turn
    public GameObject[] AdvertisementParts;
    //The amout of degrees the advertisement parts have turned sofar
    private float[] turned;
    //A boolean if we should turn at the moment or not
    private bool[] turn;
    //The index of the part that should start turning
    private int partIndex;
    //Backup texture of the first advertisement
    private Texture advertisement1;
    //Backup texture of the second advertisement
    private Texture advertisement2;
    //Backup texture of the third advertisement
    private Texture advertisement3;


    void Start()
    {
        //Only start if there are parts to turn
        if (AdvertisementParts.Length > 0)
        {
            turned = new float[AdvertisementParts.Length];
            turn = new bool[AdvertisementParts.Length];
            StartCoroutine(TurnToNextAdvertisement());
        }
    }

    IEnumerator TurnToNextAdvertisement()
    {
        //Wait for the setup amount of seconds
        yield return new WaitForSeconds(WaitBeforeTurnTime);
        //Reset the previous turn
        turned = new float[AdvertisementParts.Length];
        //Start turn
        StartTurn();
    }

    private void StartTurn()
    {
        switch (OperationDirection)
        {
            case OperationDirection.AllAtOnce:
                StartTurnAllAtOnce();
                break;
            case OperationDirection.LeftToRight:
                partIndex = 0;
                StartCoroutine(StartTurnLeftToRight());
                break;
            case OperationDirection.RightToLeft:
                partIndex = AdvertisementParts.Length - 1;
                StartCoroutine(StartTurnRightToLeft());
                break;
            case OperationDirection.InsideOut:
                partIndex = 0;
                StartCoroutine(StartTurnInsideOut());
                break;
            case OperationDirection.OutsideIn:
                partIndex = 0;
                StartCoroutine(StartTurnOutsideIn());
                break;
        }
    }

    //Turn all parts at once
    private void StartTurnAllAtOnce()
    {
        //Set all parts turning
        turn = turn.Select(v => true).ToArray();
    }

    //Turn all parts from left to right with a delay
    private IEnumerator StartTurnLeftToRight()
    {
        //Activate turning of one part
        turn[partIndex] = true;
        //Wait to start move the next one
        yield return new WaitForSeconds(WaitTimeBetweenParts);
        //Move to the next part index
        partIndex++;
        //When not all parts are moving
        if (partIndex < AdvertisementParts.Length)
        {
            //Start moving the next part
            yield return StartTurnLeftToRight();
        }
    }

    //Turn all parts from right to left with a delay
    private IEnumerator StartTurnRightToLeft()
    {
        //Activate turning of one part
        turn[partIndex] = true;
        //Wait to start move the next one
        yield return new WaitForSeconds(WaitTimeBetweenParts);
        //Move to the previous part index
        partIndex--;
        //When not all parts are moving
        if (partIndex >= 0)
        {
            //Start moving the previous part
            yield return StartTurnRightToLeft();
        }
    }

    //Turn all parts from the middle to the outside
    private IEnumerator StartTurnInsideOut()
    {
        //When an odd amount of parts is used add and substract 0.5 to get the 2 middle parts
        float deviation = 0;
        //Even ammount of parts? Start moving the middle 2 parts.
        if (AdvertisementParts.Length % 2 == 0)
        {
            deviation = 0.5f;
        }
        //Find the middle of a zero based array
        float middle = (AdvertisementParts.Length / 2f);
        int highPart = (int)(middle + deviation + partIndex);
        //An odd amount of parts need an extra run, be sure to check the boundaries
        if (highPart < AdvertisementParts.Length)
        {
            turn[highPart] = true;
        }
        int lowPart = (int)(middle - deviation - partIndex);
        //An odd amount of parts need an extra run, be sure to check the boundaries
        if (lowPart >= 0)
        {
            turn[lowPart] = true;
        }
        //Wait to start move the next parts
        yield return new WaitForSeconds(WaitTimeBetweenParts);
        partIndex++;
        //When less then half the parts are moving
        if (partIndex <= AdvertisementParts.Length / 2)
        {
            //Start moving the next parts
            yield return StartTurnInsideOut();
        }
    }

    //Turn all parts from the outside to the middle
    private IEnumerator StartTurnOutsideIn()
    {
        //Move the parts from the right in
        turn[AdvertisementParts.Length - 1 - partIndex] = true;
        //Move the parts from the left in
        turn[partIndex] = true;
        //Wait to start move the next parts
        yield return new WaitForSeconds(WaitTimeBetweenParts);
        partIndex++;
        //When less then half the parts are moving
        if (partIndex <= AdvertisementParts.Length / 2)
        {
            //Start moving the next parts
            yield return StartTurnOutsideIn();
        }
    }

    void Update()
    {
        //If parts need moving do so
        TryMoveParts();
        //If textures have changed set them on the materials
        TryRetextureParts();
    }

    private void TryMoveParts()
    {
        for (int partsIndex = 0; partsIndex < AdvertisementParts.Length; partsIndex++)
        {
            //When the turning should start
            if (turn[partsIndex])
            {
                //Calculate the rotation framerate independent
                float rotation = Turnspeed * Time.deltaTime;
                //Turn each part by a turn speed framerate independent
                AdvertisementParts[partsIndex].transform.localEulerAngles += Vector3.up * rotation;
                //Keep track of the total amount of degrees turned
                turned[partsIndex] += rotation;
                //Turn only 120 degrees each time
                if (turned[partsIndex] > 120)
                {
                    //Reset each part to the power of 120 degrees
                    AdvertisementParts[partsIndex].transform.localEulerAngles = new Vector3(AdvertisementParts[partsIndex].transform.localEulerAngles.x, (int)(AdvertisementParts[partsIndex].transform.localEulerAngles.y / 120) * 120, AdvertisementParts[partsIndex].transform.localEulerAngles.z);
                    //Stop turning when 120 degrees are met
                    turn[partsIndex] = false;
                    //When all parts have stop turning
                    if (turn.All(v => !v))
                    {
                        SendMessage("AnimationEnded", SendMessageOptions.DontRequireReceiver);
                        //Wait for the next turn sequence
                        StartCoroutine(TurnToNextAdvertisement());
                    }
                }
            }
        }
    }

    //This method changes the textures on the materials
    //when the texture properties of this script are changed during runtime
    private void TryRetextureParts()
    {
        TryRetexturePart(Advertisement2, ref advertisement2, 0);
        TryRetexturePart(Advertisement1, ref advertisement1, 1);
        TryRetexturePart(Advertisement3, ref advertisement3, 2);
    }

    private void TryRetexturePart(Texture advertisement, ref Texture backupAdvertisement, int position)
    {
        //If we have parts defined, the texture is defined and texture changed since last time
        if (AdvertisementParts.Length > 0 && advertisement != null && advertisement != backupAdvertisement)
        {
            //Set the new texture on all advertisement parts
            foreach (GameObject advertisementPart in AdvertisementParts)
            {
                MeshRenderer meshRenderer = advertisementPart.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    //Check if the amount of textures defined isn't larger then the amount of materials for the mesh
                    if (position < meshRenderer.materials.Length)
                    {
                        //Update the material texture
                        meshRenderer.materials[position].mainTexture = advertisement;
                        //Save the new texture as backup to possible detect future changes
                        backupAdvertisement = advertisement;
                    }
                }
            }
        }
    }
}
