﻿using GHI.IO;
using GHI.IO.Storage;
using GHI.Pins;
using GHI.Processor;
using GHI.Usb;
using GHI.Usb.Host;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using FEZCerberusPins = GHI.Pins.FEZCerberus;

namespace GHIElectronics.Gadgeteer {
    /// <summary>The mainboard class for the FEZ Cerberus.</summary>
    public class FEZCerberus : GT.Mainboard {
        private bool configSet;
        private OutputPort debugLed;
        private IRemovable[] storageDevices;

        /// <summary>The name of the mainboard.</summary>
        public override string MainboardName {
            get { return "GHI Electronics FEZ Cerberus"; }
        }

        /// <summary>The current version of the mainboard hardware.</summary>
        public override string MainboardVersion {
            get { return "1.3"; }
        }

        /// <summary>Constructs a new instance.</summary>
        public FEZCerberus() {
            this.configSet = false;
            this.debugLed = null;
            this.storageDevices = new IRemovable[2];

            Controller.Start();

            this.NativeBitmapConverter = this.NativeBitmapConvert;
            this.NativeBitmapCopyToSpi = this.NativeBitmapSpi;

            GT.SocketInterfaces.I2CBusIndirector nativeI2C = (s, sdaPin, sclPin, address, clockRateKHz, module) => new InteropI2CBus(s, sdaPin, sclPin, address, clockRateKHz, module);
            GT.Socket socket;

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(1);
            socket.SupportedTypes = new char[] { 'H', 'I' };
            socket.CpuPins[3] = FEZCerberusPins.Socket1.Pin3;
            socket.CpuPins[4] = FEZCerberusPins.Socket1.Pin4;
            socket.CpuPins[5] = FEZCerberusPins.Socket1.Pin5;
            socket.CpuPins[6] = FEZCerberusPins.Socket1.Pin6;
            socket.CpuPins[8] = FEZCerberusPins.Socket1.Pin8;
            socket.CpuPins[9] = FEZCerberusPins.Socket1.Pin9;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(2);
            socket.SupportedTypes = new char[] { 'A', 'I', 'K', 'U', 'Y' };
            socket.CpuPins[3] = FEZCerberusPins.Socket2.Pin3;
            socket.CpuPins[4] = FEZCerberusPins.Socket2.Pin4;
            socket.CpuPins[5] = FEZCerberusPins.Socket2.Pin5;
            socket.CpuPins[6] = FEZCerberusPins.Socket2.Pin6;
            socket.CpuPins[7] = FEZCerberusPins.Socket2.Pin7;
            socket.CpuPins[8] = FEZCerberusPins.Socket2.Pin8;
            socket.CpuPins[9] = FEZCerberusPins.Socket2.Pin9;
            socket.I2CBusIndirector = nativeI2C;
            socket.SerialPortName = "COM2";
            socket.AnalogInput3 = Cpu.AnalogChannel.ANALOG_0;
            socket.AnalogInput4 = Cpu.AnalogChannel.ANALOG_1;
            socket.AnalogInput5 = Cpu.AnalogChannel.ANALOG_2;
            GT.Socket.SocketInterfaces.SetAnalogInputFactors(socket, 3.3, 0, 12);
            GT.Socket.SocketInterfaces.RegisterSocket(socket);

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(3);
            socket.SupportedTypes = new char[] { 'A', 'O', 'P', 'Y' };
            socket.CpuPins[3] = FEZCerberusPins.Socket3.Pin3;
            socket.CpuPins[4] = FEZCerberusPins.Socket3.Pin4;
            socket.CpuPins[5] = FEZCerberusPins.Socket3.Pin5;
            socket.CpuPins[6] = FEZCerberusPins.Socket3.Pin6;
            socket.CpuPins[7] = FEZCerberusPins.Socket3.Pin7;
            socket.CpuPins[8] = FEZCerberusPins.Socket3.Pin8;
            socket.CpuPins[9] = FEZCerberusPins.Socket3.Pin9;
            socket.I2CBusIndirector = nativeI2C;
            socket.PWM7 = Cpu.PWMChannel.PWM_0;
            socket.PWM8 = Cpu.PWMChannel.PWM_1;
            socket.PWM9 = Cpu.PWMChannel.PWM_2;
            socket.AnalogOutput5 = Cpu.AnalogOutputChannel.ANALOG_OUTPUT_0;
            socket.AnalogInput3 = Cpu.AnalogChannel.ANALOG_3;
            socket.AnalogInput4 = Cpu.AnalogChannel.ANALOG_4;
            socket.AnalogInput5 = Cpu.AnalogChannel.ANALOG_5;
            GT.Socket.SocketInterfaces.SetAnalogInputFactors(socket, 3.3, 0, 12);
            GT.Socket.SocketInterfaces.SetAnalogOutputFactors(socket, 3.3, 0, 12);
            GT.Socket.SocketInterfaces.RegisterSocket(socket);

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(4);
            socket.SupportedTypes = new char[] { 'A', 'O', 'P', 'Y' };
            socket.CpuPins[3] = FEZCerberusPins.Socket4.Pin3;
            socket.CpuPins[4] = FEZCerberusPins.Socket4.Pin4;
            socket.CpuPins[5] = FEZCerberusPins.Socket4.Pin5;
            socket.CpuPins[6] = FEZCerberusPins.Socket4.Pin6;
            socket.CpuPins[7] = FEZCerberusPins.Socket4.Pin7;
            socket.CpuPins[8] = FEZCerberusPins.Socket4.Pin8;
            socket.CpuPins[9] = FEZCerberusPins.Socket4.Pin9;
            socket.I2CBusIndirector = nativeI2C;
            socket.PWM7 = Cpu.PWMChannel.PWM_3;
            socket.PWM8 = Cpu.PWMChannel.PWM_4;
            socket.PWM9 = Cpu.PWMChannel.PWM_5;
            socket.AnalogOutput5 = Cpu.AnalogOutputChannel.ANALOG_OUTPUT_1;
            socket.AnalogInput3 = Cpu.AnalogChannel.ANALOG_6;
            socket.AnalogInput4 = Cpu.AnalogChannel.ANALOG_7;
            socket.AnalogInput5 = (Cpu.AnalogChannel)8;
            GT.Socket.SocketInterfaces.SetAnalogInputFactors(socket, 3.3, 0, 12);
            GT.Socket.SocketInterfaces.SetAnalogOutputFactors(socket, 3.3, 0, 12);
            GT.Socket.SocketInterfaces.RegisterSocket(socket);

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(5);
            socket.SupportedTypes = new char[] { 'P', 'C', 'S', 'X' };
            socket.CpuPins[3] = FEZCerberusPins.Socket5.Pin3;
            socket.CpuPins[4] = FEZCerberusPins.Socket5.Pin4;
            socket.CpuPins[5] = FEZCerberusPins.Socket5.Pin5;
            socket.CpuPins[6] = FEZCerberusPins.Socket5.Pin6;
            socket.CpuPins[7] = FEZCerberusPins.Socket5.Pin7;
            socket.CpuPins[8] = FEZCerberusPins.Socket5.Pin8;
            socket.CpuPins[9] = FEZCerberusPins.Socket5.Pin9;
            socket.SPIModule = SPI.SPI_module.SPI1;
            socket.PWM7 = Cpu.PWMChannel.PWM_6;
            socket.PWM8 = Cpu.PWMChannel.PWM_7;
            socket.PWM9 = (Cpu.PWMChannel)8;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(6);
            socket.SupportedTypes = new char[] { 'P', 'S', 'U', 'X' };
            socket.CpuPins[3] = FEZCerberusPins.Socket6.Pin3;
            socket.CpuPins[4] = FEZCerberusPins.Socket6.Pin4;
            socket.CpuPins[5] = FEZCerberusPins.Socket6.Pin5;
            socket.CpuPins[6] = FEZCerberusPins.Socket6.Pin6;
            socket.CpuPins[7] = FEZCerberusPins.Socket6.Pin7;
            socket.CpuPins[8] = FEZCerberusPins.Socket6.Pin8;
            socket.CpuPins[9] = FEZCerberusPins.Socket6.Pin9;
            socket.SerialPortName = "COM3";
            socket.SPIModule = SPI.SPI_module.SPI1;
            socket.PWM7 = Cpu.PWMChannel.PWM_6;
            socket.PWM8 = Cpu.PWMChannel.PWM_7;
            socket.PWM9 = (Cpu.PWMChannel)8;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(7);
            socket.SupportedTypes = new char[] { 'F', 'Y' };
            socket.CpuPins[3] = FEZCerberusPins.Socket7.Pin3;
            socket.CpuPins[4] = FEZCerberusPins.Socket7.Pin4;
            socket.CpuPins[5] = FEZCerberusPins.Socket7.Pin5;
            socket.CpuPins[6] = FEZCerberusPins.Socket7.Pin6;
            socket.CpuPins[7] = FEZCerberusPins.Socket7.Pin7;
            socket.CpuPins[8] = FEZCerberusPins.Socket7.Pin8;
            socket.CpuPins[9] = FEZCerberusPins.Socket7.Pin9;
            socket.I2CBusIndirector = nativeI2C;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(8);
            socket.SupportedTypes = new char[] { 'D', 'Z' };
            socket.CpuPins[3] = FEZCerberusPins.Socket8.Pin3;
            socket.CpuPins[4] = FEZCerberusPins.Socket8.Pin4;
            socket.CpuPins[5] = FEZCerberusPins.Socket8.Pin5;
            socket.CpuPins[6] = FEZCerberusPins.Socket8.Pin6;
            socket.CpuPins[7] = FEZCerberusPins.Socket8.Pin7;
            socket.CpuPins[8] = GT.Socket.UnnumberedPin;
            socket.CpuPins[9] = GT.Socket.UnnumberedPin;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
        }

