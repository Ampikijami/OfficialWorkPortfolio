using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace BasicDependencyInjectionOrIOCDemonstration
{
    class Program
    {
        static readonly bool makeMessageWriterSecure = true;
        
        static void Main(string[] args)
        {
            int pseudoRand = new Random(DateTime.Now.Millisecond).Next(1, 3);
            IMessageWriter.IMessageWriter messageWriter = null;
            switch (pseudoRand)
            {
                case 1:
                    messageWriter = new ConsoleMessageWriter();
                    break;
                case 2:
                    messageWriter = new PopupNotificationWriter();
                    break;
                default:
                    break;
            }
            if(makeMessageWriterSecure)
                messageWriter = new VerifiedUserWriter(messageWriter); //Existing class functionality can be extended using constructor injection.
            DependencyInjectionAnnouncement announcer = new DependencyInjectionAnnouncement(messageWriter);
            announcer.Exclaim("Hello, I was written using the " + announcer.writer.GetType().Name + " class.");
        }
    }

    class ConsoleMessageWriter : IMessageWriter.IMessageWriter
    {
        public void Write(string message)
        {
            System.Console.WriteLine(message);
        }
    }

    class PopupNotificationWriter : IMessageWriter.IMessageWriter
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);
        public void Write(string message)
        {
            MessageBox((IntPtr)0, message, "My Message Box", 0);
        }
    }
    class VerifiedUserWriter : IMessageWriter.IMessageWriter
    {
        private IMessageWriter.IMessageWriter writer;
        public VerifiedUserWriter(IMessageWriter.IMessageWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            this.writer = writer;
        }
        public void Write(string message)
        {
            if (verifyUser(GetIdentity()))
                this.writer.Write(message);
            else
                this.writer.Write("Unverified user!");
        }
        private bool verifyUser(IIdentity identity)
        {
            if (identity.IsAuthenticated)
                return true;
            else
                return false;
        }
        private static IIdentity GetIdentity()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent();
            }
            else
            {
                // For non-Windows OSes, like Mac and Linux.
                return new GenericIdentity(
                    Environment.GetEnvironmentVariable("USERNAME")
                    ?? Environment.GetEnvironmentVariable("USER"));
            }
        }
    }
    public class DependencyInjectionAnnouncement
    {
        public IMessageWriter.IMessageWriter writer { get; private set; }

        public DependencyInjectionAnnouncement(IMessageWriter.IMessageWriter writer) //this is called constructor injection
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        public void Exclaim(string message)
        {
            this.writer.Write(message);
        }
    }
}
