using UnityEngine;

public class ArmsAttackManager : MonoBehaviour
{
    public BossManager managerBoss;

    private void FistDerActivate()
    {
        managerBoss.FistDer();
    }
    
    private void FistIzqActivate()
    {
        managerBoss.FistIzq();
    }

    private void FireballActivate()
    {
        managerBoss.Fireball();
    }

    private void DoubleFistActivate()
    {
        managerBoss.DoubleFist();
    }

    
    
    private void EndAttackActivate()
    {
        managerBoss.EndAttack();

    }
    
}
