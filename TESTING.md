# Smart Parking Gate Controller — Testing Guide

This guide is for someone opening `Smart_Parking_Gate_Controller.lgs` and verifying that the controller follows the state table.

The circuit uses **latched inputs** and three D flip-flops for the state memory. A state change occurs when the **CLOCK receives a rising edge**.

## 1. Input Map

The switches on the **far left side** are the inputs.

From **top to bottom**, they are:

| Position | Input | Meaning |
|---|---|---|
| 1 | `TicketEntered` | 1 when a vehicle/ticket is detected |
| 2 | `SpotsAvailable` | 1 when the lot has an available spot |
| 3 | `ValidTicket` | 1 when the ticket is valid |
| 4 | `GateOpenSensor` | 1 when the gate has reached fully open |
| 5 | `GateCloseSensor` | 1 when the gate has reached fully closed |
| 6 | `GateOpenTimerDone` | 1 when the open-gate waiting timer has finished |
| 7 | `Emergency` | 1 to force the controller into Failsafe |
| 8 | `Reset` | Used to leave Failsafe after the emergency is cleared |
| 9 | `CLOCK` | Pulse this to make the FSM move to its next state |

### How to pulse the clock

For each state transition:

1. Set the required inputs.
2. Turn `CLOCK` from **0 to 1**.
3. Turn `CLOCK` back from **1 to 0**.

The important part is the **0 → 1 transition**.

Do not leave unrelated inputs at 1 unless the test specifically needs them.

---

## 2. Output Map

The lights on the **far right side** are outputs.

### State indicator lights

The **top six lights**, from top to bottom, are:

| Position | State |
|---|---|
| 1 | `STATE Gate Closed` |
| 2 | `STATE Checking Info` |
| 3 | `STATE Opening Gate` |
| 4 | `STATE Gate Open + Waiting` |
| 5 | `STATE Closing Gate` |
| 6 | `STATE Failsafe` |

Normally, only **one state light should be active at a time**.

### Action / transition outputs

The **bottom six lights**, from top to bottom, are:

| Position | Output |
|---|---|
| 1 | `MotorOpeningGate` |
| 2 | `MotorClosingGate` |
| 3 | `Speaker_LotFullMessage` |
| 4 | `Speaker_InvalidTicketMessage` |
| 5 | `TakeOneSpot` |
| 6 | `StartGateOpenTimer` |

---

## 3. Starting / Resetting a Test

The default state is **Gate Closed + Waiting for Input**.

Before beginning a new normal test, use:

```text
TicketEntered = 0
SpotsAvailable = 0 or 1 as required
ValidTicket = 0 or 1 as required
GateOpenSensor = 0
GateCloseSensor = 0
GateOpenTimerDone = 0
Emergency = 0
Reset = 0
CLOCK = 0
```

Confirm that the top state output is active:

```text
STATE Gate Closed = 1
```

If the controller is currently in another state, finish that test sequence or reload/reset the simulation before beginning a new independent test.

---

# 4. Required Tests

## Test 1 — Valid Entry

Purpose: Verify that a valid vehicle with an available parking space is accepted.

### Step A — Enter Checking Info

Set:

```text
TicketEntered = 1
SpotsAvailable = 1
ValidTicket = 1
```

Pulse the clock once.

Expected state:

```text
STATE Checking Info = 1
```

### Step B — Approve Entry

Keep:

```text
SpotsAvailable = 1
ValidTicket = 1
```

Pulse the clock once again.

Expected:

```text
STATE Opening Gate = 1
MotorOpeningGate = 1
TakeOneSpot = 1
```

This verifies the normal valid-entry transition.

---

## Test 2 — Gate Fully Opens

Begin while the controller is in **Opening Gate**.

First verify that with:

```text
GateOpenSensor = 0
```

the controller remains in `Opening Gate` after a clock pulse.

Then set:

```text
GateOpenSensor = 1
```

Pulse the clock.

Expected:

```text
STATE Gate Open + Waiting = 1
StartGateOpenTimer = 1
```

After the transition, `GateOpenSensor` can be returned to 0.

---

## Test 3 — Gate Closes Normally

Begin in **Gate Open + Waiting**.

### Step A — Timer finishes

Set:

```text
GateOpenTimerDone = 1
```

Pulse the clock.

Expected:

```text
STATE Closing Gate = 1
MotorClosingGate = 1
```

Return `GateOpenTimerDone` to 0.

### Step B — Gate reaches closed position

While in Closing Gate, first verify:

```text
GateCloseSensor = 0
```

A clock pulse should leave the controller in `Closing Gate`.

Then set:

```text
GateCloseSensor = 1
```

Pulse the clock.

Expected:

```text
STATE Gate Closed = 1
```

Return `GateCloseSensor` to 0.

---

## Test 4 — Parking Lot Full

Start in **Gate Closed**.

Set:

```text
TicketEntered = 1
SpotsAvailable = 0
```

Pulse once to enter `Checking Info`.

Expected:

```text
STATE Checking Info = 1
```

Pulse again with `SpotsAvailable = 0`.

Expected:

```text
STATE Gate Closed = 1
Speaker_LotFullMessage = 1
```

