﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PacketTester
{
    public class ImuFrame
    {
        public byte ImuId = 0;
        public float Quaternion_x;
        public float Quaternion_y;
        public float Quaternion_z;
        public float Quaternion_w;
        public Int16 Magnetic_x;
        public Int16 Magnetic_y;
        public Int16 Magnetic_z;
        public Int16 Acceleration_x;
        public Int16 Acceleration_y;
        public Int16 Acceleration_z;
        public Int16 Rotation_x;
        public Int16 Rotation_y;
        public Int16 Rotation_z;
        public byte frameStatus; 

        public ImuFrame()
        {
            ImuId = 0; //this is the IMU ID
            Quaternion_x = 0.0F;
            Quaternion_y = 0.0F;
            Quaternion_z = 0.0F;
            Quaternion_w = 0.0F;
            Magnetic_x = 0;
            Magnetic_y = 0;
            Magnetic_z = 0;
            Acceleration_x = 0;
            Acceleration_y = 0;
            Acceleration_z = 0;
            Rotation_x = 0;
            Rotation_y = 0;
            Rotation_z = 0;
        }
        public ImuFrame(RawPacket packet)
        {
            if (packet.PayloadSize >= 37)
            {
                ImuId = packet.Payload[2]; //this is the IMU ID
                Quaternion_x = BitConverter.ToSingle(packet.Payload, 0+3);
                Quaternion_y = BitConverter.ToSingle(packet.Payload, 4 + 3);
                Quaternion_z = BitConverter.ToSingle(packet.Payload, 8 + 3);
                Quaternion_w = BitConverter.ToSingle(packet.Payload, 12 + 3);
                Magnetic_x = BitConverter.ToInt16(packet.Payload, 16 + 3);
                Magnetic_y = BitConverter.ToInt16(packet.Payload, 18 + 3);
                Magnetic_z = BitConverter.ToInt16(packet.Payload, 20 + 3);
                Acceleration_x = BitConverter.ToInt16(packet.Payload, 22 + 3);
                Acceleration_y = BitConverter.ToInt16(packet.Payload, 24 + 3);
                Acceleration_z = BitConverter.ToInt16(packet.Payload, 26 + 3);
                Rotation_x = BitConverter.ToInt16(packet.Payload, 28 + 3);
                Rotation_y = BitConverter.ToInt16(packet.Payload, 30 + 3);
                Rotation_z = BitConverter.ToInt16(packet.Payload, 32 + 3);
                if (packet.PayloadSize >= 38)
                {
                    frameStatus = packet.Payload[37];
                }
            }
        }
        private string swapHexBytes(string hexString)
        {
            return hexString[2].ToString() + hexString[3].ToString() + hexString[0].ToString() + hexString[1].ToString();
        }
        public string getQuaternionString()
        {
            return Quaternion_x.ToString("F4") + ";" + Quaternion_y.ToString("F4") + ";" + Quaternion_z.ToString("F4") + ";" + Quaternion_w.ToString("F4");
        }

        public string getAsciiString()
        {            
            //Quaternion_x is heading/yaw
            Int16 yaw = (Int16)(Quaternion_x * 8192*-1);
            //Quaternion_y is pitch
            Int16 pitch = (Int16)(Quaternion_y * 8192 * -1);
            //Quaternion_z is roll
            Int16 roll = 0;
            if (ImuId == 0)
            {
                roll = (Int16)(Quaternion_z * 8192 * -1);
            }
            else
            {
                roll = (Int16)(Quaternion_z * 8192);
            }
            string yawStr = yaw.ToString("X4");
            string pitchStr = pitch.ToString("X4");
            string rollStr = roll.ToString("X4");

            return swapHexBytes(pitchStr) + ";" + swapHexBytes(rollStr) + ";" + swapHexBytes(yawStr);

        }
        public string getCsvString()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(String.Format("{0:F4},{1:F4},{2:F4},{3:F4},", Quaternion_x, Quaternion_y, Quaternion_z, Quaternion_w));
            strBuilder.Append(String.Format("{0},{1},{2},", Magnetic_x, Magnetic_y, Magnetic_z));
            strBuilder.Append(String.Format("{0},{1},{2},", Acceleration_x, Acceleration_y, Acceleration_z));
            strBuilder.Append(String.Format("{0},{1},{2}\r\n", Rotation_x, Rotation_y, Rotation_z));
            return strBuilder.ToString();
        }
        public bool ParseImuFrame(RawPacket packet, byte expectedId)
        {
            bool retVal = false; 
            if (packet.Payload[1] == 0x13)
            {
                if (packet.PayloadSize >= 37)
                {
                    if (expectedId == packet.Payload[2])
                    {
                        ImuId = packet.Payload[2]; //this is the IMU ID
                        Quaternion_x = BitConverter.ToSingle(packet.Payload, 0 + 3);
                        Quaternion_y = BitConverter.ToSingle(packet.Payload, 4 + 3);
                        Quaternion_z = BitConverter.ToSingle(packet.Payload, 8 + 3);
                        Quaternion_w = BitConverter.ToSingle(packet.Payload, 12 + 3);
                        Magnetic_x = BitConverter.ToInt16(packet.Payload, 16 + 3);
                        Magnetic_y = BitConverter.ToInt16(packet.Payload, 18 + 3);
                        Magnetic_z = BitConverter.ToInt16(packet.Payload, 20 + 3);
                        Acceleration_x = BitConverter.ToInt16(packet.Payload, 22 + 3);
                        Acceleration_y = BitConverter.ToInt16(packet.Payload, 24 + 3);
                        Acceleration_z = BitConverter.ToInt16(packet.Payload, 26 + 3);
                        Rotation_x = BitConverter.ToInt16(packet.Payload, 28 + 3);
                        Rotation_y = BitConverter.ToInt16(packet.Payload, 30 + 3);
                        Rotation_z = BitConverter.ToInt16(packet.Payload, 32 + 3);
                        //validate the quaternion data
                        if (Math.Abs(Quaternion_x) > 1 || Math.Abs(Quaternion_y) > 1 || Math.Abs(Quaternion_z) > 1 || Math.Abs(Quaternion_z) > 1)
                        {
                            retVal = false; 
                        }
                        if (packet.PayloadSize >= 38)
                        {
                            frameStatus = packet.Payload[37];
                        }
                        retVal = true;
                    }
                }
            }
            return retVal;
        }
        public bool ParseDataFromFullFrame(RawPacket packet, int offset, int expectedId)
        {
            bool retVal = false;
            if (packet.Payload[offset] == (byte) expectedId)
            {
                ImuId = packet.Payload[offset]; //this is the IMU ID
                Quaternion_x = BitConverter.ToSingle(packet.Payload, offset + 0 + 1);   // 1 byte is for sensor Id
                Quaternion_y = BitConverter.ToSingle(packet.Payload, offset + 4 + 1);
                Quaternion_z = BitConverter.ToSingle(packet.Payload, offset + 8 + 1);
                Quaternion_w = BitConverter.ToSingle(packet.Payload, offset + 12 + 1);
                Magnetic_x = BitConverter.ToInt16(packet.Payload, offset + 16 + 1);
                Magnetic_y = BitConverter.ToInt16(packet.Payload, offset + 18 + 1);
                Magnetic_z = BitConverter.ToInt16(packet.Payload, offset + 20 + 1);
                Acceleration_x = BitConverter.ToInt16(packet.Payload, offset + 22 + 1);
                Acceleration_y = BitConverter.ToInt16(packet.Payload, offset + 24 + 1);
                Acceleration_z = BitConverter.ToInt16(packet.Payload, offset + 26 + 1);
                Rotation_x = BitConverter.ToInt16(packet.Payload, offset + 28 + 1);
                Rotation_y = BitConverter.ToInt16(packet.Payload, offset + 30 + 1);
                Rotation_z = BitConverter.ToInt16(packet.Payload, offset + 32 + 1);
                retVal = true;
            }
            return retVal;
        }
        override public string ToString()
        {
            //return this.roll.ToString("F6") + ";0.0;0.0";
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(String.Format("Qx{0}:Qy{1}:Qz{2}:Qw{3}\r\n", Quaternion_x, Quaternion_y, Quaternion_z, Quaternion_w));
            strBuilder.Append(String.Format("Mx{0}:My{1}:Mz{2}\r\n", Magnetic_x, Magnetic_y, Magnetic_z));
            strBuilder.Append(String.Format("Ax{0}:Ay{1}:Az{2}\r\n", Acceleration_x, Acceleration_y, Acceleration_z));
            strBuilder.Append(String.Format("Rx{0}:Ry{1}:Rz{2}\r\n", Rotation_x, Rotation_y, Rotation_z));
            return strBuilder.ToString();
        }
        public void serialize(ref MemoryStream serializerStream)
        {
            IFormatter formatter = new BinaryFormatter();
            serializerStream.Write(BitConverter.GetBytes(this.ImuId), 0, 1);
            serializerStream.Write(BitConverter.GetBytes(this.Quaternion_x), 0, 4);
            serializerStream.Write(BitConverter.GetBytes(this.Quaternion_y), 0, 4);
            serializerStream.Write(BitConverter.GetBytes(this.Quaternion_z), 0, 4);
            serializerStream.Write(BitConverter.GetBytes(this.Quaternion_w), 0, 4);
            serializerStream.Write(BitConverter.GetBytes(this.Magnetic_x), 0, 2);
            serializerStream.Write(BitConverter.GetBytes(this.Magnetic_y), 0, 2);
            serializerStream.Write(BitConverter.GetBytes(this.Magnetic_z), 0, 2);
            serializerStream.Write(BitConverter.GetBytes(this.Acceleration_x), 0, 2);
            serializerStream.Write(BitConverter.GetBytes(this.Acceleration_y), 0, 2);
            serializerStream.Write(BitConverter.GetBytes(this.Acceleration_z), 0, 2);
            serializerStream.Write(BitConverter.GetBytes(this.Rotation_x), 0, 2);
            serializerStream.Write(BitConverter.GetBytes(this.Rotation_y), 0, 2);
            serializerStream.Write(BitConverter.GetBytes(this.Rotation_z), 0, 2);
        }


    }
}
