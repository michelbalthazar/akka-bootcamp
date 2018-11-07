﻿using Akka.Actor;

namespace WatchFile
{
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // make an actor system 
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            // this is here to show you what NOT to do
            // this approach to props has no type safety
            // it will compile, but can easily blow up in your face at runtime :(
            // UNCOMMENT THE BELOW TWO LINES, BUILD THE SOLUTION, AND THEN TRY TO RUN IT TO SEE
            //Props fakeActorProps = Props.Create(typeof(FakeActor));
            //IActorRef fakeActor = MyActorSystem.ActorOf(fakeActorProps, "fakeActor");

            // set up actors, using props (split props onto own line so easier to read)
            IActorRef consoleWriterActor = MyActorSystem.ActorOf(Props.Create<ConsoleWriterActor>(), "consoleWriterActor");

            IActorRef tailCoordinatorActor = MyActorSystem.ActorOf(Props.Create(() => new TailCoordinatorActor()), "tailCoordinatorActor");

            IActorRef validationActor = MyActorSystem.ActorOf(Props.Create(() => new FileValidatorActor(consoleWriterActor)), "validationActor");

            IActorRef consoleReaderActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor()), "consoleReaderActor");

            // tell console reader to begin

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }

        /// <summary>
        /// Fake actor / marker class. Does nothing at all, and not even an actor actually. 
        /// Here to show why you shouldn't use typeof approach to Props.
        /// </summary>
        public class FakeActor { }

    }
}
