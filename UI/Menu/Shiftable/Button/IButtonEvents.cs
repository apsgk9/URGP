using System;

namespace UI.Button
{
    public interface IButtonConfirmCallback
{
    Action OnButtonConfirm { get; set; }
}

public interface IButtonSelectCallback
{
    Action OnButtonSelect { get; set; }
}

public interface IButtonDeselectCallback
{
    Action OnButtonDeselect { get; set; }
}
}