The gate should **not** enter Opening Gate.

This also verifies the priority rule that a full lot is rejected before ticket validity matters.

---

## Test 5 — Invalid Ticket

Start in **Gate Closed**.

Set:

```text
TicketEntered = 1
SpotsAvailable = 1
ValidTicket = 0
```

Pulse once to enter `Checking Info`.

Pulse again.

Expected:

```text
STATE Gate Closed = 1
Speaker_InvalidTicketMessage = 1
```

The gate should **not** open and `TakeOneSpot` should not activate.

---

## Test 6 — Emergency / Failsafe

Emergency has priority over normal operation.

From **any normal state**, set:

```text
Emergency = 1
```

Pulse the clock.

Expected:

```text
STATE Failsafe = 1
```

You can repeat this from several states if desired, for example:

- Gate Closed
- Checking Info
- Opening Gate
- Gate Open + Waiting
- Closing Gate

All should enter Failsafe.

---

## Test 7 — Emergency Reset

Begin in **Failsafe**.

### If the gate is not yet open

Set:

```text
GateOpenSensor = 0
```

Expected Failsafe action:

```text
MotorOpeningGate = 1
```

The system remains in Failsafe while it opens the gate.

### Once the gate is fully open

Set:

```text
GateOpenSensor = 1
```

The controller should remain in Failsafe and no longer need to command further opening.

Now clear the emergency and request reset:

```text
Emergency = 0
Reset = 1
```

Pulse the clock.

Expected:

```text
STATE Closing Gate = 1
MotorClosingGate = 1
```

Then complete the normal closing sequence:

```text
GateCloseSensor = 1
```

Pulse the clock.

Expected:

```text
STATE Gate Closed = 1
```

Return `Reset`, `GateOpenSensor`, and `GateCloseSensor` to 0 afterward.

---

# 5. Quick Verification Table

| Test | Main Inputs | Expected Result |
|---|---|---|
| Valid entry | `TicketEntered=1`, `SpotsAvailable=1`, `ValidTicket=1` | Checking Info → Opening Gate; `MotorOpeningGate` and `TakeOneSpot` activate |
| Gate fully opens | `GateOpenSensor=1` while Opening Gate | Gate Open + Waiting; `StartGateOpenTimer` activates |
| Gate closes normally | `GateOpenTimerDone=1`, then `GateCloseSensor=1` | Closing Gate → Gate Closed; `MotorClosingGate` activates |
| Lot full | `TicketEntered=1`, `SpotsAvailable=0` | Returns to Gate Closed; `Speaker_LotFullMessage` activates |
| Invalid ticket | `TicketEntered=1`, `SpotsAvailable=1`, `ValidTicket=0` | Returns to Gate Closed; `Speaker_InvalidTicketMessage` activates |
| Emergency | `Emergency=1` from any normal state | Failsafe |
| Emergency reset | In Failsafe: `Emergency=0`, `Reset=1` | Closing Gate, then Gate Closed after close sensor |

---

# 6. State Table Rules Being Verified

The tests above verify these intended transitions:

```text
Gate Closed + Waiting
    TicketEntered=0              -> Gate Closed + Waiting
    TicketEntered=1              -> Checking Info

Checking Info
    SpotsAvailable=0             -> Gate Closed + Lot Full Message
    ValidTicket=0                -> Gate Closed + Invalid Ticket Message
    ValidTicket=1 AND SpotsAvailable=1
                                  -> Opening Gate + TakeOneSpot

Opening Gate
    GateOpenSensor=0             -> Opening Gate
    GateOpenSensor=1             -> Gate Open + Waiting + StartGateOpenTimer

Gate Open + Waiting
    GateOpenTimerDone=0          -> Gate Open + Waiting
    GateOpenTimerDone=1          -> Closing Gate

Closing Gate
    GateCloseSensor=0            -> Closing Gate
    GateCloseSensor=1            -> Gate Closed + Waiting

Any normal state
    Emergency=1                  -> Failsafe

Failsafe
    GateOpenSensor=0             -> Keep opening gate
    GateOpenSensor=1             -> Remain in Failsafe
    Reset=1 AND Emergency=0      -> Closing Gate
```

## Priority Rules

The controller was designed with these priorities:

```text
SpotsAvailable=0 > ValidTicket=0
Emergency=1 > Reset=1 > all normal conditions
```

Therefore:

- If the lot is full, the lot-full response takes priority over an invalid ticket.
- If `Emergency=1`, the controller should remain/go to Failsafe even if `Reset=1`.

---

# 7. What Counts as a Pass

The controller passes verification if:

- Each clocked transition reaches the state shown in the state table.
- Only the correct state indicator is active.
- `MotorOpeningGate` is active while opening.
- `MotorClosingGate` is active while closing.
- `TakeOneSpot` occurs for a valid accepted entry.
- `StartGateOpenTimer` occurs when the gate reaches fully open.
- The correct speaker output occurs for lot-full and invalid-ticket cases.
- `Emergency=1` sends every normal state to Failsafe.
- Clearing Emergency and asserting Reset allows the system to close the gate and return to Gate Closed.

If all seven tests behave as described, the Smart Parking Gate Controller matches the supplied state table.
