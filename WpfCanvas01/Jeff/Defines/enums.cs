namespace Jeff.Defines
{
    public enum MouseSate
    {
        None = 0,
        LeftPressed = 1,
        LeftDragging = 2,
        LeftRelased = 3,
        LeftClicked = 4,
        MidPressed = 5,
        MidDragging = 6,
        MidReleased = 7,
        MidClicked = 8,
        RightPressed = 9,
        RightDragging = 10,
        RightReleased = 11,
        RightClicked = 12
    }

    public enum DrawState
    {
        None = 0,
        Drawing = 1,
        Selecting = 2,
        ResizeOver = 3,
        Resizing = 6,
        RotateOver = 5,
        Rotating = 6
    }

    public enum SelectMode
    {
        OFF = 0,
        ON = 1
    }

    /// <summary>
    /// R_ : Rotate
    /// S_ : reSize
    /// </summary>
    public enum MousePosition
    {
        None = 0,
        R_LT = 1,
        R_RT = 2,
        R_RB = 3,
        R_LB = 4,
        S_LM = 5,
        S_LT = 6,
        S_TM = 7,
        S_RT = 8,
        S_RM = 9,
        S_RB = 10,
        S_BM = 11,
        S_LB = 12
    }

    public enum MouseArrowState
    {
        Normal = 0,
        Select = 1,
        SLT = 2,
        SMT = 3,
        SRT = 4,
        SRM = 5,
        SRB = 6,
        SMB = 7,
        SLB = 8,
        SLM = 9,
        RLT = 10,
        RRT = 11,
        RRB = 12,
        RLB = 13
    }
}