        /// <summary>The storage device volume names supported by this mainboard.</summary>
        /// <returns>The volume names.</returns>
        public override string[] GetStorageDeviceVolumeNames() {
            return new string[] { "SD", "USB" };
        }

        /// <summary>Mounts the device with the given name.</summary>
        /// <param name="volumeName">The device to mount.</param>
        /// <returns>Whether or not the mount was successful.</returns>
        public override bool MountStorageDevice(string volumeName) {
            try {
                if (volumeName == "SD" && this.storageDevices[0] == null) {
                    this.storageDevices[0] = new SDCard();
                    this.storageDevices[0].Mount();

                    return true;
                }
                else if (volumeName == "USB" && this.storageDevices[1] == null) {
                    foreach (BaseDevice dev in Controller.GetConnectedDevices()) {
                        if (dev.GetType() == typeof(MassStorage)) {
                            this.storageDevices[1] = (MassStorage)dev;
                            this.storageDevices[1].Mount();

                            return true;
                        }
                    }
                }
            }
            catch {
            }

            return false;
        }

        /// <summary>Unmounts the device with the given name.</summary>
        /// <param name="volumeName">The device to unmount.</param>
        /// <returns>Whether or not the unmount was successful.</returns>
        public override bool UnmountStorageDevice(string volumeName) {
            if (volumeName == "SD" && this.storageDevices[0] != null) {
                this.storageDevices[0].Dispose();
                this.storageDevices[0] = null;
            }
            else if (volumeName == "USB" && this.storageDevices[1] != null) {
                this.storageDevices[1].Dispose();
                this.storageDevices[1] = null;
            }
            else {
                return false;
            }

            return true;
        }

