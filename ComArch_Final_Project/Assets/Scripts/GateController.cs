using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    bool TicketEntered;
    bool Emergency;
    bool Reset;

    public int takenSpots;
    public int totalSpots;

    public int GateOpenTime = 2;
    int gateOpenCounter;
    bool GateOpenTimerDone;

    public int GateOpeningTime = 2;
    int gateOpeningCounter;
    bool GateOpeningTimerDone;

    public int GateClosingTime = 2;
    int gateClosingCounter;
    bool GateClosingTimerDone;

    public float clockDelay = 1f;
    float clockTimer;

    List<int> validTicketNumbers = new List<int>() { 173091, 128439 };
    int userTicketNumber = 128439;

    enum State
    {
        CLOSED,
        CHECKING,
        OPENING,
        OPEN,
        CLOSING,
        FAILSAFE
    }

    List<string> outputs = new List<string>();

    State currentStateGlobal = State.CLOSED;
    State nextState;

    void Update()
    {
        CheckForInputs();

        clockTimer += Time.deltaTime;

        if (clockTimer >= clockDelay)
        {
            clockTimer = 0f;

            nextState = Transition(currentStateGlobal);

            currentStateGlobal = nextState;

            string toBeOutput = "";

            for (int i = 0; i < outputs.Count; i++)
            {
                toBeOutput += outputs[i];
            }

            Debug.Log($"{currentStateGlobal}: {toBeOutput}");

            outputs.Clear();

            if (Reset)
            {
                Reset = false;
            }
        }
    }

    void CheckForInputs()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TicketEntered = true;
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Emergency = true;
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            Emergency = false;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Reset = true;
        }
    }

    void CheckTicket(out bool ticketValid)
    {
        ticketValid = false;

        for (int i = 0; i < validTicketNumbers.Count; i++)
        {
            if (userTicketNumber == validTicketNumbers[i])
            {
                ticketValid = true;
            }
        }
    }

    State Transition(State currentState)
    {
        if (Emergency && currentState != State.FAILSAFE)
        {
            return State.FAILSAFE;
        }

        else if (currentState == State.CLOSED)
        {
            if (TicketEntered)
                return State.CHECKING;
            // Send output to unlock door
            else
                return currentState;
        }

        else if (currentState == State.CHECKING)
        {
            bool ticketValid;
            CheckTicket(out ticketValid);

            if (takenSpots >= totalSpots)
            {
                outputs.Add("Speaker_LotsFull");
                return currentState;
            }
            else if (!ticketValid)
            {
                outputs.Add("Speaker_InvalidTicket");
                return currentState;
            }
            else
            {
                outputs.Add("TakeOneSpot");
                takenSpots++;
                return State.OPENING;
            }
        }

        else if (currentState == State.OPENING)
        {
            if (gateOpeningCounter < GateOpeningTime)
            {
                GateOpeningTimerDone = false;
            }
            else
            {
                GateOpeningTimerDone = true;
            }

            if (GateOpeningTimerDone)
            {
                gateOpeningCounter = 0;
                outputs.Add("StartGateOpenTimer");
                return State.OPEN;
            }
            else
            {
                gateOpeningCounter++;
                return currentState;
            }
        }

        else if (currentState == State.OPEN)
        {
            if (gateOpenCounter >= GateOpenTime)
            {
                GateOpenTimerDone = true;
            }
            else
            {
                GateOpenTimerDone = false;
            }

            if (GateOpenTimerDone)
            {
                gateOpenCounter = 0;
                return State.CLOSING;
            }
            else
            {
                gateOpenCounter++;
                return currentState;
            }
        }

        else if (currentState == State.CLOSING)
        {
            if (gateClosingCounter < GateClosingTime)
            {
                GateClosingTimerDone = false;
            }
            else
            {
                GateClosingTimerDone = true;
            }

            if (GateClosingTimerDone)
            {
                gateClosingCounter = 0;
                TicketEntered = false;
                return State.CLOSED;
            }
            else
            {
                gateClosingCounter++;
                return currentState;
            }
        }

        else if (currentState == State.FAILSAFE)
        {
            if (gateOpeningCounter < GateOpeningTime)
            {
                GateOpeningTimerDone = false;
            }
            else
            {
                GateOpeningTimerDone = true;
            }

            if (Reset && !Emergency)
            {
                gateOpeningCounter = 0;
                Reset = false;
                return State.CLOSING;
            }
            else if (GateOpeningTimerDone)
            {
                return currentState;
            }
            else if (!GateOpeningTimerDone)
            {
                outputs.Add("MotorOpenGate");
                gateOpeningCounter++;
                return currentState;
            }
            else
            {
                Debug.LogError("No case was true in FAILSAFE block of if/else chain");
                return currentState;
            }
        }

        else
        {
            Debug.LogError("State not part of enum");
            return currentState;
        }
    }
}
