using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject gateAnchor;
    public GameObject carPastGatePoint;
    public GameObject currentWaitingCar;
    public GameObject carObjToSpawn;
    public GameObject carStartPoint;
    GameObject targetSpot;

    float translationTimeCounter;
    float clampedTranslationCurrentValue;
    float translationTargetTime;

    bool moveCarPastGate;
    bool moveCarToSpot;

    bool TicketEntered;
    bool Emergency;
    bool Reset;

    bool carCurrentlyWaiting = true;

    bool sentFailsafeOpenSignal;

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

    Quaternion startingRotation;
    Quaternion targetRotation;
    float rotationTargetTime;
    float rotationTimeCounter;
    float clampedRotationCurrentValue;

    List<int> validTicketNumbers = new List<int>() { 173091, 128439 };
    public int userTicketNumber = 128439;

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
        if (rotationTimeCounter < rotationTargetTime)
        {
            clampedRotationCurrentValue = rotationTimeCounter / rotationTargetTime;
            gateAnchor.transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, clampedRotationCurrentValue);
            rotationTimeCounter += Time.deltaTime;
        }

        if (moveCarPastGate && translationTimeCounter <= (translationTargetTime / 2))
        {
            clampedTranslationCurrentValue = translationTimeCounter / (translationTargetTime / 2);
            currentWaitingCar.transform.position = Vector2.Lerp(carStartPoint.transform.position, carPastGatePoint.transform.position, clampedTranslationCurrentValue);
            translationTimeCounter += Time.deltaTime;
        }
        else if (moveCarPastGate && translationTimeCounter > (translationTargetTime / 2))
        {
            moveCarPastGate = false;
            moveCarToSpot = true;
            translationTimeCounter = 0f;
        }

        if (moveCarToSpot && translationTimeCounter <= (translationTargetTime / 2))
        {
            clampedTranslationCurrentValue = translationTimeCounter / (translationTargetTime / 2);
            currentWaitingCar.transform.position = Vector2.Lerp(carPastGatePoint.transform.position, targetSpot.transform.position, clampedTranslationCurrentValue);
            translationTimeCounter += Time.deltaTime;
        }
        else if (moveCarToSpot && translationTimeCounter > (translationTargetTime / 2))
        {
            moveCarToSpot = false;
            translationTimeCounter = 0f;
            carCurrentlyWaiting = false;
        }

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

    void MoveCarToSpot(float time)
    {
        if (takenSpots < totalSpots)
        {
            translationTargetTime = time;
            translationTimeCounter = 0f;
            moveCarPastGate = true;
            targetSpot = GameObject.Find($"CarSpot{++takenSpots}");
        }
    }

    void GateRotate(int angle, int time)
    {
        rotationTimeCounter = 0f;
        startingRotation = gateAnchor.transform.rotation;
        targetRotation = startingRotation * Quaternion.Euler(0, 0, -angle);
        rotationTargetTime = time;
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
                TicketEntered = false;
                return State.CLOSED;
            }
            else if (!ticketValid)
            {
                outputs.Add("Speaker_InvalidTicket");
                TicketEntered = false;
                return State.CLOSED;
            }
            else
            {
                outputs.Add("TakeOneSpot");
                GateRotate(90, GateOpeningTime);
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
                MoveCarToSpot(GateOpenTime);
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
                GateRotate(-90, GateClosingTime);
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
                if (!carCurrentlyWaiting)
                {
                    currentWaitingCar = Instantiate(carObjToSpawn, carStartPoint.transform.position, carObjToSpawn.transform.rotation);
                    carCurrentlyWaiting = true;
                }
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
                sentFailsafeOpenSignal = false;
                Reset = false;
                GateRotate(-90, GateClosingTime);
                return State.CLOSING;
            }
            else if (GateOpeningTimerDone)
            {
                return currentState;
            }
            else if (!GateOpeningTimerDone)
            {
                outputs.Add("MotorOpenGate");
                if (!sentFailsafeOpenSignal)
                {
                    sentFailsafeOpenSignal = true;
                    GateRotate(90, GateOpeningTime);
                }
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
