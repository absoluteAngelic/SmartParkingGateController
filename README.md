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
| &cross; | Unity interactable visual simulation |
| &cross; | Testing (test table/screenshots, normal cases and include at least 1-2 edge/error cases) |

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
