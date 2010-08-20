// 
// EAGLView.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
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
using OpenTK.Platform.iPhoneOS;
using MonoTouch.CoreAnimation;
using OpenTK;
using OpenTK.Graphics.ES11;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;

namespace Mono.Upnp.Dcp.MediaServer1.GtkClient
{
    public partial class EAGLView : iPhoneOSGameView
    {
        [Export("layerClass")]
        static Class LayerClass ()
        {
            return iPhoneOSGameView.GetLayerClass ();
        }

        [Export("initWithCoder:")]
        public EAGLView (NSCoder coder) : base(coder)
        {
            LayerRetainsBacking = false;
            LayerColorFormat = EAGLColorFormat.RGBA8;
            ContextRenderingApi = EAGLRenderingAPI.OpenGLES1;
        }

        protected override void ConfigureLayer (CAEAGLLayer eaglLayer)
        {
            eaglLayer.Opaque = true;
        }

        protected override void OnRenderFrame (FrameEventArgs e)
        {
            base.OnRenderFrame (e);
            float[] squareVertices = { -0.5f, -0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f };
            byte[] squareColors = { 255, 255, 0, 255, 0, 255, 255, 255, 0, 0,
            0, 0, 255, 0, 255, 255 };
            
            MakeCurrent ();
            GL.Viewport (0, 0, Size.Width, Size.Height);
            
            GL.MatrixMode (All.Projection);
            GL.LoadIdentity ();
            GL.Ortho (-1.0f, 1.0f, -1.5f, 1.5f, -1.0f, 1.0f);
            GL.MatrixMode (All.Modelview);
            GL.Rotate (3.0f, 0.0f, 0.0f, 1.0f);
            
            GL.ClearColor (0.5f, 0.5f, 0.5f, 1.0f);
            GL.Clear ((uint)All.ColorBufferBit);
            
            GL.VertexPointer (2, All.Float, 0, squareVertices);
            GL.EnableClientState (All.VertexArray);
            GL.ColorPointer (4, All.UnsignedByte, 0, squareColors);
            GL.EnableClientState (All.ColorArray);
            
            GL.DrawArrays (All.TriangleStrip, 0, 4);
            
            SwapBuffers ();
        }
    }
}

