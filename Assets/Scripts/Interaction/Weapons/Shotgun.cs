using UnityEngine.Events;

public class Shotgun : Gun
{
    bool isPumped = true;
    bool pumpDown = false;

    public UnityEvent OnPumpDown;
    public UnityEvent OnPumpUp;

    protected override bool CanShoot => base.CanShoot && isPumped;

    public void PumpDown()
    {
        if (!isPumped && !pumpDown)
        {
            pumpDown = true;
            OnPumpDown.Invoke();
        }  
    }

    public void PumpUp()
    {
        if (!isPumped && pumpDown)
        {
            pumpDown = false;
            isPumped = true;
            OnPumpUp.Invoke();
        }
    }

    protected override void Shoot()
    {
        base.Shoot();
        isPumped = false;
    }
}