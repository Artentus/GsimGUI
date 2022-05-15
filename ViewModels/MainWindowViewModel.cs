using GsimGUI.Backend;
using System;

namespace GsimGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public MainWindowViewModel()
        {
            //const uint BusWidth = 32;
            //
            //var simulator = new Simulator();
            //var inputPin = simulator.AddInputPin(BusWidth);
            //var outputPin = simulator.AddOutputPin(BusWidth);
            //
            //Span<Wire> bus = stackalloc Wire[(int)BusWidth];
            //for (uint i = 0; i < BusWidth; i++)
            //{
            //    var wire = simulator.AddWire();
            //    wire.AddDriver(inputPin, (0, i));
            //    bus[(int)i] = wire;
            //}
            //
            //((Component)outputPin).ConnectInput(0, bus);
            //
            //Span<LogicState> input = stackalloc LogicState[(int)BusWidth];
            //Span<LogicState> output = stackalloc LogicState[(int)BusWidth];
            //for (uint i = 0; i < BusWidth; i++)
            //{
            //    input[(int)i] = ((i % 2) == 0) ? LogicState.Logic0 : LogicState.Logic1;
            //}
            //
            //inputPin.Set(input);
            //while (!simulator.Step()) { }
            //simulator.Step();
            //outputPin.Get(output);
            //
            //string.Join(", ", output.ToArray());
        }
    }
}
