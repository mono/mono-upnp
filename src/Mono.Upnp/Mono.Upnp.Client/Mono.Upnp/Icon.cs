//
// Icon.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Net;
using System.Threading;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp
{
	public class Icon
	{
        protected internal Icon (Device device, XmlReader reader, WebHeaderCollection headers)
        {
            if (device == null) {
                throw new ArgumentNullException ("device");
            }
            this.device = device;
            Deserialize (reader, headers);
            loaded = true;
        }

        #region Data

        private readonly bool loaded;
        private bool loading;
        private readonly object loading_mutex = new object ();
        private AutoResetEvent loading_wait = new AutoResetEvent (false);

        private readonly Device device;
        public Device Device {
            get { return device; }
        }

        public bool IsDisposed {
            get { return device.IsDisposed; }
        }

        private string mime_type;
        public string MimeType {
            get { return mime_type; }
            protected set { SetField (ref mime_type, value); }
        }

        private int? width;
        public int Width {
            get { return width.Value; }
            protected set { SetField (ref width, value); }
        }

        private int? height;
        public int Height {
            get { return height.Value; }
            protected set { SetField (ref height, value); }
        }

        private int? depth;
        public int Depth {
            get { return depth.Value; }
            protected set { SetField (ref depth, value); }
        }

        private Uri url;
        public Uri Url {
            get { return url; }
            protected set { SetField (ref url, value); }
        }

        private Exception exception;
        private byte[] data;
        public byte[] GetData ()
        {
            bool wait = false;

            lock (loading_mutex) {
                if (data != null) {
                    return data;
                } else {
                    CheckDisposed ();
                    if (loading) {
                        wait = true;
                    } else {
                        loading = true;
                    }
                }
            }

            if (wait) {
                loading_wait.WaitOne ();
                if (data == null && exception != null) {
                    throw exception;
                }
                return data;
            }

            try {
                FetchData ();
                return data;
            } catch (Exception e) {
                exception = e;
                throw e;
            } finally {
                lock (loading_mutex) {
                    loading = false;
                    loading_wait.Set ();
                }
            }
        }

        public IAsyncResult BeginGetData (object state, AsyncCallback callback)
        {
            AsyncResult result = new AsyncResult (state, callback);

            lock (loading_mutex) {
                if (data != null) {
                    result.CompletedNormally (true);
                    return result;
                } else {
                    CheckDisposed ();
                    if (loading) {
                        ThreadPool.RegisterWaitForSingleObject (loading_wait, ResumeGetData, result, -1, true);
                    } else {
                        loading = true;
                    }
                }
            }

            ThreadPool.QueueUserWorkItem (GetDataAsync);

            return result;
        }

        public byte[] EndGetData (IAsyncResult asyncResult)
        {
            if (asyncResult == null) {
                throw new ArgumentNullException ("asyncResult");
            }
            AsyncResult result = asyncResult as AsyncResult;
            if (result == null) {
                throw new ArgumentException ("The provided asyncResult did not come from a call to BeginGetData.");
            }
            if (result.Exception != null) {
                throw result.Exception;
            }
            return data;
        }

        private void GetDataAsync (object state)
        {
            AsyncResult result = (AsyncResult)state;
            try {
                FetchData ();
                result.CompletedNormally (false);
            } catch (Exception e) {
                exception = e;
                result.CompletedExceptionally (false, e);
            }
            lock (loading_mutex) {
                loading = false;
                loading_wait.Set ();
            }
        }

        private void ResumeGetData (object state, bool timeout)
        {
            AsyncResult<byte[]> result = (AsyncResult<byte[]>)state;
            if (data != null || exception == null) {
                result.CompletedNormally (false);
            } else {
                result.CompletedExceptionally (false, exception);
            }
        }

        private void FetchData ()
        {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
                HttpWebResponse response = Helper.GetResponse (request);
                data = new byte[response.ContentLength];
                response.GetResponseStream ().Read (data, 0, (int)response.ContentLength);
                response.Close ();
            } catch (WebException e) {
                if (e.Status == WebExceptionStatus.Timeout) {
                    device.CheckNetworkPresence ();
                }
                throw e;
            }
        }

        #endregion

        #region Methods

        protected void CheckDisposed ()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString (),
                    "This icon is longer available because its device has gone off the network.");
            }
        }

        private void CheckLoaded ()
        {
            if (loaded) {
                throw new InvalidOperationException ("The icon has already been deserialized.");
            }
        }

        private void SetField<T> (ref T field, T value)
        {
            CheckLoaded ();
            field = value;
        }

        private void SetField<T> (ref T? field, T value) where T : struct
        {
            CheckLoaded ();
            field = value;
        }

        #region Overrides

        public override string ToString ()
        {
            return String.Format ("Icon {{ {0}, {1}x{2}x{3}, {4} }}", mime_type, Width, Height, Depth, url);
        }

        public override bool Equals (object obj)
        {
            Icon icon = obj as Icon;
            return icon != null && icon.url == url;
        }

        public override int GetHashCode ()
        {
            return url.GetHashCode ();
        }

        #endregion

        #region Deserialize

        private void Deserialize (XmlReader reader, WebHeaderCollection headers)
        {
            Deserialize (headers);
            Deserialize (reader);
            Verify ();
        }

        protected virtual void Deserialize (WebHeaderCollection headers)
        {
        }

        protected virtual void Deserialize (XmlReader reader)
        {
            try {
                reader.Read ();
                while (reader.Read () && reader.NodeType == XmlNodeType.Element) {
                    Deserialize (reader.ReadSubtree (), reader.Name);
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing an icon.", e);
            }
        }

        protected virtual void Deserialize (XmlReader reader, string element)
        {
            CheckLoaded ();
            reader.Read ();
            switch (element) {
            case "mimetype":
                mime_type = reader.ReadString ();
                break;
            case "width":
                reader.Read ();
                width = reader.ReadContentAsInt ();
                break;
            case "height":
                reader.Read ();
                height = reader.ReadContentAsInt ();
                break;
            case "depth":
                reader.Read ();
                depth = reader.ReadContentAsInt ();
                break;
            case "url":
                url = device.GetRoot ().DeserializeUrl (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        private void Verify ()
        {
            if (mime_type == null) {
                throw new UpnpDeserializationException ("The icon has no mime-type.");
            }
            if (width == null) {
                throw new UpnpDeserializationException ("The icon has no width.");
            }
            if (height == null) {
                throw new UpnpDeserializationException ("The icon has no height.");
            }
            if (depth == null) {
                throw new UpnpDeserializationException ("The icon has no depth.");
            }
            if (url == null) {
                throw new UpnpDeserializationException ("The icon has no URL.");
            }
            VerifyDeserialization ();
        }

        protected virtual void VerifyDeserialization ()
        {
        }

        protected internal virtual void VerifyContract ()
        {
        }

        #endregion

        #endregion

    }
}
