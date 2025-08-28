namespace Minecraft_launcher
{
    public class MathExtra
    {
        public static float Clamp01Float(float value)
        {
            if (value > 1) return 1f;
            if (value < 0) return 0f;
            return value;
        }

        public static double Clamp01Double(double value)
        {
            if (value > 1) return 1d;
            if (value < 0) return 0d;
            return value;
        }


        public class Interpolations
        {
            public static double LerpDouble(double startValue, double targetValue, double timeProportion)
            {
                return startValue + (targetValue - startValue) * timeProportion;
            }

            public static double LerpDouble(double startValue, double targetValue, float elapsedTime, float duration)
            {
                float timeProportion = Clamp01Float(elapsedTime / duration);
                return LerpDouble(startValue, targetValue, timeProportion);
            }

            public static double LerpDouble(double startValue, double targetValue, float startTime, float currentTime, float duration)
            {
                float elapsedTime = currentTime - startTime;
                return LerpDouble(startValue, targetValue, elapsedTime, duration);
            }

            public class Dynamics
            {
                public class SmoothDampFollower
                {
                    private double velocity;

                    public double SmoothDamp(double position, double targetPosition, ref double velocity, double smoothTime, double deltaTime) // this uses the spring-damper system equation
                    {
                        //Debugger.SendInfo("Target position is " + targetPosition);
                        // avoid /0 error
                        smoothTime = Math.Max(0.0001, smoothTime);

                        double omega = 2.0 / smoothTime; //frequency of reaction time
                        double x = omega * deltaTime; //makes x frame independant
                        //do not ask about the following I do not know
                        double exp = 1.0 / (1.0 + x + 0.48 * x * x + 0.235 * x * x * x);

                        double targetDistance = position - targetPosition;
                        double temp = (velocity + omega * targetDistance) * deltaTime;
                        velocity = (velocity - omega * temp) * exp;
                        double result = targetPosition + (targetDistance + temp) * exp; 

                        return result;
                    }
                }
            }
        }
        
    }
}
