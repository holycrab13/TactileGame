using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Threading;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using System.Configuration;

namespace tud.mci.tangram.audio
{
    public sealed class AudioRenderer : IDisposable
    {
        #region Members

        static System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;
        readonly object _speacerLock = new Object();
        SpeechSynthesizer _speaker = null;
        SpeechSynthesizer Speaker
        {
            get
            {
                lock (_speacerLock)
                {
                    if (_speaker == null)
                    {
                        _speaker = new SpeechSynthesizer();
                        _speaker.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(_speaker_SpeakCompleted);
                    }
                    return _speaker;
                }
            }
        }

        private static readonly AudioRenderer _instance = new AudioRenderer();

        volatile bool _run;
        Thread _outputQueueThread;
        readonly object threadLock = new object();
        Thread outputQueueThread
        {
            get { lock (threadLock) { return _outputQueueThread; } }
            set { lock (threadLock) { _outputQueueThread = value; } }
        }
        static Queue _outputQueue = new Queue();
        static Queue OutputQueue = Queue.Synchronized(_outputQueue);

        volatile static int _volume = 100;
        /// <summary>
        /// The volume level for the speech output
        /// </summary>
        public static int Volume { 
            get { return _volume; } 
            set { _volume = Math.Min(100, Math.Max(0,value)); } 
        }

        volatile static int _speed = 1;
        /// <summary>
        /// The speed Level for the speech output
        /// </summary>
        public static int Speed
        {
            get { return _speed; }
            set { _speed = Math.Min(10, Math.Max(-10, value)); }
        }

        private static String StandardVoice;

        /// <summary>
        /// Gets a value indicating whether this instance is currently playing a sound or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </value>
        public bool IsPlaying { get; private set; }

        #endregion

        #region Constructor / Destructor / Singleton
        AudioRenderer()
        {
            GetVoices();
            setDefaultCulture();
            setDefaultVoice();
            Speaker.SetOutputToDefaultAudioDevice();
            //Speaker.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(speaker_SpeakCompleted);
            //Speaker.SpeakProgress += new EventHandler<SpeakProgressEventArgs>(speaker_SpeakProgress);
            //Speaker.SpeakStarted += new EventHandler<SpeakStartedEventArgs>(speaker_SpeakStarted);
        }

        ~AudioRenderer()
        {
            try
            {
                _run = false;
                if(this.outputQueueThread != null) this.outputQueueThread.Abort();
                Abort();
            }
            catch { }
        }

        public void Dispose()
        {
            Abort();
            if (Speaker != null) Speaker.Dispose();
        }

        /// <summary>
        /// Gets the singleton instance of the AudioRendere Object.
        /// </summary>
        /// <value>The instance.</value>
        public static AudioRenderer Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region synthesizer events
        
        void _speaker_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            IsPlaying = false;
            firefinishedEvent();
        }

        #endregion

        #region Voice
        private List<Voice> _installedVoices = new List<Voice>();
        private readonly object _ilLock = new object();
        private List<Voice> InstalledVoices
        {
            get
            {
                lock (_ilLock)
                {
                    return _installedVoices;
                }
            }
            set
            {
                lock (_ilLock)
                {
                    _installedVoices = value;
                }
            }
        }

        /// <summary>
        /// Sets the name of the standard voice.
        /// </summary>
        /// <param name="voiceNames">A comma separated list of voice names. The first fit will set the standard used voice.</param>
        /// <param name="ignoreCulture">if set to <c>true</c> a conflict between the set default voice culture and the voice name is ignored.</param>
        public void SetStandardVoiceName(String voiceNames, bool ignoreCulture = true)
        {
            var voices = GetVoices();
            if (!String.IsNullOrWhiteSpace(voiceNames))
            {
                string[] alternativVoices = voiceNames.Split(",".ToCharArray());
                if (voiceNames.Length > 0)
                {
                    foreach (String v in alternativVoices)
                    {
                        string v2 = v.Trim();
                        var voice = GetVoiceByName(v2);
                        if (!voice.IsEmpty)
                        {
                            if (ignoreCulture || voice.Culture.Equals(culture.Name))
                            {
                                StandardVoice = v2;
                                return;
                            }
                        }
                    }
                }
            }
            SetStandardVoiceByCulture(culture);
        }
        /// <summary>
        /// Sets the standard voice by the current system culture.
        /// Try to find an installed voice that have the same or patently equal culture.
        /// If no voice with the given culture is installed, the first installed voice is chosen.
        /// </summary>
        public void SetStandardVoiceByCulture() { SetStandardVoiceByCulture(null); }