        /// <summary>Ensures that the RGB socket pins are available by disabling the display controller if needed.</summary>
        public override void EnsureRgbSocketPinsAvailable() {
            throw new NotSupportedException();
        }

        /// <summary>Sets the state of the debug LED.</summary>
        /// <param name="on">The new state.</param>
        public override void SetDebugLED(bool on) {
            if (this.debugLed == null)
                this.debugLed = new OutputPort(FEZCerberusPins.DebugLed, on);

            this.debugLed.Write(on);
        }

        /// <summary>Sets the programming mode of the device.</summary>
        /// <param name="programmingInterface">The new programming mode.</param>
        public override void SetProgrammingMode(GT.Mainboard.ProgrammingInterface programmingInterface) {
            throw new NotSupportedException();
        }

        /// <summary>This performs post-initialization tasks for the mainboard. It is called by Gadgeteer.Program.Run and does not need to be called manually.</summary>
        public override void PostInit() {
        }

        /// <summary>
        /// Configure the onboard display controller to fulfil the requirements of a display using the RGB sockets. If doing this requires rebooting, then the method must reboot and not return. If
        /// there is no onboard display controller, then NotSupportedException must be thrown.
        /// </summary>
        /// <param name="displayModel">Display model name.</param>
        /// <param name="width">Display physical width in pixels, ignoring the orientation setting.</param>
        /// <param name="height">Display physical height in lines, ignoring the orientation setting.</param>
        /// <param name="orientationDeg">Display orientation in degrees.</param>
        /// <param name="timing">The required timings from an LCD controller.</param>
        protected override void OnOnboardControllerDisplayConnected(string displayModel, int width, int height, int orientationDeg, GTM.Module.DisplayModule.TimingRequirements timing) {
            throw new NotSupportedException();
        }

