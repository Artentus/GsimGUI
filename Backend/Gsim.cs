using System;
using System.Runtime.InteropServices;

namespace GsimGUI.Backend
{
    internal class SimulationException : Exception
    {
        public int Code { get; }

        public SimulationException(int code, string? message = null)
            : base(message)
        {
            Code = code;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Result
    {
        public const int Success = 0;
        public const int DriverAlreadyPresent = 1;
        public const int DriverNotPresent = 2;
        
        public const int NullPointerError = -1;
        public const int InvalidComponentIdError = -2;
        public const int InvalidWireIdError = -3;
        public const int InvalidComponentConfigurationError = -4;
        public const int InvalidOutputIndexError = -5;
        public const int ConflictError = -6;
        public const int InvalidLogicStateError = -7;

        public readonly int code;

        public void ThrowIfError()
        {
            if (code < 0)
            {
                string? message = code switch
                {
                    NullPointerError => throw new NullReferenceException(),
                    InvalidComponentIdError => "Invalid component ID",
                    InvalidWireIdError => "Invalid wire ID",
                    InvalidComponentConfigurationError => "Invalid component configuration",
                    InvalidOutputIndexError => "Invalid output index",
                    ConflictError => "Conflict",
                    InvalidLogicStateError => "Invalid logic state",
                    _ => "Unknown simulation exception",
                };

                throw new SimulationException(code, message);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal record struct ComponentId(ulong Id);

    [StructLayout(LayoutKind.Sequential)]
    internal record struct WireId(ulong Id);

    internal enum LogicState : uint
    {
        HighZ = 0,
        Undefined = 1,
        Logic0 = 2,
        Logic1 = 3,
    }

    internal enum ComponentKind : uint
    {
        Constant = 0,
        Unary = 1,
        Binary = 2,
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1069")]
    internal enum ComponentSubKind : uint
    {
        ConstantPullDown = 0,
        ConstantPullUp = 1,

        UnaryNot = 0,

        BinaryAnd = 0,
        BinaryNand = 1,
        BinaryOr = 2,
        BinaryNor = 3,
        BinaryXor = 4,
        BinaryXnor = 5,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ComponentCreateInfo
    {
        public ComponentKind Kind;
        public ComponentSubKind SubKind;
        public uint Width;
        public uint InputCount;
    }

    internal static class Gsim
    {
        [DllImport("gsim", EntryPoint = "simulator_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorCreate(void** outSimulator);

        [DllImport("gsim", EntryPoint = "simulator_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorDestroy(void* simulator);

        [DllImport("gsim", EntryPoint = "simulator_add_component", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorAddComponent(void* simulator, ComponentCreateInfo createInfo, ComponentId* outId);

        [DllImport("gsim", EntryPoint = "simulator_add_input_pin", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorAddInputPin(void* simulator, uint width, ComponentId* outId);

        [DllImport("gsim", EntryPoint = "simulator_add_output_pin", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorAddOutputPin(void* simulator, uint width, ComponentId* outId);

        [DllImport("gsim", EntryPoint = "simulator_remove_component", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorRemoveComponent(void* simulator, ComponentId id);

        [DllImport("gsim", EntryPoint = "simulator_add_wire", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorAddWire(void* simulator, WireId* outId);

        [DllImport("gsim", EntryPoint = "simulator_remove_wire", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorRemoveWire(void* simulator, WireId id);

        [DllImport("gsim", EntryPoint = "simulator_step", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result SimulatorStep(void* simulator, uint* outChanged);


        [DllImport("gsim", EntryPoint = "component_connect_input", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result ComponentConnectInput(void* simulator, ComponentId id, uint inputIndex, uint wireCount, WireId* wires);

        [DllImport("gsim", EntryPoint = "component_disconnect_input", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result ComponentDisconnectInput(void* simulator, ComponentId id, uint inputIndex);

        [DllImport("gsim", EntryPoint = "input_pin_set", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result InputPinSet(void* simulator, ComponentId id, uint stateCount, LogicState* states);

        [DllImport("gsim", EntryPoint = "output_pin_get", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result OutputPinGet(void* simulator, ComponentId id, uint stateCount, LogicState* states);


        [DllImport("gsim", EntryPoint = "wire_get_state", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result WireGetState(void* simulator, WireId id, LogicState* outState);

        [DllImport("gsim", EntryPoint = "wire_add_driver", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result WireAddDriver(void* simulator, WireId id, ComponentId component, uint outputIndex, uint outputSubIndex);

        [DllImport("gsim", EntryPoint = "wire_remove_driver", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe Result WireRemoveDriver(void* simulator, WireId id, ComponentId component, uint outputIndex, uint outputSubIndex);
    }
}
