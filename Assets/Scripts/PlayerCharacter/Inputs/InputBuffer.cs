using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacter.Inputs
{
    public class BufferedInput
    {
        public float time;
        public InputType input;
        public Vector2 value;

        public BufferedInput(InputType input, float time, Vector2 value = default)
        {
            this.input = input;
            this.time = time;
            this.value = value;
        }
    }
    public class InputBuffer
    {
        private Queue<BufferedInput>  buffer = new Queue<BufferedInput>();
        private float bufferTime = 0.15f;

        public void AddInput(InputType input, Vector2 value = default)
        {
            buffer.Enqueue(new BufferedInput(input, Time.time, value));
        }

        public void Update()
        {
            while (buffer.Count > 0)
            {
                var oldest = buffer.Peek();

                if (Time.time - oldest.time > bufferTime)
                {
                    buffer.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        public BufferedInput? TryConsume(InputType input)
        {
            var count = buffer.Count;
            BufferedInput? found = null;
            
            for (var i = 0; i < count; i++)
            {
                var current = buffer.Dequeue();
                
                if (found == null && current.input == input)
                {
                    found = current;
                    continue;
                }
                
                buffer.Enqueue(current);
            }
            
            return found;
        }

        public void FlushInputs(InputType input)
        {
            var count = buffer.Count;
            
            for (var i = 0; i < count; i++)
            {
                var current = buffer.Dequeue();

                if (current.input == input)
                {
                    continue;
                }
                
                buffer.Enqueue(current);
            }
        }
    }
}