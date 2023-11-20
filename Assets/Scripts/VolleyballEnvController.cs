using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Blue = 0,
    Purple = 1,
    Default = 2
}

public enum Event
{
    HitPurpleGoal = 0,
    HitBlueGoal = 1,
    HitOutOfBounds = 2,
    HitIntoBlueArea = 3,
    HitIntoPurpleArea = 4,
    BluePassLR = 5,
    BluePassRL = 6,
    PurplePassLR = 7,
    PurplePassRL = 8
}

public enum Position{
    Left =0,
    Right =1,
    Mid =2
}

public class VolleyballEnvController : MonoBehaviour
{
    int ballSpawnSide;
    public GameObject FloatingTextPrefabReward;
    public GameObject FloatingTextPrefabPenalty;

    VolleyballSettings volleyballSettings;

    public VolleyballAgent blueAgent;
    public VolleyballAgent purpleAgent;

    public List<VolleyballAgent> AgentsList = new List<VolleyballAgent>();
    List<Renderer> RenderersList = new List<Renderer>();

    Rigidbody blueAgentRb;
    Rigidbody purpleAgentRb;

    public GameObject ball;
    Rigidbody ballRb;

    public GameObject blueGoal;
    public GameObject purpleGoal;

    Renderer blueGoalRenderer;

    Renderer purpleGoalRenderer;

    Team lastHitter;

    private int resetTimer;
    public int MaxEnvironmentSteps;

    #region 2vs2
    public int lastHitterId =0;
    public int activeZone =0;
    public List<VolleyballAgent> purpleAgents = new List<VolleyballAgent>();
    public List<VolleyballAgent> blueAgents = new List<VolleyballAgent>();
    #endregion


    void Start()
    {

        // Used to control agent & ball starting positions
        blueAgentRb = blueAgent.GetComponent<Rigidbody>();
        purpleAgentRb = purpleAgent.GetComponent<Rigidbody>();
        ballRb = ball.GetComponent<Rigidbody>();

        // Starting ball spawn side
        // -1 = spawn blue side, 1 = spawn purple side
        var spawnSideList = new List<int> { -1, 1 };
        ballSpawnSide = spawnSideList[Random.Range(0, 2)];

        // Render ground to visualise which agent scored
        blueGoalRenderer = blueGoal.GetComponent<Renderer>();
        purpleGoalRenderer = purpleGoal.GetComponent<Renderer>();
        RenderersList.Add(blueGoalRenderer);
        RenderersList.Add(purpleGoalRenderer);

        volleyballSettings = FindObjectOfType<VolleyballSettings>();

        ResetScene();
    }

    /// <summary>
    /// Tracks which agent last had control of the ball
    /// </summary>
    public void UpdateLastHitter(Team team,int id)
    {
        lastHitter = team;
        lastHitterId = id;

    }

