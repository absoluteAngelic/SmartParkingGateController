# SmartParkingGateController

## Overview

The Smart Parking Gate Controller is a finite state machine designed to control the entrance gate of a parking lot. The controller decides whether a vehicle can enter based on whether parking spaces are available and whether the driver's ticket is valid.

If entry is approved, the controller opens the gate, updates the available parking count, waits for the vehicle to pass, and then closes the gate. The controller also handles invalid tickets, full parking lots, and emergency situations through a dedicated Failsafe state.

The controller was designed as a **Mealy Finite State Machine** and implemented as a working digital logic simulation using [lgsim.io](https://lgsim.io), as well as a Unity interactable visual representation.

---

## Team Members

- Luke
- Azfar
- Yusuf

---

## Project Option

### Option 2 - Smart Parking Gate Controller

> Design a parking-lot gate controller. The controller should decide whether to allow a car to enter based on available spaces and simple ticket/payment status. It should open/close the gate, update the available-space count, and handle simple cases such as a full parking lot or invalid input.

Our implementation includes:

- Ticket detection
- Ticket validation
- Parking-space availability checking
- Automatic gate opening and closing
- Gate position sensors
- Gate-open timer
- Available-space decrement signal
- Full-lot warning
- Invalid-ticket warning
- Emergency Failsafe mode
- Manual reset from Failsafe

---

## Current Progress

| Done | Deliverable |
|------|-------------|
| &check; | Github with starter Readme and Morteza added |
| &check; | State Table with states, inputs, outputs, and transitions |
| &check; | State Diagram (Mealy Finite State Machine) |
| &check; | Expand Readme with short overview, team members, our option, how to run it, and simple controller explanation |
| &check; | Working Controller simulation/implementation |
| &check; | Unity interactable visual simulation |
| &check; | Testing (test table/screenshots, normal cases and include at least 1-2 edge/error cases) |

---

## How the Controller Works

The controller uses six states:

1. **Gate Closed + Waiting for Input**  
   The controller waits for a vehicle to enter a ticket.

2. **Checking Info**  
   The controller checks whether parking spaces are available and whether the ticket is valid.

3. **Opening Gate**  
   If the ticket is valid and a parking space is available, the gate motor begins opening the gate.

4. **Gate Open and Waiting**  
   Once the gate is fully open, a timer begins. The controller keeps the gate open long enough for the vehicle to enter.

5. **Closing Gate**  
   When the timer finishes, the gate motor closes the gate. Once the closed-gate sensor activates, the controller returns to its initial state.

6. **Failsafe**  
   If an emergency occurs in any normal state, the controller enters Failsafe. The gate is opened if necessary and remains in Failsafe until the emergency is cleared and the controller is reset.

### Priority Rules

The controller uses the following priorities when multiple conditions are true:

```text
SpotsAvailable=0 > ValidTicket=0
Emergency=1 > *
Reset=1 > * only in Failsafe when Emergency=0
```
## State Table & All Possible States
<img width="1210" height="517" alt="image" src="https://github.com/user-attachments/assets/3600435b-77a0-4a4e-bded-8c66975948ef" />

[Link to Google Sheet](https://docs.google.com/spreadsheets/d/1RsZu3KsnwQQAp51Ip9har2o-DtIEsxkhf73vqLOv-rM/edit?usp=sharing)

## Mealy Finite State Machine:
<img width="1645" height="1129" alt="image" src="https://github.com/user-attachments/assets/b141e3d4-0669-4125-9381-f01ffdd2ba34" />

## Both Versions of Controller Simulation (using lgsim.io)
<img width="1205" height="702" alt="image" src="https://github.com/user-attachments/assets/9270779f-af9e-414c-877d-89980a829998" />
<img width="1353" height="657" alt="image" src="https://github.com/user-attachments/assets/be365791-5e82-45aa-b734-1adfdc821f2e" />


To see our Controller Simulation please:

- **Download our Smart_Parking_Gate_Controller.lgs file from our repo** 
- **Go to [lgsim.io](lgsim.io)**
- **Click File in the top left**
- **Click Import Workspace**
- **Select the .lgs file and then you can see the full Simulation**

## Unity visual project
<img width="1248" height="634" alt="image" src="https://github.com/user-attachments/assets/c5efe04c-90a3-4532-8652-7606393babf9" />

The red rectangles are cars, the yellow rectangle is the gate. Everything moves/is animated. Comma starts an emergency, period stops it, backspace is reset, enter inserts/submits the ticket number that's set in the unity inspector.

To run/view this project, open unity hub, press add, add project from disk, select the subfolder in this repo, then you'll be shown which version of the unity editor must be installed. You can install it, then open the project, and run it by pressing the triangle at the top of the unity editor window.



## Test Table
<img width="887" height="565" alt="image" src="https://github.com/user-attachments/assets/984728ef-6e57-4bd9-932b-2df6478bb589" />

[Link to Google Sheet](https://docs.google.com/spreadsheets/d/1RsZu3KsnwQQAp51Ip9har2o-DtIEsxkhf73vqLOv-rM/edit?usp=sharing)

- The Yellow is Normal
- The Red is Edge/ Error
- The Green is technically Normal? (It's a recovery test so I'm not 100% sure the classification)

## Breakdown of Work
All the work was evenly distributed with the exception(s) of:
- Luke taking the lead on the Unity Visual Simulation
- Azfar focusing on the Circuit Simulations on lgsim
- and Yusuf focusing on troubleshooting our logic
