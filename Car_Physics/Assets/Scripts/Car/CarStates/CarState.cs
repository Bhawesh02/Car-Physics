
    public abstract class CarState : IState
    {
        protected CarController CarController;

        protected CarState(CarController carController)
        {
            CarController = carController;
        }

        public virtual void OnEnter() {}
        public virtual void OnUpdate() {}
        public virtual void OnFixedUpdate() {}
        public virtual void OnExit() {}
        public virtual void OnDrawGizmos() {}
    }