    /// <summary>
    /// Resolves scenarios when ball enters a trigger and assigns rewards.
    /// Example reward functions are shown below.
    /// To enable Self-Play: Set either Purple or Blue Agent's Team ID to 1.
    /// </summary>
    public void ResolveEvent(Event triggerEvent)
    {
        switch (triggerEvent)
        {
            // REWARD TRIGGER WHEN BALL OOB
            case Event.HitOutOfBounds:
                if (lastHitter == Team.Blue)
                {
                    // apply penalty to blue agent
                    
                    ProcessReward(new List<VolleyballAgent>(){blueAgents[lastHitterId]},purpleAgents,-2f,0);
                    
                    if (lastHitterId == 0)
                    {
                    
                        ShowFloatingText_Blue_Penalty(0);
                    }
                    else ShowFloatingText_Blue_Penalty(1);
                
                    
                }
                else if (lastHitter == Team.Purple)
                {
                    // apply penalty to purple agent
                    ProcessReward(new List<VolleyballAgent>(){purpleAgents[lastHitterId]},blueAgents,-2f,0);
                    if (lastHitterId == 0)
                    {
                    
                        ShowFloatingText_Purple_Penalty(0);
                    }
                    else ShowFloatingText_Purple_Penalty(1);
                }

                // end episode
                TerminateEpisode();
                ResetScene();
                break;

            // REWARD TRIGGER WHEN HIT BLUE GOAL
            case Event.HitBlueGoal:
                // Blue wins                

                if (lastHitter != Team.Blue)
                {
                    ProcessReward(new List<VolleyballAgent>(){blueAgents[lastHitterId]},purpleAgents,-1f,0);
                    ShowFloatingText_Purple_Penalty(lastHitterId);
                }
                
                // turn floor blue
                StartCoroutine(GoalScoredSwapGroundMaterial(volleyballSettings.blueGoalMaterial, RenderersList, .5f));
                TerminateEpisode();
                ResetScene();
                break;

            // REWARD TRIGGER WHEN HIT PURP GOAL
            case Event.HitPurpleGoal:
                // purple wins

                if (lastHitter != Team.Purple)
                {
                    ProcessReward(new List<VolleyballAgent>(){purpleAgents[lastHitterId]},blueAgents,-1f,0);
                    ShowFloatingText_Purple_Penalty(lastHitterId);
                }
                
                // turn floor purple
                StartCoroutine(GoalScoredSwapGroundMaterial(volleyballSettings.purpleGoalMaterial, RenderersList, .5f));
                TerminateEpisode();
                ResetScene();
                break;

            case Event.HitIntoBlueArea:
                if (lastHitter == Team.Purple)
                {

                    if (lastHitterId == 0)
                    {
                        ProcessReward(new List<VolleyballAgent>(){purpleAgents[lastHitterId]},blueAgents,1f,0);
                        ShowFloatingText_Purple_Reward(0);
                    }
                    else; // ShowFloatingText_Purple_Reward(1);
                    
                    
                }
                break;

            case Event.HitIntoPurpleArea:
                if (lastHitter == Team.Blue)
                {

                    if (lastHitterId == 0)
                    {
                        ProcessReward(new List<VolleyballAgent>(){blueAgents[lastHitterId]},purpleAgents,1f,0);
                        ShowFloatingText_Blue_Reward(0);
                    }
                    else; //ShowFloatingText_Blue_Reward(1);
                    
                }
                break;

            case Event.BluePassLR:
                if (lastHitter == Team.Purple)
                {
                    if (lastHitterId == 0)
                    {
                        // Blue left pass to blue right
                        ProcessReward(new List<VolleyballAgent>(){purpleAgents[lastHitterId]},blueAgents,1f,0);
                        //Debug.Log("Blue LR");
                        //ShowFloatingText_Blue_Reward(0);
                    }
                }
                /*
                if (lastHitter == Team.Blue)
                {
                    if (lastHitterId == 0)
                    {
                        // Blue left pass to blue right
                        //blueAgents[0].AddReward(1f);
                        //ProcessReward(new List<VolleyballAgent>(){blueAgents[lastHitterId]},purpleAgents,1f,0);
                        //Debug.Log("Blue LR");
                        //ShowFloatingText_Blue_Reward(0);
                    }
                }
                */
                break;

            case Event.BluePassRL:
                if (lastHitter == Team.Blue)
                {
                    if (lastHitterId == 1)
                    {
                        // Blue right pass to blue left
                        ProcessReward(new List<VolleyballAgent>(){blueAgents[lastHitterId]},purpleAgents,1f,0);
                        ShowFloatingText_Blue_Reward(1);
                        //Debug.Log("Blue RL");
                    }
                    
                }
                break;

            case Event.PurplePassLR:
                if (lastHitter == Team.Blue)
                {
                    if (lastHitterId == 0)
                    {
                        // Blue left pass to blue right
                        ProcessReward(new List<VolleyballAgent>(){blueAgents[lastHitterId]},purpleAgents,1f,0);
                        //Debug.Log("Blue LR");
                        //ShowFloatingText_Blue_Reward(0);
                    }
                }
                /*
                if (lastHitter == Team.Purple)
                {
                    if (lastHitterId == 0)
                    {
                        // purple left pass to purple right
                        //purpleAgents[0].AddReward(1f);
                        //ProcessReward(new List<VolleyballAgent>(){purpleAgents[lastHitterId]},blueAgents,1f,0);
                        //ShowFloatingText_Purple_Reward(0);
                        //Debug.Log("Purple LR");
                    }
                    
                }
                */
                break;

            case Event.PurplePassRL:
                if (lastHitter == Team.Purple)
                {
                    if (lastHitterId == 1)
                    {
                        // Purple right pass to purple left
                        ProcessReward(new List<VolleyballAgent>(){purpleAgents[lastHitterId]},blueAgents,1f,0);
                        ShowFloatingText_Purple_Reward(1);
                        //Debug.Log("Purple RL");
                    }
                    
                }
                break;

        }
    }