        /// <summary>
        /// Sets the standard voice by a culture.
        /// Try to find an installed voice that have the same or patently equal culture.
        /// If no voice with the given culture is installed, the first installed voice is chosen.
        /// </summary>
        /// <param name="_culture">The culture. 
        /// The System.Globalization.CultureInfo can be created by an System.Globalization.CultureInfo.LCID Integer or via a standardized System.Globalization.CultureInfo.Name</param>
        public void SetStandardVoiceByCulture(System.Globalization.CultureInfo _culture)
        {
            var voices = GetVoices();

            if (_culture == null)
            {
                _culture = culture;
            }

            if (_culture != null)
            {
                foreach (Voice v in voices)
                {
                    if (v.Culture.Equals(_culture.Name))
                    {
                        StandardVoice = v.Name;
                        return;
                    }
                }

                foreach (Voice v in voices)
                {
                    if (v.Culture.StartsWith(_culture.Parent.Name))
                    {
                        StandardVoice = v.Name;
                        return;
                    }
                }
            }
            if (voices.Count > 0) StandardVoice = voices[0].Name;
            else StandardVoice = "";
        }

        /// <summary>
        /// Returns all available voices.
        /// </summary>
        /// <returns>List of voice informations</returns>
        public List<Voice> GetVoices()
        {
            List<Voice> voices = new List<Voice>();
            if (InstalledVoices != null && InstalledVoices.Count > 0)
            {
                voices.AddRange(InstalledVoices);
            }
            else if (Speaker != null)
            {
                var iv = Speaker.GetInstalledVoices();
                foreach (var v in iv)
                {
                    InstalledVoice voice = v as InstalledVoice;
                    if (voice != null)
                    {
                        VoiceInfo vi = voice.VoiceInfo;
                        voices.Add(new Voice(
                            vi.Name,
                            vi.Culture.Name,
                            vi.Gender.ToString(),
                            vi.Age.ToString()
                            ));
                    }
                }
                InstalledVoices = voices;
            }
            return voices;
        }

        /// <summary>
        /// Gets a installed voice by name if exist.
        /// </summary>
        /// <param name="name">the name of the voice to search for</param>
        /// <returns>the corresponding voice struct or an empty voice</returns>
        public Voice GetVoiceByName(string name)
        {
            var voices = GetVoices();
            foreach (var voice in voices)
            {
                if (!voice.IsEmpty && voice.Name.Equals(name))
                {
                    return voice;
                }
            }
            return new Voice();
        }

        #endregion

        #region Culture Setting

        /// <summary>
        /// Sets the default culture.
        /// Checks if in the [App.exe].config is the specific 'appSettings' key 'DefaultCulture' is set.
        /// 
        /// in the [App.exe].config file something like this has to be entered if you want to override 
        /// to use the default system language setting.
        /// 
        /// &lt;?xml version ="1.0"?&gt;
        /// &lt;configuration&gt;
        ///  	[...]
        ///  	&lt;appSettings&gt;
        /// 		&lt;add key="DefaultCulture" value="en-US" /&gt;
        /// 	&lt;/appSettings&gt;
        /// 	[...]
        /// &lt;/configuration&gt;
        /// </summary>
        void setDefaultCulture()
        {
            try
            {
                string cultureName = String.Empty;
                String appConfigCulture = ConfigurationManager.AppSettings["DefaultCulture"];
                if (!String.IsNullOrWhiteSpace(appConfigCulture))
                {
                    cultureName = appConfigCulture.ToString();
                }

                if (!String.IsNullOrWhiteSpace(cultureName))
                {
                    CultureInfo _culture = new CultureInfo(cultureName);
                    if (_culture != null)
                    {
                        culture = _culture;
                    }
                }
            }
            catch { }
            finally
            {
                if (culture == null)
                {
                    culture = System.Globalization.CultureInfo.CurrentCulture;
                }
            }
        }

