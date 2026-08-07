using System;
using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    bool ValidCard = true;
    bool Emergency;
    bool Reset;
    bool DoorOpenSensor = true;
    bool DoorClosedSensor = true;

    float timer;
    bool TimerDone;
    bool timerStart;
    int timerDuration = 2;

    public float clockDelay = 1f;
    public float clockTimer;

    enum State
    {
        LOCKED,
        UNLOCKING,
        OPEN,
        CLOSING,
        ERROR
    }

    State currentStateGlobal = State.LOCKED;
    State nextState;

    void Update()
    {
        TimerController();

        clockTimer += Time.deltaTime;

        if (clockTimer >= clockDelay)
        {
            clockTimer = 0f;

            Debug.Log(currentStateGlobal);

            nextState = Transition(currentStateGlobal);

            currentStateGlobal = nextState;
        }
    }

    State Transition(State currentState)
    {
        if (Emergency)
        {
            return State.ERROR;
        }
        else if (currentState == State.LOCKED)
        {
            if (ValidCard)
                return State.UNLOCKING;
            // Send output to unlock door
            else
                return currentState;
        }
        else if (currentState == State.UNLOCKING)
        {
            if (DoorOpenSensor)
                return State.OPEN;
            else
                return currentState;
        }
        else if (currentState == State.OPEN)
        {
            timerStart = true;

            if (TimerDone)
                return State.CLOSING;
            else
                return currentState;
        }
        else if (currentState == State.CLOSING)
        {
            if (DoorClosedSensor)
            {
                return State.LOCKED;
                // Send output to lock door
            }
            else
                return currentState;
        }
        else if (currentState == State.ERROR)
        {
            if (Reset && !Emergency)
            {
                return State.LOCKED;
            }
            else
                return currentState;
        }
        else
        {
            return currentState;
        }

    }

    void TimerController()
    {
        if (timerStart)
        {
            timer += Time.deltaTime;

            if (TimerDone)
            {
                timerStart = false;
                timer = 0;
                TimerDone = false;
            }

            if (timer >= timerDuration)
            {
                TimerDone = true;
            }
        }
    }
}
