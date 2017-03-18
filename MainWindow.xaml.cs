using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Speech.Synthesis;
using System.ComponentModel;

namespace WordLex
{
                
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool hasVoice = false;
        SpeechSynthesizer synth = new SpeechSynthesizer();
        PromptBuilder builder = new PromptBuilder();

        private readonly BackgroundWorker worker = new BackgroundWorker();


        String Language1 = "EN";
        String Language2 = "DE";

        bool ForeignToNative = true;

        int DefaultSecondsToAnswer = 10;
        int secondsToAnswer =0;

        /// <summary>
        /// Data Access provide all the words for the current vocabulary.
        /// </summary>
        DataAccess da = new DataAccess();

        /// <summary>
        /// A random numbers generator.
        /// </summary>
        Random rnd = new Random();

        /// <summary>
        /// Words available to build the quize. When no more words are available on the same level, refer this to the all known words.
        /// </summary>
        List<Word> WordsToBeConsummed;

        /// <summary>
        /// On the same selection table, all words have to be unique. While consumming words, add them here to avoid reuse.
        /// </summary>
        List<Word> ConsumedWords;


        /// <summary>
        /// Difficulty of the level is related to the number of times a word was mistaked.
        /// </summary>
        int DifficultyLevel { get; set; }

        int bads = 0;
        int goods = 0;
        
        List<Word> currentLevelWords = new List<Word>();
        Word currentWord = null;