        /// <summary>
        /// Sets the default voice.
        /// Checks if in the [App.exe].config is the specific 'appSettings' key 'StandardVoice' is set.
        /// The voice setting will be ignored if it does not fit to the configured default culture!
        /// 
        /// in the [App.exe].config file something like this has to be entered if you want to override 
        /// to use the default system language setting.
        /// 
        /// &lt;?xml version ="1.0"?&gt;
        /// &lt;configuration&gt;
        ///  	[...]
        ///  	&lt;appSettings&gt;
        /// 		&lt;add key="StandardVoice" value="Microsoft Anna" /&gt;
        /// 	&lt;/appSettings&gt;
        /// 	[...]
        /// &lt;/configuration&gt;
        /// </summary>
        void setDefaultVoice()
        {
            try
            {
                string voiceNames = String.Empty;
                String appConfigCulture = ConfigurationManager.AppSettings["StandardVoice"];
                if (!String.IsNullOrWhiteSpace(appConfigCulture))
                {
                    voiceNames = appConfigCulture.ToString();
                }

                if (!String.IsNullOrWhiteSpace(voiceNames))
                {
                    SetStandardVoiceName(voiceNames, false);
                }
            }
            catch { }
        }
        
        #endregion

        #region Play functions
        /// <summary>
        /// Plays a given text as sound via SpeechSynthesizer and stops all other sounds.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <returns></returns>
        public bool PlaySoundImmediately(String text) { AbortSound(); return PlaySound(text, StandardVoice); }
        /// <summary>
        /// Plays a given text as sound via SpeechSynthesizer and stops all other sounds.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <param name="voice">The voice that should be used for speaking.</param>
        /// <returns></returns>
        public bool PlaySoundImmediately(String text, Voice voice) { AbortSound(); return PlaySound(text, voice.Name); }
        /// <summary>
        /// Plays a given text as sound via SpeechSynthesizer and stops all other sounds.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <param name="voice">The name of the voice that should be used for speaking.</param>
        /// <returns></returns>
        public bool PlaySoundImmediately(String text, String voice) { AbortSound(); return PlaySound(text, voice); }

        /// <summary>
        /// Plays a given text as sound via SpeechSynthesizer.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <returns></returns>
        public bool PlaySound(String text)
        {
            if (InstalledVoices == null || InstalledVoices.Count < 1) return false;
            if (String.IsNullOrEmpty(StandardVoice)) SetStandardVoiceByCulture();
            return PlaySound(text, StandardVoice);
        }
        /// <summary>
        /// Plays a given text as sound via SpeechSynthesizer.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <param name="voice">The voice that should be used for speaking.</param>
        /// <returns></returns>
        public bool PlaySound(String text, Voice voice) { return PlaySound(text, voice.Name); }
        /// <summary>
        /// Plays a given text as sound via SpeechSynthesizer.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <param name="voice">The name of the voice that should be used for speaking.</param>
        /// <returns></returns>
        public bool PlaySound(String text, String voice)
        {
            if (String.IsNullOrWhiteSpace(text) || InstalledVoices == null || InstalledVoices.Count < 1) return false;
            if (String.IsNullOrWhiteSpace(voice) || !InstalledVoices.Contains(new Voice(voice, null, null, null)))
            {
                if (InstalledVoices == null || InstalledVoices.Count < 1) { return false; }
                else { voice = InstalledVoices[0].Name; }
            }
            return enqueue(new AudioOutputQueueItem(SoundOutputType.Text, text, voice, Volume, Speed));
        }

        /// <summary>
        /// Plays a wave file and stops all other sounds.
        /// </summary>
        /// <param name="path">The path to the wav file.</param>
        /// <returns>True if the file could be played</returns>
        public bool PlayWaveImmediately(string path) { AbortSound(); return PlayWave(path); }
        /// <summary>
        /// Plays some standard sounds and stops all other sounds.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <returns>True if the file could be played</returns>
        public bool PlayWaveImmediately(StandardSounds sound) { AbortSound(); return PlayWave(sound); }

