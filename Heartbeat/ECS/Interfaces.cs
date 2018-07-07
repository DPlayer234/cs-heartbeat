namespace Heartbeat
{
    /// <summary>
    ///     Makes an <see cref="ECSObject"/> updatable.
    /// </summary>
    public interface IUpdatable
    {
        void Update();
        void LateUpdate();
    }

    /// <summary>
    ///     Makes an <see cref="ECSObject"/> drawable.
    /// </summary>
    public interface IDrawable
    {
        void Draw();
    }

    /// <summary>
    ///     Allows an <see cref="ECSObject"/> to handle physics callbacks.
    /// </summary>
    public interface IPhysicHandler
    {
        void OnCollisionBegin();
        void OnCollisionStay();
        void OnCollisionEnd();

        void OnSensorBegin();
        void OnSensorStay();
        void OnSensorEnd();
    }
}
