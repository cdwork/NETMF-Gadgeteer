﻿
using System;
using Microsoft.SPOT;

using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.Interfaces;

using System.Threading;

namespace Gadgeteer.Modules.GHIElectronics
{
    // -- CHANGE FOR MICRO FRAMEWORK 4.2 --
    // If you want to use Serial, SPI, or DaisyLink (which includes GTI.SoftwareI2C), you must do a few more steps
    // since these have been moved to separate assemblies for NETMF 4.2 (to reduce the minimum memory footprint of Gadgeteer)
    // 1) add a reference to the assembly (named Gadgeteer.[interfacename])
    // 2) in GadgeteerHardware.xml, uncomment the lines under <Assemblies> so that end user apps using this module also add a reference.

    /// <summary>
    /// A High resolution color camera module with serial interface for Microsoft .NET Gadgeteer
    /// </summary>
    /// <example>
    /// <para>The following example uses a <see cref="SerialCameraL1"/> object to capture a picture when a button is pressed and display it on a display.</para>
    /// <code>
    /// using System;
    /// using System.Collections;
    /// using System.Threading;
    /// using Microsoft.SPOT;
    /// using Microsoft.SPOT.Presentation;
    /// using Microsoft.SPOT.Presentation.Controls;
    /// using Microsoft.SPOT.Presentation.Media;
    /// using Microsoft.SPOT.Touch;
    ///
    /// using Gadgeteer.Networking;
    /// using GT = Gadgeteer;
    /// using GTM = Gadgeteer.Modules;
    /// using Gadgeteer.Modules.GHIElectronics;
    ///
    /// namespace TestApp
    /// {
    ///     public partial class Program
    ///     {
    ///         void ProgramStarted()
    ///         {
    ///             sercam.SetImageSize(SerialCameraL1.Camera_Resolution.SIZE_QVGA);
    ///             sercam.PictureCaptured += new SerialCameraL1.PictureCapturedEventHandler(sercam_PictureCaptured);
    ///
    ///             button.ButtonPressed += new Button.ButtonEventHandler(button_ButtonPressed);
    ///
    ///             Debug.Print("Program Started");
    ///         }
    ///
    ///         void button_ButtonPressed(Button sender, Button.ButtonState state)
    ///         {
    ///             sercam.TakePicture();
    ///         }
    ///
    ///         void sercam_PictureCaptured(GTM.GHIElectronics.SerialCameraL1 sender, Bitmap picture)
    ///         {
    ///             Debug.GC(true);
    ///             display_T35.SimpleGraphics.DisplayImage(picture, 0, 0);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public class SerialCameraL1 : GTM.Module
    {
        private GTI.Serial serialPort;
        private const int CMD_DELAY_TIME = 50;//200;
        private const int RESET_DELAY_TIME = 1000;
        private const int POWERUP_DELAY_TIME = 2000;
        //const int BLOCK_SIZE = 128; 
        //private static byte[] dataJPG;

        /// <summary>
        /// List of the possible resolutions
        /// </summary>
        public enum Camera_Resolution
        {
            /// <summary>
            /// 640x480
            /// </summary>
            SIZE_VGA = 0x00,

            /// <summary>
            /// 320x240
            /// </summary>
            SIZE_QVGA = 0x11,

            /// <summary>
            /// 160x120
            /// </summary>
            SIZE_QQVGA = 0x22,
        };

        // Note: A constructor summary is auto-generated by the doc builder.
        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public SerialCameraL1(int socketNumber)
        {
            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);

            socket.EnsureTypeIsSupported('U', this);

            serialPort = new GTI.Serial(socket, 115200, GTI.Serial.SerialParity.None, GTI.Serial.SerialStopBits.One, 8, GTI.Serial.HardwareFlowControl.NotRequired, this);
            serialPort.ReadTimeout = 5000;
            serialPort.WriteTimeout = 5000;

            serialPort.Open();            
            ResetCamera();
        }

        // Baudrate
        //private static ushort BAUDRATE_9600 = 0xAEC8;
        //private static ushort BAUDRATE_19200 = 0x56E4;
        //private static ushort BAUDRATE_38400 = 0x2AF2;
        //private static ushort BAUDRATE_57600 = 0x1CC4;
        //private static ushort BAUDRATE_115200 = 0x0DA6;

        

        #region Picture Capture

