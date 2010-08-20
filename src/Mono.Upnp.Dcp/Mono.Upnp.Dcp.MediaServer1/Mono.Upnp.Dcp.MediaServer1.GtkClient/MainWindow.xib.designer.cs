
namespace Mono.Upnp.Dcp.MediaServer1.GtkClient
{

    // Base type probably should be MonoTouch.Foundation.NSObject or subclass
    [MonoTouch.Foundation.Register("AppDelegate")]
    public partial class AppDelegate
    {

        private MonoTouch.UIKit.UIWindow __mt_window;

        #pragma warning disable 0169
        [MonoTouch.Foundation.Connect("window")]
        private MonoTouch.UIKit.UIWindow window {
            get {
                this.__mt_window = ((MonoTouch.UIKit.UIWindow)(this.GetNativeField ("window")));
                return this.__mt_window;
            }
            set {
                this.__mt_window = value;
                this.SetNativeField ("window", value);
            }
        }

        private EAGLView __mt_glView;

        [MonoTouch.Foundation.Connect("glView")]
        private EAGLView glView {
            get {
                this.__mt_glView = ((EAGLView)(this.GetNativeField ("glView")));
                return this.__mt_glView;
            }
            set {
                this.__mt_glView = value;
                this.SetNativeField ("glView", value);
            }
        }
    }

    // Base type probably should be MonoTouch.UIKit.UIView or subclass
    [MonoTouch.Foundation.Register("EAGLView")]
    public partial class EAGLView
    {
    }
}

