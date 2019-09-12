using System;

namespace ChLib
{
    public struct HashEntry
    {
        public Position Position;
        public int Color;
        public int Value;
        public int Depth;
        public int ValueType;
        public Position BestMove;

        public override string ToString()
        {
            return string.Format("Color={0};Value={1};Depth={2};ValueType={3}\n{4}", Color, Value, Depth, ValueType, Position);
        }
    }
}
