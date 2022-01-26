using System;
using System.Collections.Generic;
using System.Text;

namespace SmartGardenVirtualSensor
{
    public class HumidityHelper
    {
        private float _currentValue;
        public float CurrentValue { 
            get { return _currentValue; } 
            set { if (value > 100) _currentValue = 100; 
                else if (value < 0) _currentValue = 0;
                else _currentValue = value; } 
        }
        public float Step { get; set; }

        public HumidityHelper(float startValue, float step)
        {
            CurrentValue = startValue;
            Step = step;
        }

        public void IncreaseHumidity(float increment)
        {
            CurrentValue += increment;
        }

        public void Tick()
        {
            CurrentValue -= Step;
        }
    }
}