    /// <summary>
    
    /// SHOWFLOAT: Initiate animation of points shown above agents head
    /// PROCESSREWARD: Applies reward to agents
    /// GOAL SWAP MATERIAL: Changes the color of the ground for a moment.
    /// TERMINATE EPISODE: Ends episode
    /// </summary>
    /// <returns>The Enumerator to be used in a Coroutine.</returns>
    /// <param name="mat">The material to be swapped.</param>
    /// <param name="time">The time the material will remain.</param>
    /// 

    void ShowFloatingText_Purple_Reward(int player_hit) {
        var go1 = Instantiate(FloatingTextPrefabReward, purpleAgents[player_hit].transform.position, Quaternion.identity, purpleAgents[player_hit].transform);
        go1.GetComponent<TextMesh>().text = "+1";
        go1.transform.position += new Vector3(-1, 2, -1); // Remove the dot (.) after Vector3
    }

    void ShowFloatingText_Blue_Reward(int player_hit) {
        var go2 = Instantiate(FloatingTextPrefabReward, blueAgents[player_hit].transform.position, Quaternion.identity, blueAgents[player_hit].transform);
        go2.GetComponent<TextMesh>().text = "+1";
        go2.transform.position += new Vector3(-1, 2, -1); // Remove the dot (.) after Vector3
    }

    void ShowFloatingText_Purple_Penalty(int player_hit) {
        var go1 = Instantiate(FloatingTextPrefabPenalty, purpleAgents[player_hit].transform.position, Quaternion.identity, purpleAgents[player_hit].transform);
        go1.GetComponent<TextMesh>().text = "-0.5";
        go1.transform.position += new Vector3(-1, 2, -1); // Remove the dot (.) after Vector3
    }

    void ShowFloatingText_Blue_Penalty(int player_hit) {
        var go2 = Instantiate(FloatingTextPrefabPenalty, blueAgents[player_hit].transform.position, Quaternion.identity, blueAgents[player_hit].transform);
        go2.GetComponent<TextMesh>().text = "-0.5";
        go2.transform.position += new Vector3(-1, 2, -1); // Remove the dot (.) after Vector3
    }
    private void ProcessReward(List<VolleyballAgent> toBeRewarded,List<VolleyballAgent> toBePunished, float reward, float punishment){
        for( int i=0;i<toBeRewarded.Count;i++)
            toBeRewarded[i].AddReward(reward);
        for( int i=0;i<toBePunished.Count;i++)
            toBePunished[i].AddReward(punishment);
    }

