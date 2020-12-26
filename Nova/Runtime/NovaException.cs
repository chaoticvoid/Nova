using System;

namespace Nova.Runtime {
    // Used to throw a Nova-based exception class
    [NovaDoNotExport]
    public class NovaException : Exception {
        public NovaException(NovaInstance obj) {
            var klass = obj.Class;

            var exceptionFound = false;
            var _class = obj.Class;
            do {
                if (_class.Name.Equals("Exception")) {
                    exceptionFound = true;
                    break;
                }
                _class = _class.Parent;
            } while (!exceptionFound && _class != null);

            if (exceptionFound) {
                ExceptionClass = klass;
                InnerObject = obj;
            }
            else {
                ExceptionClass = Nova.Box(typeof (NovaException));
                InnerObject = null;
            }
        }

        public NovaInstance InnerObject { get; private set; }

        public NovaClass ExceptionClass { get; private set; }
    }
}