        static int GoodAnsware { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            UpdateLevel();
            dispatcherTimer.Start();

            Build();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                {
                    if (ForeignToNative)
                    {
                        //synth.SelectVoice();
                    }
                    else
                    {
                        //synth.SelectVoice();
                    }
                    builder.ClearContent();
                    builder.AppendText(ForeignToNative ? currentWord.Foreign : currentWord.Native);
                    synth.SpeakAsyncCancelAll();
                    synth.SpeakAsync(builder);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            da.Save();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            secondsToAnswer--;
            labelNews.Content = "" + secondsToAnswer;
            if(secondsToAnswer == 0)
            {
                labelLastPair.Content = "Time expired!";
                Build();
            }
        }

        private void UpdateLevel()
        {
            currentLevelWords.Clear();
            foreach (Word word in da.words)
            {
                int dificulty = ForeignToNative ? word.ForeignDifficulty : word.NativeDifficulty;
                if (dificulty == DifficultyLevel)
                {
                    currentLevelWords.Add(word);
                }
            }
        }

        private void Build()
        {
            labelWordsLeft.Content = currentLevelWords.Count();
            labelWordsBads.Content = bads;
            labelWordsGoods.Content = goods;
            labelLevel.Content = DifficultyLevel;

            if (currentLevelWords.Count() < 1)
            {
                labelWordToFind.Content = "Try another level!";
                button1.Content = "";
                button2.Content = "";
                button3.Content = "";
                button4.Content = "";
                button5.Content = "";
                return;
            }
            List<Button> availableButtons = new List<Button>() { button1, button2, button3, button4, button5 };
            GoodAnsware = rnd.Next(availableButtons.Count());
            currentWord = currentLevelWords[rnd.Next(currentLevelWords.Count())];
            if (hasVoice)
            {
                if (worker.IsBusy)
                {
                    if (worker.WorkerSupportsCancellation)
                    {
                        worker.CancelAsync();
                    }
                }
                try
                {
                    worker.RunWorkerAsync();
                }
                catch (Exception ex)
                {

                }
            }
            availableButtons[GoodAnsware].Content = ForeignToNative ? currentWord.Native : currentWord.Foreign;
            
            labelWordToFind.Content = ForeignToNative ? currentWord.Foreign : currentWord.Native;

            WordsToBeConsummed = currentLevelWords;
            ConsumedWords = new List<Word>() { currentWord };
            Word randomWord = null;

            if (GoodAnsware != 0)
            {
                randomWord = GetRandomlyUniqueWord();
                button1.Content = ForeignToNative ? randomWord.Native : randomWord.Foreign;
            }
            if (GoodAnsware != 1)
            {
                randomWord = GetRandomlyUniqueWord();
                button2.Content = ForeignToNative ? randomWord.Native : randomWord.Foreign;
            }
            if (GoodAnsware != 2)
            {
                randomWord = GetRandomlyUniqueWord();
                button3.Content = ForeignToNative ? randomWord.Native : randomWord.Foreign;
            }
            if (GoodAnsware != 3)
            {
                randomWord = GetRandomlyUniqueWord();
                button4.Content = ForeignToNative ? randomWord.Native : randomWord.Foreign;
            }
            if (GoodAnsware != 4)
            {
                randomWord = GetRandomlyUniqueWord();
                button5.Content = ForeignToNative ? randomWord.Native : randomWord.Foreign;
            }

            secondsToAnswer = DefaultSecondsToAnswer;
        }

        /// <summary>
        /// Get a random word from the same level when available, or from all words in ithe same dictionary.
        /// No duplicates are alowed in the same quize.
        /// </summary>
        /// <returns></returns>
        private Word GetRandomlyUniqueWord()
        {
            if (currentLevelWords.Count() - ConsumedWords.Count() < 1)
            {
                WordsToBeConsummed = da.words;
            }
            Word randomWord = currentWord;
            while (ConsumedWords.Contains(randomWord))
            {
                randomWord = WordsToBeConsummed[rnd.Next(WordsToBeConsummed.Count())];
            }
            ConsumedWords.Add(randomWord);
            return randomWord;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Solve(0);
        }

        /// <summary>
        /// Interpret the given answer
        /// </summary>
        /// <param name="givenAnswer"></param>
        private void Solve(int givenAnswer)
        {
            labelColor.Background = Brushes.WhiteSmoke;
            if (givenAnswer == GoodAnsware)
            {
                labelLastPair.Foreground = Brushes.Green;
                currentLevelWords.Remove(currentWord);
                labelNews.Content = "Nice one!";
                labelNews.Foreground = Brushes.Green;
                while (currentLevelWords.Count()==0)
                {
                    DifficultyLevel++;
                    UpdateLevel();
                    labelNews.Content = "Level was updated!";
                    labelColor.Background = Brushes.Yellow;
                }
                if (ForeignToNative)
                {
                    if (currentWord.ForeignDifficulty > 0)
                    {
                        currentWord.ForeignDifficulty--;
                    }
                }
                else
                {
                    if (currentWord.NativeDifficulty > 0)
                    {
                        currentWord.NativeDifficulty--;
                    }
                }
                goods++;
            }
            else
            {
                labelNews.Content = "---";
                labelLastPair.Foreground = Brushes.Red;
                labelNews.Foreground = Brushes.Red;
                if(ForeignToNative)
                {
                    currentWord.ForeignDifficulty++;
                }
                else
                {
                    currentWord.NativeDifficulty++;
                }
                bads++;
            }
            labelLastPair.Content = (ForeignToNative ? currentWord.Foreign : currentWord.Native) + " = " + (ForeignToNative ? currentWord.Native : currentWord.Foreign);
            Build();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Solve(1);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Solve(2);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Solve(3);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            Solve(4);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int fontSize = (int)(Height-100)/12;
            labelWordToFind.FontSize = fontSize;
            button1.FontSize = fontSize;
            button2.FontSize = fontSize;
            button3.FontSize = fontSize;
            button4.FontSize = fontSize;
            button5.FontSize = fontSize;
        }

        private void level_up(object sender, MouseButtonEventArgs e)
        {
            LevelUp();
        }

        private void LevelUp()
        {
            DifficultyLevel++;
            UpdateLevel();
            labelLastPair.Content = "You increased the level of difficulty.";
            Build();
        }

        private void level_down(object sender, MouseButtonEventArgs e)
        {
            LevelDown();
        }

        private void LevelDown()
        {
            if (DifficultyLevel > 0)
            {
                DifficultyLevel--;
                UpdateLevel();
                labelLastPair.Content = "You lowered the level of difficulty.";
                Build();
            }
        }

        private void labelWordToFind_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            labelLastPair.Content = "Word Changed!";
            Build();
        }

        private void labelNews_ReverseDictionaryDirection(object sender, MouseButtonEventArgs e)
        {
            ForeignToNative = !ForeignToNative;
            labelLastPair.Content = "[" + (ForeignToNative ? Language2 : Language1) + "->" + (ForeignToNative ? Language1 : Language2) + "]";
            UpdateLevel();
            Build();
            hasVoice = false;
            foreach (InstalledVoice voice in synth.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                string Language = ForeignToNative ? Language2 : Language1;
                if (info.Culture.TwoLetterISOLanguageName.ToUpper().Contains(Language.ToUpper()))
                {
                    hasVoice = true;
                    synth.SelectVoice(voice.VoiceInfo.Name);
                    builder.Culture = voice.VoiceInfo.Culture;
                    break;
                }
            }
            if(!hasVoice)
            {
                //labelLastPair.Content = "Voieces at: https://www.microsoft.com/en-us/download/details.aspx?id=27224";
            }
        }
    }
}
