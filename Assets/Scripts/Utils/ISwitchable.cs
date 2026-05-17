/// <summary>
/// Objet activable par un interrupteur (remplace SendMessage).
/// </summary>
public interface ISwitchable
{
    void EnableFromSwitch();
    void CallFromSwitch();
}