        /// <summary>
        /// Plays a wave file.
        /// </summary>
        /// <param name="path">The path to the wav file.</param>
        /// <returns>True if the file could be played</returns>
        public bool PlayWave(string path)
        {
            return enqueue(new AudioOutputQueueItem(SoundOutputType.Wave, path, null, Volume, Speed));
        }
        /// <summary>
        /// Plays some standard sounds.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <returns>True if the file could be played</returns>
        public bool PlayWave(StandardSounds sound)
        {
            //TODO: set the path generic
            string path = @"C:/Windows/media/";
            switch (sound)
            {
                case StandardSounds.None:
                    return false;
                case StandardSounds.Error:
                    path += "Windows Error.wav";
                    break;
                case StandardSounds.Critical:
                    path += "Windows Critical Stop.wav";
                    break;
                case StandardSounds.End:
                    path += "Windows Balloon.wav";
                    break;
                case StandardSounds.Ping:
                    path += "Windows Ding.wav";
                    break;
                case StandardSounds.Start:
                    path += "Windows Logon Sound.wav";
                    break;
                case StandardSounds.Stop:
                    path += "Windows Logoff Sound.wav";
                    break;
                case StandardSounds.Notify:
                    path += "Windows Notify.wav";
                    break;
                case StandardSounds.Info:
                    path += "Windows Information Bar.wav";
                    break;
                default:
                    return false;
            }
            return PlayWave(path);
        }

