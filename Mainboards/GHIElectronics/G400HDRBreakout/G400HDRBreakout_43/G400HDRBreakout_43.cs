﻿using GHI.IO;
using GHI.IO.Storage;
using GHI.Processor;
using GHI.Usb;
using GHI.Usb.Host;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using G400 = GHI.Pins.G400;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace GHIElectronics.Gadgeteer
{
    /// <summary>
    /// The mainboard class for the G400HDR Breakout.
    /// </summary>
    public class G400HDRBreakout : GT.Mainboard
    {
        private OutputPort debugLed;
        private Removable[] storageDevices;
        private Device usbMassStorageDevice;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public G400HDRBreakout()
        {
            this.debugLed = null;
            this.storageDevices = new Removable[3];
            this.usbMassStorageDevice = null;

            Controller.DeviceConnected += (a, b) =>
            {
                if (b.Device.Type == GHI.Usb.Device.DeviceType.MassStorage)
                {
                    this.usbMassStorageDevice = b.Device;
                    this.usbMassStorageDevice.Disconnected += (c, d) => this.UnmountStorageDevice("USB MassStorage");
                }
            };

            this.NativeBitmapConverter = this.BitmapConverter;

            GT.SocketInterfaces.I2CBusIndirector nativeI2C = (s, sdaPin, sclPin, address, clockRateKHz, module) => new InteropI2CBus(s, sdaPin, sclPin, address, clockRateKHz, module);
            GT.Socket socket;


            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(1);
            socket.SupportedTypes = new char[] { 'R', 'Y' };
            socket.CpuPins[3] = G400.PC11;
            socket.CpuPins[4] = G400.PC12;
            socket.CpuPins[5] = G400.PC13;
            socket.CpuPins[6] = G400.PC14;
            socket.CpuPins[7] = G400.PC15;
            socket.CpuPins[8] = G400.PC27;
            socket.CpuPins[9] = G400.PC28;
            socket.I2CBusIndirector = nativeI2C;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            
            
            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(2);
            socket.SupportedTypes = new char[] { 'G', 'Y' };
            socket.CpuPins[3] = G400.PC5;  
            socket.CpuPins[4] = G400.PC6;  
            socket.CpuPins[5] = G400.PC7;  
            socket.CpuPins[6] = G400.PC8;  
            socket.CpuPins[7] = G400.PC9;  
            socket.CpuPins[8] = G400.PC10; 
            socket.CpuPins[9] = G400.PA28; 
            socket.I2CBusIndirector = nativeI2C;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(3);
            socket.SupportedTypes = new char[] { 'B', 'Y' };
            socket.CpuPins[3] = G400.PC0; 
            socket.CpuPins[4] = G400.PC1; 
            socket.CpuPins[5] = G400.PC2; 
            socket.CpuPins[6] = G400.PC3; 
            socket.CpuPins[7] = G400.PC4; 
            socket.CpuPins[8] = G400.PC29;
            socket.CpuPins[9] = G400.PC30;
            socket.I2CBusIndirector = nativeI2C;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(4);
            socket.SupportedTypes = new char[] { 'D', 'I' };
            socket.CpuPins[3] = G400.PD5;
            socket.CpuPins[4] = (Cpu.Pin)SpecialPurposePin.USBD_A_DM;
            socket.CpuPins[5] = (Cpu.Pin)SpecialPurposePin.USBD_A_DP;
            socket.CpuPins[6] = G400.PA27;
            socket.CpuPins[7] = G400.PA29;
            socket.CpuPins[8] = G400.PA30;
            socket.CpuPins[9] = G400.PA31;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(5);
            socket.SupportedTypes = new char[] { 'Z' };
            socket.CpuPins[3] = (Cpu.Pin)SpecialPurposePin.RESET;   
            socket.CpuPins[4] = (Cpu.Pin)SpecialPurposePin.TCK;     
            socket.CpuPins[5] = (Cpu.Pin)SpecialPurposePin.RTC_BATT;
            socket.CpuPins[6] = (Cpu.Pin)SpecialPurposePin.TDO;     
            socket.CpuPins[7] = (Cpu.Pin)SpecialPurposePin.TRST;    
            socket.CpuPins[8] = (Cpu.Pin)SpecialPurposePin.TMS;     
            socket.CpuPins[9] = (Cpu.Pin)SpecialPurposePin.TDI;     
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(6);
            socket.SupportedTypes = new char[] { 'S', 'U', 'Y' };
            socket.CpuPins[3] = G400.PC31;
            socket.CpuPins[4] = G400.PA5; 
            socket.CpuPins[5] = G400.PA6; 
            socket.CpuPins[6] = G400.PC22;
            socket.CpuPins[7] = G400.PA22;
            socket.CpuPins[8] = G400.PA21;
            socket.CpuPins[9] = G400.PA23;
            socket.SPIModule = SPI.SPI_module.SPI1;
            socket.SerialPortName = "COM3";
            socket.I2CBusIndirector = nativeI2C;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            
            
            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(7);
            socket.SupportedTypes = new char[] { 'I', 'U', 'X' };
            socket.CpuPins[3] = G400.PA26;
            socket.CpuPins[4] = G400.PA10;
            socket.CpuPins[5] = G400.PA9; 
            socket.CpuPins[6] = G400.PB12;
            socket.CpuPins[8] = G400.PA30;
            socket.CpuPins[9] = G400.PA31;
            socket.SerialPortName = "COM1";
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(8);
            socket.SupportedTypes = new char[] { 'A', 'I', 'X' };
            socket.CpuPins[3] = G400.PB14;
            socket.CpuPins[4] = G400.PB15;
            socket.CpuPins[5] = G400.PB16;
            socket.CpuPins[6] = G400.PB17;
            socket.CpuPins[8] = G400.PA30;
            socket.CpuPins[9] = G400.PA31;
            socket.AnalogInput3 = Cpu.AnalogChannel.ANALOG_3;
            socket.AnalogInput4 = Cpu.AnalogChannel.ANALOG_4;
            socket.AnalogInput5 = Cpu.AnalogChannel.ANALOG_5;
            GT.Socket.SocketInterfaces.SetAnalogInputFactors(socket, 3.3, 0, 10);
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(9);
            socket.SupportedTypes = new char[] { 'P', 'U', 'Y' };
            socket.CpuPins[3] = G400.PB18;
            socket.CpuPins[4] = G400.PA7; 
            socket.CpuPins[5] = G400.PA8; 
            socket.CpuPins[6] = G400.PC26;
            socket.CpuPins[7] = G400.PC18;
            socket.CpuPins[8] = G400.PC21;
            socket.CpuPins[9] = G400.PC20;
            socket.PWM7 = Cpu.PWMChannel.PWM_0;
            socket.PWM8 = Cpu.PWMChannel.PWM_3;
            socket.PWM9 = Cpu.PWMChannel.PWM_2;
            socket.SerialPortName = "COM4";
            socket.I2CBusIndirector = nativeI2C;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
            

            socket = GT.Socket.SocketInterfaces.CreateNumberedSocket(10);
            socket.SupportedTypes = new char[] { 'F' };
            socket.CpuPins[3] = G400.PB8; 
            socket.CpuPins[4] = G400.PA15;
            socket.CpuPins[5] = G400.PA18;
            socket.CpuPins[6] = G400.PA16;
            socket.CpuPins[7] = G400.PA19;
            socket.CpuPins[8] = G400.PA20;
            socket.CpuPins[9] = G400.PA17;
            GT.Socket.SocketInterfaces.RegisterSocket(socket);
        }

        /// <summary>
        /// The name of the mainboard.
        /// </summary>
        public override string MainboardName
        {
            get { return "GHI Electronics G400HDR Breakout"; }
        }

        /// <summary>
        /// The current version of the mainboard hardware.
        /// </summary>
        public override string MainboardVersion
        {
            get { return "1.3"; }
        }

        /// <summary>
        /// The storage device volume names supported by this mainboard.
        /// </summary>
        /// <returns>The volume names.</returns>
        public override string[] GetStorageDeviceVolumeNames()
        {
            return new string[] { "SD", "SD SPI", "USB MassStorage" };
        }

        /// <summary>
        /// Mounts the device with the given name.
        /// </summary>
        /// <param name="volumeName">The device to mount.</param>
        /// <returns>Whether or not the mount was successful.</returns>
        public override bool MountStorageDevice(string volumeName)
        {
            switch (volumeName)
            {
                case "SD":
                    this.storageDevices[0] = new SD(SD.SDInterface.MCI);
                    this.storageDevices[0].Mount();

                    break;

                case "SD SPI":
                    this.storageDevices[1] = new SD(SD.SDInterface.SPI);
                    this.storageDevices[1].Mount();

                    break;

                case "USB MassStorage":
                    if (this.usbMassStorageDevice == null) throw new InvalidOperationException("No USB MassStorage device is plugged into the device.");

                    this.storageDevices[2] = new UsbMassStorage(this.usbMassStorageDevice);
                    this.storageDevices[2].Mount();

                    break;

                default:
                    throw new ArgumentException("volumeName", "volumeName must be present in the array returned by GetStorageDeviceVolumeNames.");
            }

            return true;
        }

        /// <summary>
        /// Unmounts the device with the given name.
        /// </summary>
        /// <param name="volumeName">The device to unmount.</param>
        /// <returns>Whether or not the unmount was successful.</returns>
        public override bool UnmountStorageDevice(string volumeName)
        {
            switch (volumeName)
            {
                case "SD":
                    if (this.storageDevices[0] == null) return false;

                    this.storageDevices[0].Unmount();
                    this.storageDevices[0].Dispose();
                    this.storageDevices[0] = null;

                    break;

                case "SD SPI":
                    if (this.storageDevices[1] == null) return false;

                    this.storageDevices[1].Unmount();
                    this.storageDevices[1].Dispose();
                    this.storageDevices[1] = null;

                    break;

                case "USB MassStorage":
                    if (this.storageDevices[2] == null) return false;

                    this.storageDevices[2].Unmount();
                    this.storageDevices[2].Dispose();
                    this.storageDevices[2] = null;

                    break;

                default:
                    throw new ArgumentException("volumeName", "volumeName must be present in the array returned by GetStorageDeviceVolumeNames.");
            }

            return true;
        }

        /// <summary>
        /// Configure the onboard display controller to fulfil the requirements of a display using the RGB sockets.
        /// If doing this requires rebooting, then the method must reboot and not return.
        /// If there is no onboard display controller, then NotSupportedException must be thrown.
        /// </summary>
        /// <param name="displayModel">Display model name.</param>
        /// <param name="width">Display physical width in pixels, ignoring the orientation setting.</param>
        /// <param name="height">Display physical height in lines, ignoring the orientation setting.</param>
        /// <param name="orientationDeg">Display orientation in degrees.</param>
        /// <param name="timing">The required timings from an LCD controller.</param>
        protected override void OnOnboardControllerDisplayConnected(string displayModel, int width, int height, int orientationDeg, GTM.Module.DisplayModule.TimingRequirements timing)
        {
            Configuration.Display.Height = (uint)height;
            Configuration.Display.HorizontalBackPorch = timing.HorizontalBackPorch;
            Configuration.Display.HorizontalFrontPorch = timing.HorizontalFrontPorch;
            Configuration.Display.HorizontalSyncPolarity = timing.HorizontalSyncPulseIsActiveHigh;
            Configuration.Display.HorizontalSyncPulseWidth = timing.HorizontalSyncPulseWidth;
            Configuration.Display.OutputEnableIsFixed = timing.UsesCommonSyncPin; //not the proper property, but we needed it;
            Configuration.Display.OutputEnablePolarity = timing.CommonSyncPinIsActiveHigh; //not the proper property, but we needed it;
            Configuration.Display.PixelClockRateKHz = timing.MaximumClockSpeed;
            Configuration.Display.PixelPolarity = timing.PixelDataIsValidOnClockRisingEdge;
            Configuration.Display.VerticalBackPorch = timing.VerticalBackPorch;
            Configuration.Display.VerticalFrontPorch = timing.VerticalFrontPorch;
            Configuration.Display.VerticalSyncPolarity = timing.VerticalSyncPulseIsActiveHigh;
            Configuration.Display.VerticalSyncPulseWidth = timing.VerticalSyncPulseWidth;
            Configuration.Display.Width = (uint)width;

            if (Configuration.Display.Save())
            {
                Debug.Print("Updating display configuration. THE MAINBOARD WILL NOW REBOOT.");
                Debug.Print("To continue debugging, you will need to restart debugging manually (Ctrl-Shift-F5)");

                Microsoft.SPOT.Hardware.PowerState.RebootDevice(false);
            }

            switch (orientationDeg)
            {
                case 0: Configuration.Display.CurrentRotation = Configuration.Display.Rotation.Normal; break;
                case 90: Configuration.Display.CurrentRotation = Configuration.Display.Rotation.Clockwise90; break;
                case 180: Configuration.Display.CurrentRotation = Configuration.Display.Rotation.Half; break;
                case 270: Configuration.Display.CurrentRotation = Configuration.Display.Rotation.CounterClockwise90; break;
                default: throw new ArgumentOutOfRangeException("orientationDeg", "orientationDeg must be 0, 90, 180, or 270.");
            }
        }

        /// <summary>
        /// Ensures that the RGB socket pins are available by disabling the display controller if needed.
        /// </summary>
        public override void EnsureRgbSocketPinsAvailable()
        {
            if (Configuration.Display.Disable())
            {
                Debug.Print("Updating display configuration. THE MAINBOARD WILL NOW REBOOT.");
                Debug.Print("To continue debugging, you will need to restart debugging manually (Ctrl-Shift-F5)");

                Microsoft.SPOT.Hardware.PowerState.RebootDevice(false);
            }
        }

        /// <summary>
        /// Sets the state of the debug LED.
        /// </summary>
        /// <param name="on">The new state.</param>
        public override void SetDebugLED(bool on)
        {
            if (this.debugLed == null)
                this.debugLed = new OutputPort(G400.PD3, on);

            this.debugLed.Write(on);
        }

        /// <summary>
        /// Sets the programming mode of the device.
        /// </summary>
        /// <param name="programmingInterface">The new programming mode.</param>
        public override void SetProgrammingMode(GT.Mainboard.ProgrammingInterface programmingInterface)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This performs post-initialization tasks for the mainboard.  It is called by Gadgeteer.Program.Run and does not need to be called manually.
        /// </summary>
        public override void PostInit()
        {

        }

        private void BitmapConverter(Bitmap bitmap, byte[] pixelBytes, GT.Mainboard.BPP bpp)
        {
            if (bpp != GT.Mainboard.BPP.BPP16_BGR_BE) throw new ArgumentOutOfRangeException("bpp", "Only BPP16_BGR_BE supported");

            GHI.Utilities.Bitmaps.Convert(bitmap, GHI.Utilities.Bitmaps.BitsPerPixel.BPP16_BGR_BE, pixelBytes);
        }

        private class InteropI2CBus : GT.SocketInterfaces.I2CBus
        {
            public override ushort Address { get; set; }
            public override int Timeout { get; set; }
            public override int ClockRateKHz { get; set; }

            private SoftwareI2CBus softwareBus;

            public InteropI2CBus(GT.Socket socket, GT.Socket.Pin sdaPin, GT.Socket.Pin sclPin, ushort address, int clockRateKHz, GTM.Module module)
            {
                this.Address = address;
                this.ClockRateKHz = clockRateKHz;

                this.softwareBus = new SoftwareI2CBus(socket.CpuPins[(int)sclPin], socket.CpuPins[(int)sdaPin]);
            }

            public override void WriteRead(byte[] writeBuffer, int writeOffset, int writeLength, byte[] readBuffer, int readOffset, int readLength, out int numWritten, out int numRead)
            {
                this.softwareBus.WriteRead((byte)this.Address, writeBuffer, writeOffset, writeLength, readBuffer, readOffset, readLength, out numWritten, out numRead);
            }
        }

        private enum SpecialPurposePin
        {
            USBD_A_DM = -9,
            USBD_A_DP = -10,
            USBD_B_DM = -11,
            USBD_B_DP = -12,
            USBD_C_DM = -13,
            USBD_C_DP = -14,
            RTC_BATT = -15,
            RESET = -16,
            TCK = -19,
            TDO = -20,
            TMS = -21,
            TRST = -22,
            TDI = -23,
        }
    }
}