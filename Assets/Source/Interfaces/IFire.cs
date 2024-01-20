using System;

public interface IFire
{
    public event Action OnFire;

    public void SetInput(bool input);
}