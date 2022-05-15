using System;

namespace GsimGUI.Backend
{
    internal sealed class Simulator : IDisposable
    {
        readonly IntPtr handle;
        bool disposed;

        public Simulator()
        {
            unsafe
            {
                void* ptr = IntPtr.Zero.ToPointer();
                Gsim.SimulatorCreate(&ptr).ThrowIfError();
                handle = new IntPtr(ptr);
            }
        }

        public Component AddComponent(ComponentCreateInfo createInfo)
        {
            var id = new ComponentId(0);
            unsafe { Gsim.SimulatorAddComponent(handle.ToPointer(), createInfo, &id).ThrowIfError(); }
            return new Component(handle, id);
        }

        public InputPin AddInputPin(uint width)
        {
            var id = new ComponentId(0);
            unsafe { Gsim.SimulatorAddInputPin(handle.ToPointer(), width, &id).ThrowIfError(); }
            return new InputPin(handle, id);
        }

        public OutputPin AddOutputPin(uint width)
        {
            var id = new ComponentId(0);
            unsafe { Gsim.SimulatorAddOutputPin(handle.ToPointer(), width, &id).ThrowIfError(); }
            return new OutputPin(handle, id);
        }

        public void RemoveComponent(in Component component)
        {
            unsafe { Gsim.SimulatorRemoveComponent(handle.ToPointer(), component.Id).ThrowIfError(); }
        }

        public Wire AddWire()
        {
            var id = new WireId(0);
            unsafe { Gsim.SimulatorAddWire(handle.ToPointer(), &id).ThrowIfError(); }
            return new Wire(handle, id);
        }

        public void RemoveWire(in Wire wire)
        {
            unsafe { Gsim.SimulatorRemoveWire(handle.ToPointer(), wire.Id).ThrowIfError(); }
        }

        public bool Step()
        {
            unsafe
            {
                uint changed = 0;
                Gsim.SimulatorStep(handle.ToPointer(), &changed).ThrowIfError();
                return changed != 0;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                { }

                unsafe { Gsim.SimulatorDestroy(handle.ToPointer()).ThrowIfError(); }
                disposed = true;
            }
        }

        ~Simulator()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
