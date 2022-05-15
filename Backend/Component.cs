using System;
using System.Collections.Generic;

namespace GsimGUI.Backend
{
    internal readonly struct Component
    {
        readonly IntPtr simulator;
        internal readonly ComponentId Id;

        public Component(IntPtr simulator, ComponentId id)
        {
            this.simulator = simulator;
            Id = id;
        }

        public void ConnectInput(uint inputIndex, in IList<Wire> wires)
        {
            if (wires.Count <= 64)
            {
                Span<WireId> wireIds = stackalloc WireId[wires.Count];
                for (int i = 0; i < wires.Count; i++)
                    wireIds[i] = wires[i].Id;

                unsafe
                {
                    fixed (WireId* ptr = wireIds)
                        Gsim.ComponentConnectInput(simulator.ToPointer(), Id, inputIndex, (uint)wires.Count, ptr);
                }
            }
            else
            {
                var wireIds = new WireId[wires.Count];
                for (int i = 0; i < wires.Count; i++)
                    wireIds[i] = wires[i].Id;

                unsafe
                {
                    fixed (WireId* ptr = wireIds)
                        Gsim.ComponentConnectInput(simulator.ToPointer(), Id, inputIndex, (uint)wires.Count, ptr);
                }
            }
        }

        public void DisconnectInput(uint inputIndex)
        {
            unsafe { Gsim.ComponentDisconnectInput(simulator.ToPointer(), Id, inputIndex); }
        }
    }

    internal readonly struct InputPin
    {
        readonly IntPtr simulator;
        internal readonly ComponentId Id;

        public InputPin(IntPtr simulator, ComponentId id)
        {
            this.simulator = simulator;
            Id = id;
        }

        public void Set(ReadOnlySpan<LogicState> state)
        {
            unsafe
            {
                fixed (LogicState* ptr = state)
                    Gsim.InputPinSet(simulator.ToPointer(), Id, (uint)state.Length, ptr);
            }
        }

        public static implicit operator Component(InputPin pin)
            => new(pin.simulator, pin.Id);
    }

    internal readonly struct OutputPin
    {
        readonly IntPtr simulator;
        internal readonly ComponentId Id;

        public OutputPin(IntPtr simulator, ComponentId id)
        {
            this.simulator = simulator;
            Id = id;
        }

        public void Get(Span<LogicState> state)
        {
            unsafe
            {
                fixed (LogicState* ptr = state)
                    Gsim.OutputPinGet(simulator.ToPointer(), Id, (uint)state.Length, ptr);
            }
        }

        public static implicit operator Component(OutputPin pin)
            => new(pin.simulator, pin.Id);
    }
}
