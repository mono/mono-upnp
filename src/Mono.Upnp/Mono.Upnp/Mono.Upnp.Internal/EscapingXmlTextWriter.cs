// 
// EscapingXmlTextWriter.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Peterson
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
using System.Xml;

namespace Mono.Upnp.Internal
{
    class EscapingXmlTextWriter : XmlTextWriter
    {
        public EscapingXmlTextWriter (Stream stream, Encoding encoding)
             : base (stream, encoding)
        {
        }
        
        public override void WriteString (string text)
        {
            if (text == null) {
                return;
            }
            try {
            StringBuilder builder = null;
            var start = 0;
            
            for (var i = 0; i < text.Length; i++) {
                if (text[i] == '<' || text[i] == '>' || text[i] == '"' || text[i] == '\'' || text[i] == '&') {
                    if (builder == null) {
                        builder = new StringBuilder (text.Length);
                    }
                    
                    builder.Append (text, start, i - start);
                    builder.Append ('&');
                    
                    switch (text[i]) {
                    case '<':
                        builder.Append ("lt");
                        break;
                    case '>':
                        builder.Append ("gt");
                        break;
                    case '"':
                        builder.Append ("quot");
                        break;
                    case '\'':
                        builder.Append ("apos");
                        break;
                    case '&':
                        builder.Append ("amp");
                        break;
                    }
                    
                    builder.Append (';');
                    start = i + 1;
                }
            }
            
            if (builder != null) {
                if (start < text.Length) {
                    builder.Append (text, start, text.Length - start);
                }
                base.WriteRaw (builder.ToString ());
            } else {
                base.WriteString (text);
            }
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }
    }
}
