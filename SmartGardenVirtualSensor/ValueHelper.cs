using System;
using System.Collections.Generic;
using System.Text;

namespace SmartGardenVirtualSensor
{
    /// <summary>
    /// Класс - помощник для генерации значений
    /// </summary>
    public class ValueHelper
    {
        public int GenerateValue(int min, int max)
        {
            return new Random().Next(min, max);
        }

        public int GenerateValue()
        {
            return new Random().Next(0, 100);
        }
    }
}
