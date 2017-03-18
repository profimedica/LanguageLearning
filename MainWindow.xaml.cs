using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordLex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataAccess da = new DataAccess();
        Random rnd = new Random();

        int bads = 0;
        int goods = 0;

        int currentDifficulty = 0;
        List<Word> currentLevelWords = new List<Word>();
        Word currentWord = null;

        static int GoodAnsware { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            foreach(Word word in da.words)
            {
                if(word.Difficulty == currentDifficulty)
                {
                    currentLevelWords.Add(word);
                }
            }

            Build();
        }

        private void Build()
        {
            List<Button> availableButtons = new List<Button>() { button1, button2, button3, button4, button5 };
            GoodAnsware = rnd.Next(availableButtons.Count());
            currentWord = currentLevelWords[rnd.Next(currentLevelWords.Count())];
            availableButtons[GoodAnsware].Content = currentWord.Native;
            
            labelWordToFind.Content = currentWord.Foreign;
            if (GoodAnsware != 0) button1.Content = da.words[rnd.Next(da.words.Count())].Native;
            if (GoodAnsware != 1) button2.Content = da.words[rnd.Next(da.words.Count())].Native;
            if (GoodAnsware != 2) button3.Content = da.words[rnd.Next(da.words.Count())].Native;
            if (GoodAnsware != 3) button4.Content = da.words[rnd.Next(da.words.Count())].Native;
            if (GoodAnsware != 4) button5.Content = da.words[rnd.Next(da.words.Count())].Native;

            labelWordsLeft.Content = currentLevelWords.Count();
            labelWordsBads.Content = bads;
            labelWordsGoods.Content = goods;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Solve(0);
        }

        private void Solve(int v)
        {
            if (v == GoodAnsware)
            {
                labelLastPair.Foreground = Brushes.Green;
                currentLevelWords.Remove(currentWord);
                goods++;
            }
            else
            {
                labelLastPair.Foreground = Brushes.Red;
                currentWord.Difficulty++;
                bads++;
            }
            labelLastPair.Content = currentWord.Foreign + " = " + currentWord.Native;
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
    }
}