        private Boolean ResetCamera()
        {
            byte[] send = new byte[] { 0x56, 0x00, 0x26, 0x00 };

            serialPort.Write(send, 0, send.Length);

            Thread.Sleep(RESET_DELAY_TIME); // following document, need to be sleep 1000ms after resetting.

            byte[] receive = ReadBytes();

            if (receive != null && receive.Length > 0)
            {
                return true;
            }
            return false;
        }
        private bool StopFrameBufferControl()
        {
            CleanSerialPort();

            byte[] send = new byte[] { 0x56, 0x00, 0x36, 0x01, 0x00 };
            serialPort.Write(send, 0, send.Length);

            Thread.Sleep(CMD_DELAY_TIME);

            byte[] receive = ReadBytes();

            if (receive != null && receive.Length >= 4)
            {
                if (receive[0] == 0x76 && receive[1] == 0 && receive[2] == 0x36 && receive[3] == 0 && receive[4] == 0)
                {
                    return true;
                }
            }

            return false;
        }
        private void ResumeToNextFrame()
        {
            CleanSerialPort();
            byte[] send = new byte[] { 0x56, 0x00, 0x36, 0x01, 0x03 };
            serialPort.Write(send, 0, send.Length);
            byte[] reply = new byte[5];
            ReadBytes(reply, 0, 5);

        }
        private int GetFrameBufferLength()
        {
            CleanSerialPort();

            byte[] send = new byte[] { 0x56, 0x00, 0x34, 0x01, 0x00 };

            serialPort.Write(send, 0, send.Length);

            Thread.Sleep(CMD_DELAY_TIME);

            int size = 0;
            byte[] receive = ReadBytes();

            if (receive != null && receive.Length >= 9)
            {
                if (receive[0] == 0x76 && receive[1] == 0 && receive[2] == 0x34 && receive[3] == 0 && receive[4] == 0x4)
                {
                    size = receive[5] << 24 | receive[6] << 16 | receive[7] << 8 | receive[8];
                }
            }
            return size;
        }

        
        private Boolean ReadFrameBuffer()
        {
          
            CleanSerialPort();
            //byte[] size4byte = new byte[4];
            byte[] header = new byte[5];
          
            //size4byte[0] = (byte)(dataSize >> 24);
            //size4byte[1] = (byte)(dataSize >> 16);
            //size4byte[2] = (byte)(dataSize >> 8);
            //size4byte[3] = (byte)(dataSize);

            byte[] send = new byte[] { 0x56, 0x00, 0x32, 0x0C, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x00, (byte)(dataSize >> 24), (byte)(dataSize >> 16), (byte)(dataSize >> 8), (byte)(dataSize), 0x10, 0x00 };
            serialPort.Write(send, 0, send.Length);
            Thread.Sleep(10);
            try
            {
                // Read header
                ReadBytes(header, 0, 5);

                // Read rawdata
                ReadBytes(dataImage, 0, dataSize);

                // read header
                ReadBytes(header, 0, 5);
            }
            catch
            {
                dataImage = null;               
                return false;
            }
            // check correct data
            if (dataImage[0] == 0xFF && dataImage[1] == 0xD8 && dataImage[dataSize - 2] == 0xFF && dataImage[dataSize - 1] == 0xD9) // make sure this is JPeg format
            {
                return true;
            }
            else
            {
                dataImage = null;
                return false;
            }
            
        }
        
       
        /// <summary>
        /// Sets the image size
        /// </summary>
        /// <param name="resolution">The resolution to set the image size to</param>
        /// <returns>If setting the parameter was successful</returns>
        public bool SetImageSize(Camera_Resolution resolution)
        {
            byte[] send = new byte[] { 0x56, 0x00, 0x31, 0x05, 0x04, 0x01, 0x00, 0x19, (byte)resolution };

            serialPort.Write(send, 0, send.Length);

            Thread.Sleep(CMD_DELAY_TIME);

            byte[] receive = ReadBytes();
            if (receive != null && receive[0] == 0x76 && receive[1] == 0x00 && receive[2] == 0x31 && receive[3] == 0x00 && receive[4] == 0x00)
            {
                return true;
            }

            return false;
        }

		/// <summary>
		/// Sets the ratio.
		/// </summary>
		/// <param name="ratio">The ratio.</param>
		/// <returns>Whether it was successful or not.</returns>
        public Boolean SetRatio(byte ratio)
        {
            CleanSerialPort();
            byte[] send = new byte[] { 0x56, 0x00, 0x31, 0x05, 0x04, 0x01, 0x00, 0x1A, ratio };
            serialPort.Write(send, 0, send.Length);
            Thread.Sleep(CMD_DELAY_TIME);
            byte[] receive = ReadBytes();
            if (receive != null && receive.Length >= 5)
            {
                if (receive[0] == 0x76 && receive[1] == 0 && receive[2] == 0x31 && receive[3] == 0 && receive[4] == 0x0)
                {
                    ResetCamera();
                    return true;
                }
            }
            return false;
        }
        #endregion



