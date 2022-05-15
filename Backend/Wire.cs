using System;

namespace GsimGUI.Backend
{
    internal readonly struct Wire
    {
        readonly IntPtr simulator;
        internal readonly WireId Id;

        public Wire(IntPtr simulator, WireId id)
        {
            this.simulator = simulator;
            Id = id;
        }

        public LogicState GetState()
        {
            unsafe {
                var state = LogicState.HighZ;
                Gsim.WireGetState(simulator.ToPointer(), Id, &state).ThrowIfError();
                return state;
            }
        }

        public bool AddDriver(in Component component, (uint, uint) outputIndex)
        {
            unsafe
            {
                var result = Gsim.WireAddDriver(simulator.ToPointer(), Id, component.Id, outputIndex.Item1, outputIndex.Item2);
                result.ThrowIfError();
                return result.code != Result.DriverAlreadyPresent;
            }
        }

        public bool RemoveDriver(in Component component, (uint, uint) outputIndex)
        {
            unsafe
            {
                var result = Gsim.WireRemoveDriver(simulator.ToPointer(), Id, component.Id, outputIndex.Item1, outputIndex.Item2);
                result.ThrowIfError();
                return result.code != Result.DriverNotPresent;
            }
        }
    }
}
