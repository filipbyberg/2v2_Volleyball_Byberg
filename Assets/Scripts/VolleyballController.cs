using UnityEngine;

public class VolleyballController : MonoBehaviour
{
    [HideInInspector]
    public VolleyballEnvController envController;

    public GameObject purpleGoal;
    public GameObject blueGoal;
    Collider purpleGoalCollider;
    Collider blueGoalCollider;

    void Start()
    {
        envController = GetComponentInParent<VolleyballEnvController>();
        purpleGoalCollider = purpleGoal.GetComponent<Collider>();
        blueGoalCollider = blueGoal.GetComponent<Collider>();
    }

    /// <summary>
    /// Detects whether the ball lands in the blue, purple, or out of bounds area
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("boundary"))
        {
            // ball went out of bounds
            envController.ResolveEvent(Event.HitOutOfBounds);
        }
        else if (other.gameObject.CompareTag("blueBoundary"))
        {
            // ball hit into blue side
            envController.ResolveEvent(Event.HitIntoBlueArea);
        }
        else if (other.gameObject.CompareTag("purpleBoundary"))
        {
            // ball hit into purple side
            envController.ResolveEvent(Event.HitIntoPurpleArea);
        }
        else if (other.gameObject.CompareTag("purpleGoal"))
        {
            // ball hit purple goal (blue side court)
            envController.ResolveEvent(Event.HitPurpleGoal);
        }
        else if (other.gameObject.CompareTag("blueGoal"))
        {
            // ball hit blue goal (purple side court)
            envController.ResolveEvent(Event.HitBlueGoal);
        }
        
        else if (other.gameObject.CompareTag("purpleZoneLeft"))
        {
            // blue purple pass to purple left
            envController.ResolveEvent(Event.PurplePassRL);
        }
        else if (other.gameObject.CompareTag("purpleZoneRight"))
        {
            // purple left pass to purple right
            envController.ResolveEvent(Event.PurplePassLR);
        }
        else if (other.gameObject.CompareTag("blueZoneLeft"))
        {
            // blue right pass to blue left
            envController.ResolveEvent(Event.BluePassRL);
        }
        else if (other.gameObject.CompareTag("blueZoneRight"))
        {
            // blue left pass to blue right
            envController.ResolveEvent(Event.BluePassLR);
        }
        
    }


}