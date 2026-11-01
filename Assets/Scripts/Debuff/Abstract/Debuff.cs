public abstract class Debuff
{
    float interval;
    int ticksLeft;
    float timer;

    public bool IsFinished => ticksLeft <= 0;

    protected Debuff(float interval, int ticks)
    {
        this.interval = interval;
        ticksLeft = ticks;
    }

    public void Update(EnemyAI target, float deltaTime)
    {
        timer += deltaTime;

        if (timer >= interval)
        {
            timer -= interval;
            ticksLeft--;
            OnTick(target);
        }
    }

    protected abstract void OnTick(EnemyAI target);
}