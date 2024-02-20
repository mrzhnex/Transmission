using AudioVisualizationLibrary.Enums;
using System;
using System.Collections.Generic;

namespace AudioVisualizationLibrary.Classes
{
    public class ProgressInfo
    {
        public delegate void ValueChangedEventHandler(ProgressInfo sender);

        public event ValueChangedEventHandler ValueChanged;
        public event ValueChangedEventHandler MinimumValueChanged;
        public event ValueChangedEventHandler MaximumValueChanged;

        public Directions Direction;
        private double MinimumValue = 0;
        public double Minimum
        {
            get => this.MinimumValue;
            set
            {
                this.MinimumValue = value;
                this.MinimumValueChanged?.Invoke(this);
            }
        }

        private double ValueField;
        public double Value
        {
            get => this.ValueField;
            set
            {
                this.ValueField = value;
                this.ValueChanged?.Invoke(this);
            }
        }

        private double MaximumValue = 100;
        public double Maximum
        {
            get => this.MaximumValue;
            set
            {
                this.MaximumValue = value;
                this.MaximumValueChanged?.Invoke(this);
            }
        }
        public ProgressInfo()
        {

        }
        ~ProgressInfo()
        {
            GC.SuppressFinalize(this);
        }
    }
    public class DeviceListInfo
    {
        public delegate void SelectedIndexChangedEventHandler(int index);
        public event SelectedIndexChangedEventHandler SelectedIndexChanged;

        public List<String> Items;

        private int SelectedIndexValue = -1;
        public int SelectedIndex
        {
            get => this.SelectedIndexValue;
            set
            {
                this.SelectedIndexValue = value;
                this.SelectedIndexChanged?.Invoke(value);
            }
        }
        public bool IsEnabled;
        public DeviceListInfo()
        {
            this.Items = new List<string>();
        }

        ~DeviceListInfo()
        {
            GC.SuppressFinalize(this);
        }
    }
}
