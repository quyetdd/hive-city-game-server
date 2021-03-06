﻿using System.IO;
using System.Xml.Serialization;
using BEPUutilities;
using HiveCity.Server.Proxy.Common.MessageObjects;
using System;

namespace HiveCity.Server.Region.Models
{
    public class Position : IEquatable<Position>
    {
        public Vector3 Translation { get; set; }
        public float X { get { return Translation.X; } }
        public float Y { get { return Translation.Y; } }
        public float Z { get { return Translation.Z; } }

        public short Heading { get; set; } // 0 - 65535  0.00549 deg per int value (shorten mem space)

        public Position() : this(0, 0, 0, 0) { }

        public Position(float x, float y, float z)
            : this(x, y, z, 0)
        { }

        public Position(float x, float y, float z, short heading)
        {
            XYZ(x, y, z);
            Heading = heading;
        }

        public void XYZ(float x, float y, float z)
        {
            Translation = new Vector3(x, y, z);

        }

        // call constructer of something else
        public static implicit operator PositionData(Position pos)
        {
            return new PositionData(pos.X, pos.Y, pos.Z, pos.Heading);
        }

        public static implicit operator Position(Matrix pos)
        {
            return new Position(pos.Translation.X, pos.Translation.Y, pos.Translation.Z);
        }

        public static implicit operator Position(PositionData pos)
        {
            return new Position(pos.X, pos.Y, pos.Z) { Heading = pos.Heading };
        }

        public static implicit operator Vector3(Position pos)
        {
            return new Vector3(pos.X, pos.Y, pos.Z);
        }
        
        public bool Equals(Position other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return NearlyEqual(Translation.X, other.Translation.X, 0.01f) &&
                   NearlyEqual(Translation.Y, other.Translation.Y, 0.01f) &&
                   NearlyEqual(Translation.Z, other.Translation.Z, 0.01f);
        }

        public bool NearlyEqual(float a, float b, float epsilon)
        {
            //float absA = Math.Abs(a);
            //float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b) return true;
            if (a == 0 || b == 0 || diff < float.Epsilon) return diff < (epsilon * float.Epsilon);

            return diff <= epsilon;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals((Position)other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Translation.GetHashCode() * 397) ^ Heading.GetHashCode();
            }
        }

        public static bool operator ==(Position left, Position right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !Equals(left, right);
        }

        public string Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PositionData));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, (PositionData)this);
            return writer.ToString();
        }

        public static PositionData Deserialize(string value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PositionData));
            StringReader reader = new StringReader(value);
            return (PositionData)serializer.Deserialize(reader);
        }
    }
}
