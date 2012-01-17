// 
// UpdateTextWriter.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Peterson
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Text;

namespace Mono.Upnp.Dcp.MediaServer1.Xml
{
    public class UpdateTextWriter : TextWriter
    {
        readonly TextWriter text_writer;
        
        public UpdateTextWriter (TextWriter textWriter)
        {
            if (textWriter == null) {
                throw new ArgumentNullException ("textWriter");
            }
            
            text_writer = textWriter;
        }
        
        public override Encoding Encoding {
            get { return text_writer.Encoding; }
        }
        
        public override void Write (char value)
        {
            if (value == ',') {
                text_writer.Write ('\\');
            }
            text_writer.Write (value);
        }
        
        public override void Write (char[] buffer, int index, int count)
        {
            if (buffer == null) {
                throw new ArgumentNullException ("buffer");
            } else if (index < 0) {
                throw new ArgumentOutOfRangeException ("index", "The index is less than zero.");
            } else if (count < 0) {
                throw new ArgumentOutOfRangeException ("count", "The count is less than zero.");
            }
            
            var limit = count + index;
            
            if (limit > buffer.Length) {
                throw new ArgumentException ("The index and count go beyond the length of the buffer.");
            }
            
            var comma_count = 0;
            for (var i = index; i < limit; i++) {
                if (buffer[i] == ',') {
                    comma_count++;
                }
            }
            
            if (comma_count == 0) {
                text_writer.Write (buffer, index, count);
            } else {
                var new_buffer = new char[count + comma_count];
                for (int i = index, j = 0; i < limit; i++, j++) {
                    if (buffer[i] == ',') {
                        new_buffer[j] = '\\';
                        j++;
                    }
                    new_buffer[j] = buffer[i];
                }
                text_writer.Write (new_buffer);
            }
        }
        
        public override void Write (char[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException ("buffer");
            
            Write (buffer, 0, buffer.Length);
        }
        
        public override void Write (string value)
        {
            if (value != null) {
                text_writer.Write (Escape (value));
            }
        }
        
        public override void Flush ()
        {
            text_writer.Flush ();
        }
        
        public override void Close ()
        {
            text_writer.Close ();
        }
        
        protected override void Dispose (bool disposing)
        {
            if (disposing) {
                text_writer.Dispose ();
            }
        }
        
        static string Escape (string source)
        {
            var tail = source.IndexOf (',');
            if (tail != -1) {
                var head = 0;
                var builder = new StringBuilder (source.Length + 1);
                do {
                    builder.Append (source, head, tail - head);
                    builder.Append (@"\,");
                    head = tail + 1;
                    tail = source.IndexOf (',', head);
                } while (tail != -1);
                if (head != source.Length) {
                    builder.Append (source, head, source.Length - head);
                }
                return builder.ToString ();
            } else {
                return source;
            }
        }
    }
}
