using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using System;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;

namespace FeliciaBot
{
    public class FeliciaBot9000
    {
        public static TwitchClient client;
        public ConnectionCredentials credentials;
        private static bool completed = false;
        private static object locker = new object();
        public static readonly SrgsRuleRef Garbage;
        public static int counter = 0;

        public void Initiate()
        {
            try
            {
                credentials = new ConnectionCredentials("FeliciaBot9000", "oauth:syf7f73u4y81o23kf7xez5aze3emco", "irc-ws.chat.twitch.tv", 80, false);

                client = new TwitchClient(credentials);
                client.OnJoinedChannel += OnJoinedChannel;
                client.Connect();
                client.JoinChannel("sitekaz");
                SpeechRecognition();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        private void SpeechRecognition()
        {
            using (var speechEngine = new SpeechRecognizer())
            {
                var recognize = new Choices(new string[] { "Felicia", "red", "blue", "green" });
                
                var grammarBuilder = new GrammarBuilder(recognize);
                var grammar = new Grammar(grammarBuilder);
                speechEngine.LoadGrammar(grammar);
                
                speechEngine.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(SpeechDetectedHandler);
                speechEngine.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(SpeechHypothesizedHandler);
                speechEngine.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRecognitionRejectedHandler);
                speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizedHandler);

                Console.ReadLine();
            }
        }

        private static void RecognizeCompletedHandler(object sender, RecognizeCompletedEventArgs e)
        {
            Console.WriteLine($"Recognize Completed:  {e?.Result?.Text}  {e?.Result?.Confidence}  {e?.AudioPosition}");
        }

        private static void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Recognized Handler: " + e?.Result);
        
        }

        private static void SpeechRecognitionRejectedHandler(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine($"Rejected... Alternatives - {e?.Result?.Alternates} Words Generated - {e?.Result?.Words} Text Result - {e?.Result?.Text}");
            Console.WriteLine("Rejected Words...");

            foreach(var word in e?.Result?.Words)
            {
                Console.WriteLine($"\t{word}");
            }
        }

        private static void SpeechHypothesizedHandler(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.WriteLine($"Hypothesizing.... {e?.Result?.Text}");
            if (e.Result != null && e.Result.Text != null && e.Result.Text.ToLower().Contains("felicia"))
            {
                counter += 1;
                client.SendMessage($"Counter: {counter}");
            }
        }

        private static void SpeechDetectedHandler(object sender, SpeechDetectedEventArgs e)
        {
            Console.WriteLine($"Speech Detected... {e?.AudioPosition} {sender}");
        }

        private static void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            client.SendMessage("Initiating Protocol 143...");
        }
    }
}