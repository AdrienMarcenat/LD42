﻿using System.Collections.Generic;

public class CommandStack
{
    private Stack<Command> m_CommandStack;

    public CommandStack ()
    {
        m_CommandStack = new Stack<Command> ();
        this.RegisterAsListener ("Player", typeof (PlayerInputGameEvent));
    }

    ~CommandStack()
    {
        this.RegisterAsListener ("Player");
    }

    public void PushCommand(Command command)
    {
        m_CommandStack.Push (command);
    }

    public Command PopCommand ()
    {
        return m_CommandStack.Pop ();
    }

    public void OnGameEvent (PlayerInputGameEvent inputEvent)
    {
        if (UpdaterProxy.Get().IsPaused())
        {
            return;
        }
        string input = inputEvent.GetInput ();
        EInputState state = inputEvent.GetInputState ();
        if (state == EInputState.Down)
        {
            switch (input)
            {
                case "Undo":
                    if (GetNumberOfCommand () > 0)
                    {
                        PopCommand ().Undo ();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public int GetNumberOfCommand()
    {
        return m_CommandStack.Count;
    }

    public void Reset()
    {
        m_CommandStack.Clear ();
    }
}

public class CommandStackProxy : UniqueProxy<CommandStack>
{ }
