// This is an interface, a contract that all our behavior scripts must follow.
// It ensures that every behavior has an Initialize method.
public interface IEnemyBehavior
{
    void Initialize(EnemyData data);
}