        #region Serial function

        private void CleanSerialPort()
        {
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        private byte[] ReadBytes()
        {
            byte[] data = null;
            int byteread = serialPort.BytesToRead;
            if (byteread > 0)
            {
                data = new byte[byteread];
                ReadBytes(data, 0, data.Length);
            }
            return data;
        }
        private void ReadBytes(byte[] data, int offset, int count)
        {
            //DateTime startTime = DateTime.Now;
            do
            {
				int bytesRead = serialPort.Read(data, offset, count);
                offset += bytesRead;
                count -= bytesRead;

                //if ((DateTime.Now - startTime).Milliseconds > serialPort.ReadTimeout)
                //{
                    
                //   throw new Exception();
                //}
				if (bytesRead == 0)
				{
					serialPort.DiscardInBuffer();
					break;
				}
				Thread.Sleep(1);
            } while (count > 0);

            if (count != 0) throw new Exception();
           
        }
        #endregion

		/// <summary>
		/// Whether or not a new image is ready.
		/// </summary>
        public Boolean isNewImageReady = false;
        private Boolean isUpdateStreaming = false;
        private Boolean isPause = false;

		/// <summary>
		/// The image data.
		/// </summary>
        public byte[] dataImage;
        private int dataSize;
        private Thread ThreadUpdateStreaming;

        #region User Control
		/// <summary>
		/// Starts streaming from the device.
		/// </summary>
        public void StartStreaming()
        {
            StopStreaming();
            isUpdateStreaming = true;
            isNewImageReady = false;
            isPause = false;
            ThreadUpdateStreaming = new Thread(UpdateStreaming);
            ThreadUpdateStreaming.Start();
            
                      
        }
        //int Debug_cnt = 0;
        private void UpdateStreaming()
        {
            while (isUpdateStreaming)            
            {
                try
                {
                    if (isPause) continue; // incase user need to pause
                    StopFrameBufferControl();
                    dataSize = GetFrameBufferLength();
                    if (dataSize > 0)
                    {
                        isNewImageReady = false;
                        dataImage = null; // Optimize for Cerberus
                        dataImage = new byte[dataSize];
                        isNewImageReady = ReadFrameBuffer();
                        if (isNewImageReady == false)
                        {
                            ResetCamera(); // Reset SerialCameraL1
                        }
                        else
                        {
                            ResumeToNextFrame(); // capture next 
                        }
                    }
                }
                catch
                {
                    // should never go here
                    isNewImageReady = false;
                    dataImage = null;
                    dataSize = 0;
                    Debug.GC(true);
                    ResetCamera(); // Reset SerialCameraL1
                }
                Thread.Sleep(CMD_DELAY_TIME); // it is better to delay some here
                
            }
        }

		/// <summary>
		/// Stops streaming from the device.
		/// </summary>
        public void StopStreaming()
        {
            isUpdateStreaming = false;
            isNewImageReady = false;
            if (ThreadUpdateStreaming != null) // wait for thread is stopped.
            {
                while (ThreadUpdateStreaming.IsAlive)
                {
                    Thread.Sleep(CMD_DELAY_TIME);
                }
                ThreadUpdateStreaming = null;
            }
        }

		/// <summary>
		/// Returns the image data.
		/// </summary>
		/// <returns>The image data.</returns>
        public byte[] GetImageData()
        {
            if (isNewImageReady && (dataImage != null && dataImage.Length > 0))
            {
                isNewImageReady = false;
                return dataImage;
            }
            return null;

        }

		/// <summary>
		/// Draws a bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap to draw.</param>
		/// <param name="xBitmap">The x coordinate to draw at.</param>
		/// <param name="yBitmap">The y coordinate to draw at.</param>
		/// <param name="width">The width of the image to draw.</param>
		/// <param name="height">The height of the image to draw.</param>
        public void DrawImage(Bitmap bitmap, int xBitmap, int yBitmap, int width, int height)
        {
            if (isNewImageReady && (dataImage != null && dataImage.Length > 0))
            {
                isNewImageReady = false;
                PauseStreaming();
                Bitmap image = new Bitmap(dataImage, Bitmap.BitmapImageType.Jpeg);               
                bitmap.DrawImage(xBitmap, yBitmap, image, 0, 0, width, height);
                ResumeStreaming();
            }
        }

		/// <summary>
		/// Pauses streaming from the device.
		/// </summary>
        public void PauseStreaming()
        {
            isPause = true;            
        }

		/// <summary>
		/// Resumes streaming from the device.
		/// </summary>
        public void ResumeStreaming()
        {
            isPause = false;
        }
        #endregion

    }
}
