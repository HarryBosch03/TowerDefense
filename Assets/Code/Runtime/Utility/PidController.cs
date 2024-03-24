namespace TowerDefense.Runtime.Utility
{
    [System.Serializable]
    public class PidController
    {
        public float p, i, d;

        private float integralLast;
        private float errorLast;

        public float ComputeForce(float position, float targetPosition, float deltaTime) => ComputeForce(targetPosition - position, deltaTime);

        public float ComputeForce(float error, float deltaTime)
        {
            var integral = integralLast + error * deltaTime;
            var derivative = (error - errorLast) / deltaTime;
            var output = p * error + i * integral + d * derivative;

            errorLast = error;
            integralLast = integral;

            return output;
        }
    }
}