        /// <summary>
        /// Speaks a given text.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <param name="voiceName">Name of the voice.</param>
        /// <returns>True if the text could be enqueued</returns>
        private bool speakText(String text, String voiceName)
        {
            try
            {
                if (Speaker != null && !String.IsNullOrWhiteSpace(text))
                {
                    int trys = 0;
                    while (Speaker.State != SynthesizerState.Ready && trys++ < 5)
                    {
                        Thread.Sleep(20);
                    }

                    if (Speaker.State == SynthesizerState.Ready)
                    {
                        // Configure the audio output. 
                        Speaker.SetOutputToDefaultAudioDevice();
                        Speaker.Rate = Convert.ToInt32(Speed);
                        Speaker.Volume = Convert.ToInt32(Volume);
                        Speaker.SelectVoice(voiceName);
                        IsPlaying = true;
                        Speaker.SpeakAsync(text);
                        fireTextSpokenEvent(text);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Plays the *.wav file. 
        /// Converts the file path to an ssml command and send it to the SpeechSynthesizer
        /// </summary>
        /// <param name="path">The path to the .wav file.</param>
        /// <returns>True if the file could be enqueued</returns>
        private bool playWavFile(string path)
        {
            if (String.IsNullOrEmpty(path) || !File.Exists(path)) return false;

            // Build an SSML prompt in a string.
            string str = "<speak version=\"1.0\"";
            str += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
            str += " xml:lang=\"en-US\">";
            str += "<audio src=\"file:///" + path + "\" />";
            str += "</speak>";

            Speaker.SetOutputToDefaultAudioDevice();

            IsPlaying = true;
            Speaker.SpeakSsmlAsync(str);
            fireFilePlayedEvent(path);

            return true;
        }

        #endregion

        #region Abort

        /// <summary>
        /// Aborts the current playing sound.
        /// </summary>
        /// <returns>true if abortion was successful</returns>
        public bool AbortCurrentSound()
        {
            if (Speaker.GetCurrentlySpokenPrompt() != null)
            {
                Speaker.SpeakAsyncCancel(Speaker.GetCurrentlySpokenPrompt());
                IsPlaying = false;
                fireStopedEvent();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Aborts all enqueued sound outputs.
        /// </summary>
        public void Abort()
        {
            if(this.outputQueueThread != null)this.outputQueueThread.Abort();
            AbortSound();
        }

        public void AbortSound()
        {
            OutputQueue.Clear();
            Speaker.SpeakAsyncCancelAll();
            IsPlaying = false;
            fireStopedEvent();
        }

        #endregion

        #region Queuing

        #region Thread
        void createOutputQueueThread()
        {
            _run = true;
            if (outputQueueThread != null && !(outputQueueThread.ThreadState == ThreadState.Aborted || outputQueueThread.ThreadState == ThreadState.AbortRequested))
            {
                try
                {
                    if (outputQueueThread != null && outputQueueThread.IsAlive) return;
                    else if (outputQueueThread != null) outputQueueThread.Start();
                    else buildThread();
                }
                catch (ThreadStartException) { buildThread(); }
            }
            else
            { buildThread(); }
        }

        private void buildThread()
        {
            outputQueueThread = new Thread(new ThreadStart(checkOutputQueue));
            outputQueueThread.Name = "TangramLectorAudioQueueThread";
            outputQueueThread.IsBackground = true;
            outputQueueThread.Start();
        }

        void checkOutputQueue()
        {
            while (_run)
            {
                if (OutputQueue.Count > 0)
                {
                    try
                    {
                        //give the speaker some time to reject
                        Thread.Sleep(2);
                        if (Speaker.State == SynthesizerState.Ready)
                            handleQueueItem(OutputQueue.Dequeue());
                    }
                    catch { }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
        }

        #endregion

        /// <summary>
        /// Enqueues the specified obj to the output queue.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private bool enqueue(Object obj)
        {
            try
            {
                createOutputQueueThread();
                OutputQueue.Enqueue(obj);
            }
            catch
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Handles the queued items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private bool handleQueueItem(Object obj)
        {
            try
            {
                if (obj is AudioOutputQueueItem)
                {
                    AudioOutputQueueItem ai = (AudioOutputQueueItem)obj;
                    switch (ai.Type)
                    {
                        case SoundOutputType.Text:
                            return speakText(ai.Value.ToString(), ai.VoiceName);
                        case SoundOutputType.Wave:
                            if (ai.Value is StandardSounds) return PlayWaveImmediately((StandardSounds)ai.Value);
                            else return playWavFile(ai.Value.ToString());
                        case SoundOutputType.Standardsound:
                            break;
                        default:
                            break;
                    }
                }
                else if (obj is String) { return speakText(obj as String, String.Empty); }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Get the number of elements in the queue.
        /// </summary>
        /// <returns>Amount of output requests in the queue</returns>
        public int NumberInQueue()
        {
            return OutputQueue.Count;
        }

        #endregion

        #region event

        /// <summary>
        /// Occurs when a text should been spoken by the text to speech engine.
        /// </summary>
        public event EventHandler<AudioRendererTextEventArgs> TextSpoken;
        /// <summary>
        /// Occurs when a sound file should be played.
        /// </summary>
        public event EventHandler<AudioRendererSoundEventArgs> FilePlayed;
        /// <summary>
        /// Occurs when the audio output was aborted.
        /// </summary>
        public event EventHandler<EventArgs> Stoped;
        /// <summary>
        /// Occurs when a audio output was finished successfully.
        /// </summary>
        public event EventHandler<EventArgs> Finished;

        void fireTextSpokenEvent(string text)
        {
            try
            {
                Task t = new Task(() =>
                    {
                        if (TextSpoken != null)
                        {
                            try { TextSpoken.Invoke(this, new AudioRendererTextEventArgs(text)); }
                            catch (Exception)
                            { }
                        }
                    });
                t.Start();
            }
            catch (Exception)
            { }
        }

        void fireFilePlayedEvent(string soundName)
        {
            try
            {
                Task t = new Task(() =>
                {
                    if (FilePlayed != null)
                    {
                        try { FilePlayed.Invoke(this, new AudioRendererSoundEventArgs(soundName)); }
                        catch (Exception)
                        { }
                    }
                });
                t.Start();
            }
            catch (Exception)
            { }
        }

        void fireStopedEvent()
        {
            try
            {
                Task t = new Task(() =>
                {
                    if (Stoped != null)
                    {
                        try { Stoped.Invoke(this, null); }
                        catch (Exception)
                        { }
                    }
                });
                t.Start();
            }
            catch (Exception)
            { }
        }

        void firefinishedEvent()
        {
            try
            {
                Task t = new Task(() =>
                {
                    if (Finished != null)
                    {
                        try { Finished.Invoke(this, null); }
                        catch (Exception)
                        { }
                    }
                });
                t.Start();
            }
            catch (Exception)
            { }
        }

        #endregion

    }

    #region Structs
    /// <summary>
    /// A simple structure to store important voice informations. 
    /// </summary>
    public struct Voice
    {
        bool _notEmpty;
        /// <summary>
        /// If voice is not set
        /// </summary>
        public bool IsEmpty { get { return !_notEmpty; } }
        /// <summary>
        /// Name of the voice. Essential for getting the voice to the speech engine.
        /// </summary>
        public readonly String Name;
        /// <summary>
        /// Gives the culture of the voice, e.g. en-US or de-DE 
        /// </summary>
        public readonly String Culture;
        /// <summary>
        /// Gives the gender of the voice if set
        /// </summary>
        public readonly String Gender;
        /// <summary>
        /// Give some information about the age of the speaker if set.
        /// </summary>
        public readonly String Adge;
        public Voice(string name, String culture, String gender, String adge)
        {
            this.Name = name; this.Culture = culture; this.Gender = gender; this.Adge = adge; _notEmpty = true;
        }
        public override string ToString() { return Name + ", Culture: " + Culture + ", Gender: " + Gender + ", Age: " + Adge; }
        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;
            else
            {
                if (obj is Voice)
                {
                    Voice v = (Voice)obj;
                    if (
                        (Name.Equals(v.Name)) &&
                        (String.IsNullOrEmpty(v.Culture) || Culture.Equals(v.Culture)) &&
                        (String.IsNullOrEmpty(v.Gender) || Gender.Equals(v.Gender)) &&
                        (String.IsNullOrEmpty(v.Adge) || Adge.Equals(v.Adge))
                    )
                        return true;
                }

            }
            return false;
        }
        public override int GetHashCode() { return base.GetHashCode(); }
    }

    struct AudioOutputQueueItem
    {
        public SoundOutputType Type;
        public Object Value;
        public String VoiceName;
        public int Volume;
        public int Speed;

        public AudioOutputQueueItem(SoundOutputType type, Object value, String voice, int volume, int speed)
        {
            Type = type; Value = value; VoiceName = voice; Volume = Math.Max(volume, 0); Speed = Math.Max(speed, 0);
        }

    }
    #endregion

    #region Enums
    enum SoundOutputType
    {
        Text,
        Wave,
        Standardsound
    }

    /// <summary>
    /// Some standard sounds
    /// </summary>
    public enum StandardSounds
    {
        /// <summary>
        /// No Sound
        /// </summary>
        None,
        /// <summary>
        /// a normal Error occurred
        /// </summary>
        Error,
        /// <summary>
        /// a critical error occurred
        /// </summary>
        Critical,
        /// <summary>
        /// maybe at the end of something - operation is not possible
        /// </summary>
        End,
        /// <summary>
        /// a small ping sound
        /// </summary>
        Ping,
        /// <summary>
        /// a longer start sound
        /// </summary>
        Start,
        /// <summary>
        /// a longer end sound
        /// </summary>
        Stop,
        /// <summary>
        /// a smooth notification sound
        /// </summary>
        Notify,
        /// <summary>
        /// a very short notification ping
        /// </summary>
        Info,
    }

    #endregion

    #region Event Args

    /// <summary>
    /// Event args to convey information about the spoken text
    /// </summary>
    public class AudioRendererTextEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the text that was spoken via text to speech engine.
        /// </summary>
        /// <value>The text.</value>
        public String Text { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRendererTextEventArgs"/> class.
        /// </summary>
        /// <param name="text">The text that was spoken by the text to speech engine.</param>
        public AudioRendererTextEventArgs(string text)
        {
            this.Text = text;
        }
    }

    /// <summary>
    /// Event args to convey information about the sound file that was played
    /// </summary>
    public class AudioRendererSoundEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the sound file that was played.
        /// </summary>
        /// <value>The name of the sound.</value>
        public String SoundName { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRendererSoundEventArgs"/> class.
        /// </summary>
        /// <param name="sName">Name of the sound file that was played.</param>
        public AudioRendererSoundEventArgs(string sName)
        {
            this.SoundName = sName;
        }
    }


    #endregion

}