    private void ProcessRewardWithZone(List<VolleyballAgent> toBeRewarded,List<VolleyballAgent> toBePunished, float reward, float punishment){
        for( int i=0;i<toBeRewarded.Count;i++){
            toBeRewarded[i].AddReward(reward);}
        for( int i=0;i<toBePunished.Count;i++){
            if((int)toBePunished[i].position == activeZone){
                toBePunished[i].AddReward(punishment);
            }
        }
    }
    private void TerminateEpisode(){
        foreach(VolleyballAgent agent in blueAgents)
            agent.EndEpisode();
        foreach(VolleyballAgent agent in purpleAgents)
            agent.EndEpisode();
            
    }
    IEnumerator GoalScoredSwapGroundMaterial(Material mat, List<Renderer> rendererList, float time)
    {
        foreach (var renderer in rendererList)
        {
            renderer.material = mat;
        }

        yield return new WaitForSeconds(time); // wait for 2 sec

        foreach (var renderer in rendererList)
        {
            renderer.material = volleyballSettings.defaultMaterial;
        }

    }

    /// <summary>
    /// Called every step. Control max env steps.
    /// </summary>
    void FixedUpdate()
    {
        resetTimer += 1;
        if (resetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            blueAgent.EpisodeInterrupted();
            purpleAgent.EpisodeInterrupted();
            ResetScene();
        }
    }

    /// <summary>
    /// Reset agent and ball spawn conditions.
    /// </summary>
    public void ResetScene()
    {
        resetTimer = 0;
        lastHitter = Team.Default; // reset last hitter
        int random = 2; //Random.Range(1,3);

        for( int i=0;i<purpleAgents.Count;i++){
            PositionAgent(purpleAgents[i], false, (Position)(int)((i+random)%2));
        }
        for( int i=0;i<blueAgents.Count;i++){
           PositionAgent(blueAgents[i], true, (Position)(int)((i+random)%2));
        }
        //Debug.Log("Reset Agents");

        ResetBall();
    }



    /// <summary>
    /// Reset agent transform based on it's position{Left,Right,Front,Back}
    /// </summary>
    private void PositionAgent(VolleyballAgent agent, bool blue, Position position){
        int zFactor =0;
        int xFactor = 0;

        if(position == Position.Left || position == Position.Right)
            xFactor = 3;
        else
            zFactor = 3;

        int teamFactor = blue? 1 : -1;
        int zPositionFactor = -1;
        int xPositionFactor = position ==Position.Left ? -1:1;

        agent.position = position;

        var randomPosX = Random.Range(-2f, 2f) + (xFactor * xPositionFactor *teamFactor);
        var randomPosZ = Random.Range(-2f, 2f) + (zFactor* zPositionFactor * teamFactor);
        var randomPosY = 0.5f; //Random.Range(0.5f, 3.75f); // depends on jump height
        var randomRot = Random.Range(-45f, 45f);

        agent.transform.localPosition = new Vector3(randomPosX, randomPosY, randomPosZ);
        agent.transform.eulerAngles = new Vector3(0, randomRot, 0);
        agent.GetComponent<Rigidbody>().velocity = default;
    }


    /// <summary>
    /// Reset ball spawn conditions
    /// </summary>
    void ResetBall()
    {
        // INCREASED RANDOMNESS
        //var randomPosX = Random.Range(-4f, 4f);
        //var randomPosZ = Random.Range(4f, 12f);
        //var randomPosY = Random.Range(8f, 10f);
        
        var randomPosX = (Random.Range(0, 2) == 0) ? -3 : 3;
        var randomPosZ = Random.Range(6f, 10f);
        var randomPosY = 10f;

        // alternate ball spawn side
        // -1 = spawn blue side, 1 = spawn purple side
        ballSpawnSide = -1 * ballSpawnSide;

        if (ballSpawnSide == -1)
        {
            ball.transform.localPosition = new Vector3(randomPosX, randomPosY, randomPosZ);
        }
        else if (ballSpawnSide == 1)
        {
            ball.transform.localPosition = new Vector3(randomPosX, randomPosY, -1 * randomPosZ);
        }

        ballRb.angularVelocity = Vector3.zero;
        ballRb.velocity = Vector3.zero;
    }
}
