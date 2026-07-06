# Unity-MATLAB-UAV
A simulated UAV modelled in MATLAB Simulink flies in a virtual Unity world. Basic autonomy algorithms are implemeted including waypoint following combined with LIDAR informed obstacle avoidance.

# Pre-requesits
You will need the following applications installed to setup and run this repo. The versions referenced are the ones originally used but not necasserily a requirement:
- MATLAB R2025b. You will need the following addons:
  - Aerospace Blockset
  - Aerospace Toolbox
  - Requirements Toolbox
  - ROS Toolbox
  - Simulink
  - UAV Toolbox
- Unity 6.3 LTS - Editor version 6000.3.10f1
- ROS2 Humble Hawksbill

# Setup

Download and extract this repo to an easy to access location on your device.

## ROS
1. If using Windows OS you will need to install Windows Subsystem for Linux (WSL) in order to install and run ROS2. Follow this tutorial to install and setup WSL with Ubuntu 22.04: https://www.youtube.com/watch?v=PsGTlZ3SUGg  
(if using a different ROS distribution the Ubuntu distribution must be chosen accordingly. Corresponding Ubuntu/ROS distributions are detailed on the ROS website)

3. Follow the installation guide for ROS2 Humble: https://docs.ros.org/en/humble/Installation/Ubuntu-Install-Debs.html

4. Navigate to `home` -> `'username'` within your Ubuntu system and create a new folder called `ros2_ws` with a subfolder named `src`. This is your Colcon workspace which will be used to install ROS packages.

5. Now follow the installation guide for the setup of the ROS TCP Endpoint. Skip the docker setup step unless you specifically want to use this method. Ensure you follow the ROS2 section, not ROS1: https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/ros_unity_integration/setup.md

7. Run the ROS TCP Endpoint. You should see something like the following output in your Ubuntu terminal running the Endpoint:
   
   `[INFO] [1778595777.518194872] [UnityEndpoint]: Starting server on 172.28.87.13:10000`

To make setup easier for future it's advisable to source the ROS2 and workspace `setup.bash` files in your `.bashrc`. This will source ROS and your ROS workspace whenever you launch a new terminal:

1. In your Ubuntu system, navigate to `home/'username'` and open the `.bashrc` file. Append the following lines at the file:  
     
   ```bash 
   source /opt/ros/humble/setup.bash  
   cd /home/'username'/ros2_ws  
   source install/setup.bash
   ```  
## Unity
In Unity Hub, open the `Projects` tab, then `Add` -> `Add project from disk`. Open the `Unity Project` folder from the dowloaded repo.

Continue following the instructions in the ROS TCP Endpoint installation guide which will guide you through installing the ROS TCP Connector addon in Unity. Once You've completed the `Unity Setup` section you're good to go - dont worry about the `Install Unity Robotics Demo`, its not needed for this project.

Run the Unity project. If a sucessful connection to the the ROS network has been established you should see two blue arrows in the top left of the game window:

<img width="495" height="111" alt="image" src="https://github.com/user-attachments/assets/3127e9c6-68e2-43c6-8a5a-c2a1b07772ef" />


Ensure show HUD is enabled in ROS settings and that the game window scale is set to 1x, or this may not be visible.

7. The termial running the ROS TCP Endpoint should display something like the following:
   `[INFO] [1778595783.216133306] [UnityEndpoint]: Connection from 172.28.80.1`  
   `[INFO] [1778595783.358052132] [UnityEndpoint]: RegisterSubscriber(/cntrl, <class 'geometry_msgs.msg._point.Point'>) OK`  
   `etc`

separate camera required in scene to ttransmit as camera cannot render to texture (required for transmit) and render to unity display simulteaneously

## MATLAB
**IMPORTANT**: MATLAB must be installed either directly in your root `(C:)` or a folder with no whitespace characters (i.e. **not** `Program Files` or `Program Files x86`). This avoids issues wich can break ROS Toolbox Python scripts.

### ROS Setup:
In order for ROS Toolbox to function it must be linked to a valid python enterpreter (`3.9.x` or `3.10.x` for R2025b). If not already installed, download and install from the Python website: https://www.python.org/downloads/. Preferably install this in the same directory as your MATLAB installation.

1. Navigate to the `HOME` tab in MATLAB and open `Settings`:

    <img width="1575" height="151" alt="image" src="https://github.com/user-attachments/assets/96e327b3-569a-4ec9-9220-a363fbb81f5f" />

2. Open `ROS Toolbox` and insert the filepath to your Python enterpreter. Leave ROS Middleware (RMW) Implementation as `rmw_fastrtps_cpp`. Click `OK` once MATLAB has finished creating the venv:

   <img width="1081" height="777" alt="image" src="https://github.com/user-attachments/assets/57007d7a-7e14-4c81-9c93-f84cbf0e39f4" />

### Load Model:
In the MATLAB File Explorer, navigate to the location where you saved this repo. 
- Double click the `MATLAB Unity UAV.mlproj` file.
- Once the project has loaded, double click the `MATLAB Unity UAV.prj` file to open it.
- Under the `models` folder double click the `MasterModel` to load the Simulink model. 

# Running the System
Components must be run in the following order:
1. ROS TCP Endpoint
2. Usity Scene
3. MATLAB Simulink Model

if get NaN error from the Vector Field Histogram Block, restart the ROS TCP Endpoint
