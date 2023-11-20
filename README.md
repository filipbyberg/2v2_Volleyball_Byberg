# 🏐 Ultimate Volleyball
![Final_3_both (online-video-cutter com)](https://github.com/filipbyberg/2v2_Volleyball_Byberg/assets/80341025/ba19e55e-cf16-46ac-81ce-cf5da51a2a07)

## About
**Ultimate Volleyball** is a multi-agent reinforcement learning environment built on [Unity ML-Agents](https://unity.com/products/machine-learning-agents).


This projecct is based on Joy Zhangs "Ultimate Volleyball" project [https://github.com/CoderOneHQ/ultimate-volleyball]. Extending from a simple 1v1 volleyball game, to a 2v2 cooperative game with cooperative agents.

> **Version:** Up-to-date with ML-Agents Release 19
 

## Getting Started
1. Install the [Unity ML-Agents toolkit](https:github.com/Unity-Technologies/ml-agents) (Release 19+) by following the [installation instructions](https://github.com/Unity-Technologies/ml-agents/blob/release_18_docs/docs/Installation.md).
2. Download or clone this repo containing the Unity project.
3. Open the  project in Unity (Unity Hub → Projects → Add → Select root folder for this repo).
4. Load the `VolleyballMain` scene (Project panel → Assets → Scenes → `VolleyballMain.unity`).
5. Click the ▶ button at the top of the window. This will run the agent in inference mode using the provided baseline model.

## Training

1. If you previously changed Behavior Type to `Heuristic Only`, ensure that the Behavior Type is set back to `Default` (see [Heuristic Mode](#heuristic-mode)).
2. Activate the virtual environment containing your installation of `ml-agents`.
3. Make a copy of the [provided training config file](config/Volleyball.yaml) in a convenient working directory.
4. Run from the command line `mlagents-learn <path to config file> --run-id=<some_id> --time-scale=1`
    - Replace `<path to config file>` with the actual path to the file in Step 3
5. When you see the message "Start training by pressing the Play button in the Unity Editor", click ▶ within the Unity GUI.
6. From another terminal window, navigate to the same directory you ran Step 4 from, and run `tensorboard --logdir results` to observe the training process. 

For more detailed instructions, check the [ML-Agents getting started guide](https://github.com/Unity-Technologies/ml-agents/blob/release_18_docs/docs/Getting-Started.md).

## Self-Play
To enable self-play:
1. Set either Purple or Blue Agent Team ID to 1.
![Set Team ID](https://uploads-ssl.webflow.com/5ed1e873ef82ae197179be22/6131cc22959cd47d4b359382_selfplay.jpg)
2. Include the self-play hyperparameter hierarchy in your trainer config file, or use the provided file in `config/Volleyball_SelfPlay.yaml` ([ML-Agents Documentation](https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Learning-Environment-Design-Agents.md#teams-for-adversarial-scenarios))
3. Set your reward function in `ResolveEvent()` in `VolleyballEnvController.cs`.

## Environment Description
**Goal:** Get the agents to play in a cooperative manner, with passing, shooting and proper positioning behvaviour.

**Action space:**

4 discrete action branches:
- Forward motion (3 possible actions: forward, backward, no action)
- Rotation (3 possible actions: rotate left, rotate right, no action)
- Side motion (3 possible actions: left, right, no action)
- Jump (2 possible actions: jump, no action)

**Observation space:**

Total size: 13
- Agent Y-rotation (1)
- Normalised directional vector from agent to ball (3)
- Distance from agent to ball (1)
- Agent X, Y, Z velocity (3)
- Ball X, Y, Z relative velocity (3)
- Magnitude distance to center (1)
- Position Integer (1)

**Reward function:**

There are implemented multiple different rewards to play with, some of wich are shooting over the net, zones for positioning, jumping penalty to avoid excessive jumping, passing mechanism, shooting out of bounds, etc. Not all of them are active as default, but are easily enabled to play around with.

## Baselines
The following trained models are included in Assets/Models_all/Final:
- `Volleyball_Both.onnx` - Agents with shared policy to both pass and shoot
- `Volleyball_Shooting.onnx` - Shooting and proper positioning behaviour
- `Volleyball_Passing.onnx` - Passing and proper positioning behaviour
