public interface ILevelObject
{
    /// <summary>
    /// Restarts the object to default state to sync with player.
    /// </summary>
    void restartObject();

    /// <summary>
    /// Turns off the unnecessary objects for optimization.
    /// </summary>
    void turnOffObject();
}
