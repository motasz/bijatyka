using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacter.Inputs
{
    public class BufferedInput
    {
        public readonly float Time;
        public readonly InputType Input;
        public Vector2 Value;

        public BufferedInput(InputType input, float time, Vector2 value = default)
        {
            Input = input;
            Time = time;
            Value = value;
        }
    }
    public class InputBuffer
    {
        private readonly Queue<BufferedInput>  _buffer = new Queue<BufferedInput>();
        private const float BufferTime = 0.10f;

        public void AddInput(InputType input, Vector2 value = default)
        {
            _buffer.Enqueue(new BufferedInput(input, Time.time, value));
        }

        public void Update()
        {
            while (_buffer.Count > 0)
            {
                var oldest = _buffer.Peek();

                if (Time.time - oldest.Time > BufferTime)
                {
                    _buffer.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        public void RemoveOldest()
        {
            _buffer.Dequeue();
        }

        public BufferedInput? GetOldest()
        {
            return _buffer.Count == 0 ? null : _buffer.Peek();
        }

        public void FlushInputs(InputType input = default)
        {
            var count = _buffer.Count;
            
            for (var i = 0; i < count; i++)
            {
                var current = _buffer.Dequeue();

                if (current.Input == input || input == default)
                {
                    continue;
                }
                
                _buffer.Enqueue(current);
            }
        }
    }
}