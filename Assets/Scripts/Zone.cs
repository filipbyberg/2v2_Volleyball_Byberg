using UnityEngine;

public class Zone : MonoBehaviour
{
    public Position position;
    private bool isInImproperContact = false;

    private VolleyballEnvController envController;

    [SerializeField] private float improperContactEnterPenalty;
    [SerializeField] private float improperContactStayPenalty;

    private void Start(){
        envController = GetComponentInParent<VolleyballEnvController>();
    }
    // HANDLES FAULTY BEHAVIOUR
    private void HandleAgentContact(Collider other, float penalty)
    {
        VolleyballAgent agent = other.GetComponent<VolleyballAgent>();
        if(agent == null) return;

        if (agent.position != position)
        {
            if (position == Position.Mid){
                // No penalty
            }
            else {
                // Agent in faulty position
                agent.AddReward(penalty);
                if (penalty == improperContactEnterPenalty)
                {
                    isInImproperContact = true;
                } 
                else if (penalty == 0)
                {
                    isInImproperContact = false;
                }
            }
        }
        else {
            // Agent in correct position
            penalty = 0;
        }
    }
    private void HandleBallContact(){

    }
    
    // AGENT ENTER FAULTY ZONE
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ball"))
            envController.activeZone = (int)position;
        else
            HandleAgentContact(other, improperContactEnterPenalty);
    }

    // AGENT STAYS IN FAULTY ZONE
    private void OnTriggerStay(Collider other)
    {
        HandleAgentContact(other, improperContactStayPenalty);
    }
    /*
    private void OnTriggerExit(Collider other)
    {
        HandleAgentContact(other, 0f); // No penalty on exit
    }
    */
}
