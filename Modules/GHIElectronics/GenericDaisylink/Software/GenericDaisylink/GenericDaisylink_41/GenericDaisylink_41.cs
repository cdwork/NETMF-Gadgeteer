﻿using GTM = Gadgeteer.Modules;

namespace Gadgeteer.Modules.GHIElectronics
{
    /// <summary>
    /// A GenericDaisylink module for Microsoft .NET Gadgeteer
    /// </summary>
    public class GenericDaisylink : GTM.Module.DaisyLinkModule
    {
        private const byte GHI_DAISYLINK_MANUFACTURER = 0x10;
        private const byte GHI_DAISYLINK_TYPE_GENERIC = 0x01;
        private const byte GHI_DAISYLINK_VERSION_GENERIC = 0x01;

        // Note: A constructor summary is auto-generated by the doc builder.
        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public GenericDaisylink(int socketNumber)
            : base(socketNumber, GHI_DAISYLINK_MANUFACTURER, GHI_DAISYLINK_TYPE_GENERIC, GHI_DAISYLINK_VERSION_GENERIC, GHI_DAISYLINK_VERSION_GENERIC, 50, "Generic")
        {
            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);
        }

        /// <summary>
        /// Writes to the daisylink register specified by the address. Does not allow writing to the reserved registers.
        /// </summary>
        /// <param name="address">Address of the register.</param>
        /// <param name="writebuffer">Byte to write.</param>
        public void WriteRegister(byte address, byte writebuffer)
        {
            WriteParams((byte)(DaisyLinkOffset + address), (byte)writebuffer);
        }

        /// <summary>
        /// Reads a byte from the specified register. Allows reading of reserved registers.
        /// </summary>
        /// <param name="memoryaddress">Address of the register.</param>
        /// <returns></returns>
        public byte ReadRegister(byte memoryaddress)
        {
            return Read(memoryaddress);
        }
    }
}