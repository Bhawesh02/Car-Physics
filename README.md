# Car Physics

This repository contains a Unity-based car simulation system, including a car controller and state management for both player and bot driving modes. The system is designed with modularity and flexibility, so it can be expanded based on any car project requirements.

---

## Features

### 1. **Car States**
- **INIT**: Initialization state for setup.
- **PLAYER_DRIVING**: State where the player controls the car using input axes.
- **BOT_DRIVING**: AI-controlled state where the car follows a predefined spline.
- **COUNT**: Represents the total number of states.

### 2. **Drive Types**
- **FORWARD_WHEEL_DRIVE**: Power is applied to the front wheels.
- **BACK_WHEEL_DRIVE**: Power is applied to the back wheels.

### 3. **Car Controller**
The main `CarController` class is responsible for:
- Switching car states.
- Applying physics for movement, acceleration, and braking.
- Interacting with the car's `Tyre` components for realistic suspension and steering.
- Supporting both player and bot driving modes.
- **Allows switching car data at runtime**, enabling different car behaviors as per requirements.

### 4. **Car Data**
The `CarData` class holds the configuration for different car behaviors, allowing for easy switching of car characteristics at runtime. You can define the car's type, suspension, engine performance, and other parameters to customize how the car behaves in the simulation. This feature allows dynamic adjustments of car behavior based on project needs, such as switching between different types of vehicles or modifying a car's characteristics during gameplay.

- **Car Type**: Defines the type of the car (e.g., `SPORTS_CAR`, `SUV`, `TRUCK`).
- **Drive Configuration**: Defines whether the car uses front or rear-wheel drive.
- **Suspension Settings**: Configures suspension force, damping, and rest length.
- **Engine Performance**: Sets maximum torque, speed, and acceleration rate.
- **Braking**: Defines braking force for deceleration.
- **Steering**: Specifies steering angle and speed.
- **Weight and Physics**: Configures mass and center of mass.

---

## Classes Overview

### `CarController`
Manages car physics, states, and behaviors. Key components:
- **Fields**:
  - `m_frontTyres`, `m_backTyres`: Arrays holding tyre components for front and rear wheels.
  - `m_carDatas`: List of car configurations (`CarData`).
  - `m_splineComputer`: Spline for bot navigation.
- **Methods**:
  - `InitCarStates()`: Initializes and registers car states in the state machine.
  - `SwitchCarData()`: Switches car configuration at runtime based on `CarDataType`, allowing dynamic behavior changes.
  - `AccelerateCar()`: Applies torque to wheels.
  - `ApplyBrakes()`: Applies braking forces to wheels.

---

### `Tyre`
Handles individual tyre physics, including suspension, braking, and torque application.  
- **Key Features**:
  - Suspension force calculation.
  - Steering and torque application.
  - Rotation simulation for visual realism.

---

### `CarState` (Abstract Class)
Defines the base structure for all car states. Implements the `IState` interface.

### `PlayerDrivingCarState`
Manages the player-controlled car behavior using Unity's input system.  
Key Features:
- Reads input axes (`Horizontal` and `Vertical`).
- Controls acceleration, braking, and tyre rotation.

### `BotDrivingCarState`
Manages bot-controlled car behavior using a spline path.  
Key Features:
- Follows a predefined `SplineComputer`.
- Adjusts tyre rotation and movement direction based on spline targets.

---

### `StateMachine`
A flexible state machine implementation for managing car states.  
Key Features:
- Supports transitions between states.
- Executes `OnEnter`, `OnUpdate`, `OnFixedUpdate`, and `OnExit` for each state.

---

## How It Works

1. **Initialization**:  
   - The `CarController` initializes with a specific state (`PLAYER_DRIVING` or `BOT_DRIVING`).
   - Configurations are loaded from `CarData`.

2. **Player Driving**:
   - Input controls (`WASD` or arrow keys) manage car movement.
   - Tyres rotate and apply torque based on input.

3. **Bot Driving**:
   - The bot car follows a spline path.
   - The bot adjusts steering and acceleration to reach target points on the spline.

4. **Suspension and Braking**:
   - Physics-based suspension adjusts tyre positions and applies forces dynamically.
   - Braking applies deceleration forces proportional to configured brake factors.

---

## Usage

1. Add the `CarController` script to your car GameObject.
2. Assign required components:
   - `Tyres` for front and rear wheels.
   - `Rigidbody` for physics.
   - `SplineComputer` for bot driving.
3. Configure `CarData` for different car behaviors.
4. Adjust inspector values like `Center of Mass Offset`, `Brake Factors`, and `Wheel Rotation Speed`.

---

## Debugging & Visualization
- **Gizmos**: Draw suspension forces, spline targets, and tyre velocities in the editor.
- **Unity Editor Handles**: Display directional forces for tyres in real-time.

---

## Future Improvements
- Add more car states (e.g., drifting, stunts).
- Implement dynamic weather effects.
- Integrate multiplayer driving.
- Extend `CarData` for more car types and advanced tuning options.

---

## Requirements
- Dreamteck Splines package for spline handling.

---
## Gameplay

### Player Driving
[![](https://github.com/Bhawesh02/Car-Physics/blob/main/Car_Physics/Assets/Extra/Car%20Physics%20Gameplay.gif)](https://youtube.com/shorts/r4XY97JedFg)

---

## Contributing
This tool represents my individual development efforts but thrives on community engagement. Feedback, suggestions, and collaborative contributions are highly encouraged. If you're passionate about mesh deformation techniques, Unity development workflows, or procedural generation, I’d love to connect!

---

## Contact

You can connect with me on LinkedIn: [Bhawesh Agarwal](https://www.linkedin.com/in/bhawesh-agarwal-70b98b113). Feel free to reach out if you're interested in discussing the game's mechanics, and development process, or if you simply want to talk about game design and development.