        private void NativeBitmapConvert(Bitmap bitmap, byte[] pixelBytes, GT.Mainboard.BPP bpp) {
            if (bpp != GT.Mainboard.BPP.BPP16_BGR_BE) throw new ArgumentOutOfRangeException("bpp", "Only BPP16_BGR_BE supported");

            GHI.Utilities.Bitmaps.Convert(bitmap, GHI.Utilities.Bitmaps.BitsPerPixel.BPP16_BGR_BE, pixelBytes);
        }

        private void NativeBitmapSpi(Bitmap bitmap, SPI.Configuration config, int xSrc, int ySrc, int width, int height, GT.Mainboard.BPP bpp) {
            if (bpp != GT.Mainboard.BPP.BPP16_BGR_BE) throw new ArgumentOutOfRangeException("bpp", "Only BPP16_BGR_BE supported");

            if (!this.configSet) {
                Display.Populate(Display.GHIDisplay.DisplayN18);
                Display.Bpp = GHI.Utilities.Bitmaps.BitsPerPixel.BPP16_BGR_BE;
                Display.ControlPin = Cpu.Pin.GPIO_NONE;
                Display.BacklightPin = Cpu.Pin.GPIO_NONE;
                Display.ResetPin = Cpu.Pin.GPIO_NONE;
                Display.ChipSelectPin = config.ChipSelect_Port;
                Display.SpiModule = config.SPI_mod;

                if ((bitmap.Width == 128 || bitmap.Width == 160) && (bitmap.Height == 128 || bitmap.Height == 160))
                    Display.CurrentRotation = bitmap.Width == 128 ? Display.Rotation.Normal : Display.Rotation.Clockwise90;

                Display.Save();

                this.configSet = true;
            }

            bitmap.Flush(xSrc, ySrc, width, height);
        }

        private class InteropI2CBus : GT.SocketInterfaces.I2CBus {
            private SoftwareI2CBus softwareBus;

            public override ushort Address { get; set; }

            public override int Timeout { get; set; }

            public override int ClockRateKHz { get; set; }

            public InteropI2CBus(GT.Socket socket, GT.Socket.Pin sdaPin, GT.Socket.Pin sclPin, ushort address, int clockRateKHz, GTM.Module module) {
                this.Address = address;
                this.ClockRateKHz = clockRateKHz;

                this.softwareBus = new SoftwareI2CBus(socket.CpuPins[(int)sclPin], socket.CpuPins[(int)sdaPin]);
            }

            public override void WriteRead(byte[] writeBuffer, int writeOffset, int writeLength, byte[] readBuffer, int readOffset, int readLength, out int numWritten, out int numRead) {
                this.softwareBus.WriteRead((byte)this.Address, writeBuffer, writeOffset, writeLength, readBuffer, readOffset, readLength, out numWritten, out numRead);
            }
        }
    }
}