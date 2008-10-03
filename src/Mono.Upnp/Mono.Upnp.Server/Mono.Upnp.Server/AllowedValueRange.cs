namespace Mono.Upnp.Server
{
	public class AllowedValueRange
	{
        private readonly object max_value;
        private readonly object min_value;
        private readonly object steps;

        public AllowedValueRange (object maxValue, object minValue, object steps)
        {
            max_value = maxValue;
            min_value = minValue;
            this.steps = steps;
        }

        public object MaxValue {
            get { return max_value; }
        }

        public object MinValue {
            get { return min_value; }
        }

        public object Steps {
            get { return steps; }
        }
	}
}
