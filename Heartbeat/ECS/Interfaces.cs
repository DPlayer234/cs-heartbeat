namespace Heartbeat
{
    /// <summary>
    ///     Makes an <see cref="ECSObject"/> updatable.
    /// </summary>
    public interface IUpdateObject
    {
        void Update();
        void LateUpdate();
    }

    /// <summary>
    ///     Makes an <see cref="ECSObject"/> drawable.
    /// </summary>
    public interface IDrawObject
    {
        void Draw();
    }

    /// <summary>
    ///     Allows an <see cref="ECSObject"/> to handle physics callbacks.
    /// </summary>
    public interface IPhysicObject
    {
        void OnCollisionBegin();
        void OnCollisionStay();
        void OnCollisionEnd();

        void OnSensorBegin();
        void OnSensorStay();
        void OnSensorEnd();
    }
}
